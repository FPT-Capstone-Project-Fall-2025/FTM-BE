using FTM.Application.IServices;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace FTM.Application.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<RedisCacheService> _logger;
        private const string KeyPrefix = "AppPrefix_";
        private readonly AsyncPolicy _retryPolicy;

        public RedisCacheService(IDistributedCache cache, IMemoryCache memoryCache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _memoryCache = memoryCache;
            _logger = logger;
            _retryPolicy = Policy.Handle<RedisConnectionException>()
                                .Or<RedisTimeoutException>()
                                .Or<Exception>()
                                .WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(200 * attempt),
                                    (exception, timespan, retryCount, context) =>
                                    {
                                        _logger.LogWarning(exception, "Redis operation failed. Waiting {Delay} before next entry. Attempt {RetryCount}", timespan, retryCount);

                                    });

        }

        public async Task<bool> IsRedisConnectedAsync()
        {
            string testKey = $"{KeyPrefix}__redis_health_check__";
            string testValue = Guid.NewGuid().ToString();

            try
            {
                await _retryPolicy.ExecuteAsync(() =>
                    _cache.SetStringAsync(testKey, testValue,
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                        })
                );

                var value = await _retryPolicy.ExecuteAsync(() => _cache.GetStringAsync(testKey));
                await _cache.RemoveAsync(testKey);

                return value == testValue;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis health check failed. Falling back to MemoryCache.");
                return false;
            }
        }

        public async Task<T> GetDataAsync<T>(string key)
        {
            string cacheKey = KeyPrefix + key;

            try
            {
                var value = await _retryPolicy.ExecuteAsync(() => _cache.GetStringAsync(cacheKey));
                if (!string.IsNullOrEmpty(value))
                {
                    return typeof(T) == typeof(string)
                        ? (T)(object)value
                        : JsonSerializer.Deserialize<T>(value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis get failed, trying MemoryCache.");
            }

            if (_memoryCache.TryGetValue(cacheKey, out T memoryValue))
            {
                _logger.LogInformation("MemoryCache hit for key {Key}", key);
                return memoryValue;
            }

            _logger.LogInformation("Cache miss for key {Key}", key);
            return default;
        }

        public async Task SetDataAsync<T>(string key, T value, int absoluteMinutesExp = 10, int slidingMinutesExp = 0)
        {
            string cacheKey = KeyPrefix + key;

            string cacheValue = typeof(T) == typeof(string)
                ? value as string
                : JsonSerializer.Serialize(value);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(absoluteMinutesExp)
            };
            if (slidingMinutesExp > 0)
                cacheOptions.SlidingExpiration = TimeSpan.FromMinutes(slidingMinutesExp);

            try
            {
                await _retryPolicy.ExecuteAsync(() => _cache.SetStringAsync(cacheKey, cacheValue, cacheOptions));
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis set failed, falling back to MemoryCache.");
            }

            var memoryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(absoluteMinutesExp)
            };
            if (slidingMinutesExp > 0)
                memoryOptions.SlidingExpiration = TimeSpan.FromMinutes(slidingMinutesExp);

            _memoryCache.Set(cacheKey, value, memoryOptions);
        }

        public async Task RemoveDataAsync(string key)
        {
            string cacheKey = KeyPrefix + key;

            try
            {
                await _retryPolicy.ExecuteAsync(() => _cache.RemoveAsync(cacheKey));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis remove failed, falling back to MemoryCache.");
            }

            _memoryCache.Remove(cacheKey);
        }
    }
}
