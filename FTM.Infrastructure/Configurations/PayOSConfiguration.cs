using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace FTM.Infrastructure.Configurations
{
    public static class PayOSConfiguration
    {
        public static void AddPayOSServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Get PayOS configuration from appsettings.json
            var payOSSettings = new PayOSSettings
            {
                ClientId = configuration["PayOS:ClientId"] ?? "MOCK_CLIENT_ID",
                ApiKey = configuration["PayOS:ApiKey"] ?? "MOCK_API_KEY", 
                ChecksumKey = configuration["PayOS:ChecksumKey"] ?? "MOCK_CHECKSUM_KEY",
                WebhookUrl = configuration["PayOS:WebhookUrl"] ?? "http://localhost:5000/api/webhooks/payos"
            };

            // Register PayOS settings
            services.AddSingleton(payOSSettings);
        }
    }

    public class PayOSSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ChecksumKey { get; set; } = string.Empty;
        public string WebhookUrl { get; set; } = string.Empty;
    }
}