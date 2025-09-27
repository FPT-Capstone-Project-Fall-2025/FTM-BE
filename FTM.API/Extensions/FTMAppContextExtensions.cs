using FTM.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;

namespace FTM.API.Extensions
{
    public static class FTMAppContextExtensions
    {
        public static IServiceCollection AddIdentityAppDbContext(this IServiceCollection services)
        {
           
            string connectionString = GetCustomConnectionString("test_gp_identity");

            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseNpgsql(connectionString, options =>
                {
                    options.CommandTimeout(300);
                    options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            },
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Transient);

            return services;
        }


        public static IServiceCollection AddFTMDbContext(this IServiceCollection services)
        {
            string connectionString = GetCustomConnectionString("test_gp");

            services.AddDbContext<FTMDbContext>(options =>
            {
                options.UseNpgsql(connectionString, options =>
                {
                    options.CommandTimeout(300);
                    options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            },
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Transient);

            return services;
        }


        private static string GetCustomConnectionString(string dbName)
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = "128.199.168.119",
                Port = 5432,
                Database = dbName,
                Username = "appuser",
                Password = "secret@123",
                Timeout = 5,         
                CommandTimeout = 300, 
                Pooling = true,      
                TrustServerCertificate = true
            };

            return builder.ConnectionString;
        }
    }
}
