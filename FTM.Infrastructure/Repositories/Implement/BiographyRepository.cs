using FTM.Domain.Constants;
using FTM.Domain.Entities.Applications;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using static FTM.Domain.Constants.Constants;

namespace FTM.Infrastructure.Repositories
{
    public class BiographyRepository : IBiographyRepository
    {
        private readonly AppIdentityDbContext _context;

        public BiographyRepository(AppIdentityDbContext context)
        {
            _context = context;
        }


        #region Biography Description

        public async Task<Biography?> GetDescriptionByUserIdAsync(Guid userId)
        {
            return await _context.Biographies
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Type == BiographyType.Description);
        }

        public async Task<Biography> CreateDescriptionAsync(Biography biography)
        {
            biography.Type = BiographyType.Description;
            biography.CreatedAt = DateTime.UtcNow;
            _context.Biographies.Add(biography);
            await _context.SaveChangesAsync();
            return biography;
        }

        public async Task<Biography> UpdateDescriptionAsync(Biography biography)
        {
            biography.UpdatedAt = DateTime.UtcNow;
            _context.Biographies.Update(biography);
            await _context.SaveChangesAsync();
            return biography;
        }

        #endregion

        #region Biography Events

        public async Task<IEnumerable<Biography>> GetEventsByUserIdAsync(Guid userId)
        {
            return await _context.Biographies
                .Where(b => b.UserId == userId && b.Type == BiographyType.Event)
                .OrderBy(b => b.EventDate)
                .ToListAsync();
        }

        public async Task<Biography?> GetEventByIdAsync(Guid id)
        {
            return await _context.Biographies
                .FirstOrDefaultAsync(b => b.Id == id && b.Type == BiographyType.Event);
        }

        public async Task<Biography?> GetUserEventByIdAsync(Guid userId, Guid eventId)
        {
            return await _context.Biographies
                .FirstOrDefaultAsync(b => b.Id == eventId && b.UserId == userId && b.Type == BiographyType.Event);
        }

        public async Task<Biography> CreateEventAsync(Biography biography)
        {
            biography.Type = BiographyType.Event;
            biography.CreatedAt = DateTime.UtcNow;
            _context.Biographies.Add(biography);
            await _context.SaveChangesAsync();
            return biography;
        }

        public async Task<Biography> UpdateEventAsync(Biography biography)
        {
            biography.UpdatedAt = DateTime.UtcNow;
            _context.Biographies.Update(biography);
            await _context.SaveChangesAsync();
            return biography;
        }

        public async Task<bool> DeleteEventAsync(Guid id)
        {
            var biography = await GetEventByIdAsync(id);
            if (biography == null)
                return false;

            _context.Biographies.Remove(biography);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserOwnsEventAsync(Guid userId, Guid eventId)
        {
            return await _context.Biographies
                .AnyAsync(b => b.Id == eventId && b.UserId == userId && b.Type == BiographyType.Event);
        }

        #endregion
    }
}