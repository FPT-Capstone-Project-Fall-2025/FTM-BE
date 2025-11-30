using FTM.Domain.Entities.Applications;
using FTM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;

namespace FTM.Infrastructure.Data.SeedData
{
    public class JsonImporter
    {
        private readonly FTMDbContext _context;

        public JsonImporter(FTMDbContext context)
        {
            _context = context;
        }

        public async Task ImportProvincesAsync()
        {
            try
            {
                // Check if provinces already exist
                if (await _context.Mprovinces.AnyAsync())
                {
                    Console.WriteLine("Provinces already exist, skipping import.");
                    return;
                }

                var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "province.json");
                
                if (!File.Exists(jsonPath))
                {
                    // Try alternative path
                    jsonPath = @"d:\FPT-University\FTM-BE\FTM.Infrastructure\Data\SeedData\province.json";
                }

                if (!File.Exists(jsonPath))
                {
                    Console.WriteLine($"Province JSON file not found at: {jsonPath}");
                    return;
                }

                var jsonContent = await File.ReadAllTextAsync(jsonPath);
                var provinceDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent);

                var provinces = new List<Mprovince>();

                foreach (var kvp in provinceDict)
                {
                    var province = kvp.Value;
                    provinces.Add(new Mprovince
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

                await _context.Mprovinces.AddRangeAsync(provinces);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Successfully imported {provinces.Count} provinces.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing provinces: {ex.Message}");
                throw;
            }
        }

        public async Task ImportWardsAsync()
        {
            try
            {
                // Check if wards already exist
                if (await _context.MWards.AnyAsync())
                {
                    Console.WriteLine("Wards already exist, skipping import.");
                    return;
                }

                var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "ward.json");
                
                if (!File.Exists(jsonPath))
                {
                    // Try alternative path
                    jsonPath = @"d:\FPT-University\FTM-BE\FTM.Infrastructure\Data\SeedData\ward.json";
                }

                if (!File.Exists(jsonPath))
                {
                    Console.WriteLine($"Ward JSON file not found at: {jsonPath}");
                    return;
                }

                var jsonContent = await File.ReadAllTextAsync(jsonPath);
                var wardDict = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent);

                var wards = new List<MWard>();
                int batchSize = 1000;
                int count = 0;

                foreach (var kvp in wardDict)
                {
                    var ward = kvp.Value;
                    wards.Add(new MWard
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
                    
                    // Process in batches to avoid memory issues
                    if (count % batchSize == 0)
                    {
                        await _context.MWards.AddRangeAsync(wards);
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"Processed {count} wards...");
                        wards.Clear();
                    }
                }

                // Process remaining wards
                if (wards.Any())
                {
                    await _context.MWards.AddRangeAsync(wards);
                    await _context.SaveChangesAsync();
                }

                Console.WriteLine($"Successfully imported {count} wards.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing wards: {ex.Message}");
                throw;
            }
        }

        public async Task ImportAllAsync()
        {
            await ImportProvincesAsync();
            await ImportWardsAsync();
        }
    }
}