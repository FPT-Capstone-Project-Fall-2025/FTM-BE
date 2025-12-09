using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Application.IServices
{
    public interface IRedisCacheService
    {
        Task<T?> GetDataAsync<T>(string key);
        Task<bool> IsRedisConnectedAsync();
        Task SetDataAsync<T>(string key, T value, int absoluteMinutesExp = 10, int slidingMinutesExp = 0);
        Task RemoveDataAsync(string key);
    }
}
