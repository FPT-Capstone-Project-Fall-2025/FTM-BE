using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using FTM.Domain.Specification.HonorBoard;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FTM.Infrastructure.Repositories.Implement
{
    public class CareerHonorRepository : ICareerHonorRepository
    {
        private readonly FTMDbContext _context;

        public CareerHonorRepository(FTMDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CareerHonor>> GetCareerHonorsAsync(CareerHonorSpecParams specParams)
        {
            var query = _context.CareerHonors
                .Include(c => c.GPMember)
                .Include(c => c.FamilyTree)
                .Where(c => c.IsDeleted == false)
                .AsQueryable();

            // Apply filters
            if (specParams.FamilyTreeId.HasValue)
            {
                query = query.Where(c => c.FamilyTreeId == specParams.FamilyTreeId.Value);
            }

            if (specParams.GPMemberId.HasValue)
            {
                query = query.Where(c => c.GPMemberId == specParams.GPMemberId.Value);
            }

            if (specParams.Year.HasValue)
            {
                query = query.Where(c => c.YearOfAchievement == specParams.Year.Value);
            }

            if (!string.IsNullOrWhiteSpace(specParams.OrganizationName))
            {
                query = query.Where(c => c.OrganizationName.Contains(specParams.OrganizationName));
            }

            if (specParams.IsDisplayed.HasValue)
            {
                query = query.Where(c => c.IsDisplayed == specParams.IsDisplayed.Value);
            }

            // Apply sorting (default: newest first, then by year descending)
            query = query.OrderByDescending(c => c.CreatedOn)
                         .ThenByDescending(c => c.YearOfAchievement);

            // Apply pagination
            if (specParams.Skip > 0)
            {
                query = query.Skip(specParams.Skip);
            }

            if (specParams.Take > 0)
            {
                query = query.Take(specParams.Take);
            }

            return await query.ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(CareerHonorSpecParams specParams)
        {
            var query = _context.CareerHonors
                .Where(c => c.IsDeleted == false)
                .AsQueryable();

            // Apply same filters as GetCareerHonorsAsync
            if (specParams.FamilyTreeId.HasValue)
            {
                query = query.Where(c => c.FamilyTreeId == specParams.FamilyTreeId.Value);
            }

            if (specParams.GPMemberId.HasValue)
            {
                query = query.Where(c => c.GPMemberId == specParams.GPMemberId.Value);
            }

            if (specParams.Year.HasValue)
            {
                query = query.Where(c => c.YearOfAchievement == specParams.Year.Value);
            }

            if (!string.IsNullOrWhiteSpace(specParams.OrganizationName))
            {
                query = query.Where(c => c.OrganizationName.Contains(specParams.OrganizationName));
            }

            if (specParams.IsDisplayed.HasValue)
            {
                query = query.Where(c => c.IsDisplayed == specParams.IsDisplayed.Value);
            }

            return await query.CountAsync();
        }

        public async Task<CareerHonor?> GetCareerHonorByIdAsync(Guid id)
        {
            return await _context.CareerHonors
                .Include(c => c.GPMember)
                .Include(c => c.FamilyTree)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);
        }

        public async Task<CareerHonor> CreateCareerHonorAsync(CareerHonor careerHonor)
        {
            _context.CareerHonors.Add(careerHonor);
            await _context.SaveChangesAsync();
            
            // Reload with navigation properties
            return (await GetCareerHonorByIdAsync(careerHonor.Id))!;
        }

        public async Task<CareerHonor> UpdateCareerHonorAsync(CareerHonor careerHonor)
        {
            _context.CareerHonors.Update(careerHonor);
            await _context.SaveChangesAsync();
            
            // Reload with navigation properties
            return (await GetCareerHonorByIdAsync(careerHonor.Id))!;
        }

        public async Task<bool> DeleteCareerHonorAsync(Guid id)
        {
            var honor = await _context.CareerHonors.FindAsync(id);
            if (honor == null) return false;

            honor.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MemberExistsInFamilyTreeAsync(Guid memberId, Guid familyTreeId)
        {
            return await _context.FTMembers
                .AnyAsync(m => m.Id == memberId && m.FTId == familyTreeId && m.IsDeleted == false);
        }
    }
}
