using FTM.Domain.Entities.Applications;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FTM.Infrastructure.Repositories
{
    public class WorkRepository : IWorkRepository
    {
        private readonly AppIdentityDbContext _context;

        public WorkRepository(AppIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkExperience>> GetWorkByUserIdAsync(Guid userId)
        {
            return await _context.WorkExperiences
                .Where(w => w.UserId == userId)
                .Include(w => w.Positions)
                .OrderByDescending(w => w.StartDate)
                .ToListAsync();
        }

        public async Task<WorkExperience?> GetWorkByIdAsync(Guid id)
        {
            return await _context.WorkExperiences
                .Include(w => w.Positions)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<WorkExperience> CreateWorkAsync(WorkExperience workExperience)
        {
            _context.WorkExperiences.Add(workExperience);
            await _context.SaveChangesAsync();
            return workExperience;
        }

        public async Task<WorkExperience> UpdateWorkAsync(WorkExperience workExperience)
        {
            _context.WorkExperiences.Update(workExperience);
            await _context.SaveChangesAsync();
            return workExperience;
        }

        public async Task<bool> DeleteWorkAsync(Guid id)
        {
            var work = await GetWorkByIdAsync(id);
            if (work == null) return false;
            _context.WorkExperiences.Remove(work);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserOwnsWorkAsync(Guid userId, Guid workId)
        {
            return await _context.WorkExperiences.AnyAsync(w => w.Id == workId && w.UserId == userId);
        }

        public async Task<WorkPosition> AddPositionAsync(WorkPosition position)
        {
            _context.WorkPositions.Add(position);
            await _context.SaveChangesAsync();
            return position;
        }

        public async Task<WorkPosition> UpdatePositionAsync(WorkPosition position)
        {
            _context.WorkPositions.Update(position);
            await _context.SaveChangesAsync();
            return position;
        }

        public async Task<bool> DeletePositionAsync(Guid positionId)
        {
            var position = await _context.WorkPositions.FindAsync(positionId);
            if (position == null) return false;
            _context.WorkPositions.Remove(position);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
