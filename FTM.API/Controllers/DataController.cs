using FTM.API.Extensions;
using FTM.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly FTMDbContext _context;

        public DataController(FTMDbContext context)
        {
            _context = context;
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _context.Mprovinces
                .OrderBy(p => p.Name)
                .Select(p => new { p.Code, p.Name, p.Type, p.NameWithType })
                .ToListAsync();

            return Ok(new { Count = provinces.Count, Data = provinces.Take(10) });
        }

        [HttpGet("wards")]
        public async Task<IActionResult> GetWards()
        {
            var wards = await _context.MWards
                .OrderBy(w => w.Name)
                .Select(w => new { w.Code, w.Name, w.Type, w.Path })
                .ToListAsync();

            return Ok(new { Count = wards.Count, Data = wards.Take(10) });
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetDataCount()
        {
            var provinceCount = await _context.Mprovinces.CountAsync();
            var wardCount = await _context.MWards.CountAsync();

            return Ok(new { 
                Provinces = provinceCount, 
                Wards = wardCount,
                Message = provinceCount == 0 && wardCount == 0 ? "No data found. Please restart the app to seed data." : "Data available"
            });
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedData()
        {
            try
            {
                await HttpContext.RequestServices.SeedDataAsync();
                return Ok(new { Message = "Data seeding completed successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error seeding data: {ex.Message}" });
            }
        }
    }
}