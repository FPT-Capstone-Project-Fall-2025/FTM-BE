using FTM.Domain.Entities.Applications;
using FTM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FTM.API.Extensions
{
    public static class DataSeedExtension
    {
        public static async Task SeedDataAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var ftmContext = scope.ServiceProvider.GetRequiredService<FTMDbContext>();
            var identityContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Starting data seeding for both databases...");

                // Seed FTM Database
                logger.LogInformation("Seeding FTM Database (gp_test)...");
                await SeedProvincesAsync(ftmContext, logger, "FTM");
                await SeedWardsAsync(ftmContext, logger, "FTM");

                // Seed Identity Database  
                logger.LogInformation("Seeding Identity Database (gp_identity_test)...");
                await SeedProvincesAsync(identityContext, logger, "Identity");
                await SeedWardsAsync(identityContext, logger, "Identity");

                logger.LogInformation("Data seeding completed for both databases!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during data seeding");
            }
        }

        private static async Task SeedProvincesAsync(DbContext context, ILogger logger, string dbName)
        {
            var provinces = context.Set<Mprovince>();
            
            if (await provinces.AnyAsync())
            {
                logger.LogInformation($"Provinces already exist in {dbName} database, skipping...");
                return;
            }

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "FTM.Infrastructure", "Data", "SeedData", "province.json");
            
            logger.LogInformation($"Looking for province JSON at: {jsonPath}");
            
            if (!File.Exists(jsonPath))
            {
                logger.LogWarning($"Province JSON file not found at: {jsonPath}");
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var provinceDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent);

            var provinceList = new List<Mprovince>();

            foreach (var kvp in provinceDict)
            {
                var province = kvp.Value;
                provinceList.Add(new Mprovince
                {
                    Id = Guid.NewGuid(),
                    Code = province.code?.ToString(),
                    Name = province.name?.ToString(),
                    Type = province.type?.ToString(),
                    Slug = province.slug?.ToString(),
                    NameWithType = province.name_with_type?.ToString(),
                    CreatedBy = "System",
                    CreatedOn = DateTimeOffset.UtcNow,
                    LastModifiedBy = "System",
                    LastModifiedOn = DateTimeOffset.UtcNow,
                    IsDeleted = false
                });
            }

            await provinces.AddRangeAsync(provinceList);
            await context.SaveChangesAsync();

            logger.LogInformation($"Seeded {provinceList.Count} provinces successfully in {dbName} database!");
        }

        private static async Task SeedWardsAsync(DbContext context, ILogger logger, string dbName)
        {
            var wards = context.Set<MWard>();
            
            if (await wards.AnyAsync())
            {
                logger.LogInformation($"Wards already exist in {dbName} database, skipping...");
                return;
            }

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "FTM.Infrastructure", "Data", "SeedData", "ward.json");
            
            logger.LogInformation($"Looking for ward JSON at: {jsonPath}");
            
            if (!File.Exists(jsonPath))
            {
                logger.LogWarning($"Ward JSON file not found at: {jsonPath}");
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var wardDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent);

            var batchSize = 1000;
            var count = 0;
            var wardList = new List<MWard>();

            logger.LogInformation($"Starting to seed {wardDict.Count} wards in batches of {batchSize} for {dbName} database...");

            foreach (var kvp in wardDict)
            {
                var ward = kvp.Value;
                wardList.Add(new MWard
                {
                    Id = Guid.NewGuid(),
                    Code = ward.code?.ToString(),
                    Name = ward.name?.ToString(),
                    Type = ward.type?.ToString(),
                    Slug = ward.slug?.ToString(),
                    NameWithType = ward.name_with_type?.ToString(),
                    Path = ward.path?.ToString(),
                    PathWithType = ward.path_with_type?.ToString(),
                    CreatedBy = "System",
                    CreatedOn = DateTimeOffset.UtcNow,
                    LastModifiedBy = "System",
                    LastModifiedOn = DateTimeOffset.UtcNow,
                    IsDeleted = false
                });

                count++;

                if (count % batchSize == 0)
                {
                    await wards.AddRangeAsync(wardList);
                    await context.SaveChangesAsync();
                    logger.LogInformation($"Processed {count} wards for {dbName} database...");
                    wardList.Clear();
                }
            }

            if (wardList.Any())
            {
                await wards.AddRangeAsync(wardList);
                await context.SaveChangesAsync();
            }

            logger.LogInformation($"Seeded {count} wards successfully in {dbName} database!");
        }
    }
}