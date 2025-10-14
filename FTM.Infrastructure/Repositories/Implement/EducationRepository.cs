using FTM.Domain.Entities.Applications;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FTM.Infrastructure.Repositories
{
    public class EducationRepository : IEducationRepository
    {
        private readonly AppIdentityDbContext _context;

        public EducationRepository(AppIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Education>> GetEducationsByUserIdAsync(Guid userId)
        {
            return await _context.Educations
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<Education?> GetEducationByIdAsync(Guid id)
        {
            return await _context.Educations.FindAsync(id);
        }

        public async Task<Education> CreateEducationAsync(Education education)
        {
            _context.Educations.Add(education);
            await _context.SaveChangesAsync();
            return education;
        }

        public async Task<Education> UpdateEducationAsync(Education education)
        {
            _context.Educations.Update(education);
            await _context.SaveChangesAsync();
            return education;
        }

        public async Task<bool> DeleteEducationAsync(Guid id)
        {
            var education = await GetEducationByIdAsync(id);
            if (education == null) return false;
            _context.Educations.Remove(education);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserOwnsEducationAsync(Guid userId, Guid educationId)
        {
            return await _context.Educations.AnyAsync(e => e.Id == educationId && e.UserId == userId);
        }
    }
}
