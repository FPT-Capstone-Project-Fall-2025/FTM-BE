using FTM.API.Helpers;
using FTM.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Diagnostics;

namespace FTM.API.Extensions
{
    public static class FTMAppContextExtensions
    {
        public static IServiceCollection AddIdentityAppDbContext(this IServiceCollection services)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            string connectionString = GetCustomConnectionString(Environment.GetEnvironmentVariable("DB_NAME_AUTHEN"));

            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseNpgsql(connectionString, options =>
                {
                    options.CommandTimeout(300);
                    options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });

                options.EnableDetailedErrors();          // REQUIRED
                options.EnableSensitiveDataLogging();    // REQUIRED
            },
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Scoped);



            return services;
        }


        public static IServiceCollection AddFTMDbContext(this IServiceCollection services)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            string connectionString = GetCustomConnectionString(Environment.GetEnvironmentVariable("DB_NAME_SYSTEM"));

            services.AddDbContext<FTMDbContext>(options =>
            {
                options.UseNpgsql(connectionString, options =>
                {
                    options.CommandTimeout(300);
                    options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });

                options.EnableDetailedErrors();          // REQUIRED
                options.EnableSensitiveDataLogging();    // REQUIRED
            },
            contextLifetime: ServiceLifetime.Scoped,
            optionsLifetime: ServiceLifetime.Scoped);

            return services;
        }


        private static string GetCustomConnectionString(string dbName)
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = Environment.GetEnvironmentVariable("DB_HOST"),
                Port = int.Parse(Environment.GetEnvironmentVariable("DB_PORT")),
                Database = dbName,
                Username = Environment.GetEnvironmentVariable("DB_USERNAME"),
                Password = Environment.GetEnvironmentVariable("DB_PASSWORD"),
                Timeout = 5,         
                CommandTimeout = 300, 
                Pooling = true,      
                TrustServerCertificate = true
            };

            return builder.ConnectionString;
        }
    }
}
