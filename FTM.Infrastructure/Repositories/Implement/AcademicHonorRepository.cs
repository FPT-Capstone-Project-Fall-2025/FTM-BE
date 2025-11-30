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
    public class AcademicHonorRepository : IAcademicHonorRepository
    {
        private readonly FTMDbContext _context;

        public AcademicHonorRepository(FTMDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AcademicHonor>> GetAcademicHonorsAsync(AcademicHonorSpecParams specParams)
        {
            var query = _context.AcademicHonors
                .Include(a => a.GPMember)
                .Include(a => a.FamilyTree)
                .Where(a => a.IsDeleted == false)
                .AsQueryable();

            // Apply filters
            if (specParams.FamilyTreeId.HasValue)
            {
                query = query.Where(a => a.FamilyTreeId == specParams.FamilyTreeId.Value);
            }

            if (specParams.GPMemberId.HasValue)
            {
                query = query.Where(a => a.GPMemberId == specParams.GPMemberId.Value);
            }

            if (specParams.Year.HasValue)
            {
                query = query.Where(a => a.YearOfAchievement == specParams.Year.Value);
            }

            if (!string.IsNullOrWhiteSpace(specParams.InstitutionName))
            {
                query = query.Where(a => a.InstitutionName.Contains(specParams.InstitutionName));
            }

            if (specParams.IsDisplayed.HasValue)
            {
                query = query.Where(a => a.IsDisplayed == specParams.IsDisplayed.Value);
            }

            // Apply sorting (default: newest first, then by year descending)
            query = query.OrderByDescending(a => a.CreatedOn)
                         .ThenByDescending(a => a.YearOfAchievement);

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

        public async Task<int> GetTotalCountAsync(AcademicHonorSpecParams specParams)
        {
            var query = _context.AcademicHonors
                .Where(a => a.IsDeleted == false)
                .AsQueryable();

            // Apply same filters as GetAcademicHonorsAsync
            if (specParams.FamilyTreeId.HasValue)
            {
                query = query.Where(a => a.FamilyTreeId == specParams.FamilyTreeId.Value);
            }

            if (specParams.GPMemberId.HasValue)
            {
                query = query.Where(a => a.GPMemberId == specParams.GPMemberId.Value);
            }

            if (specParams.Year.HasValue)
            {
                query = query.Where(a => a.YearOfAchievement == specParams.Year.Value);
            }

            if (!string.IsNullOrWhiteSpace(specParams.InstitutionName))
            {
                query = query.Where(a => a.InstitutionName.Contains(specParams.InstitutionName));
            }

            if (specParams.IsDisplayed.HasValue)
            {
                query = query.Where(a => a.IsDisplayed == specParams.IsDisplayed.Value);
            }

            return await query.CountAsync();
        }

        public async Task<AcademicHonor?> GetAcademicHonorByIdAsync(Guid id)
        {
            return await _context.AcademicHonors
                .Include(a => a.GPMember)
                .Include(a => a.FamilyTree)
                .FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == false);
        }

        public async Task<AcademicHonor> CreateAcademicHonorAsync(AcademicHonor academicHonor)
        {
            _context.AcademicHonors.Add(academicHonor);
            await _context.SaveChangesAsync();
            
            // Reload with navigation properties
            return (await GetAcademicHonorByIdAsync(academicHonor.Id))!;
        }

        public async Task<AcademicHonor> UpdateAcademicHonorAsync(AcademicHonor academicHonor)
        {
            _context.AcademicHonors.Update(academicHonor);
            await _context.SaveChangesAsync();
            
            // Reload with navigation properties
            return (await GetAcademicHonorByIdAsync(academicHonor.Id))!;
        }

        public async Task<bool> DeleteAcademicHonorAsync(Guid id)
        {
            var honor = await _context.AcademicHonors.FindAsync(id);
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
