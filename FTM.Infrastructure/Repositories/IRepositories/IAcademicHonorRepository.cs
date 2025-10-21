using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Specification.HonorBoard;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IAcademicHonorRepository
    {
        Task<IEnumerable<AcademicHonor>> GetAcademicHonorsAsync(AcademicHonorSpecParams specParams);
        Task<int> GetTotalCountAsync(AcademicHonorSpecParams specParams);
        Task<AcademicHonor?> GetAcademicHonorByIdAsync(Guid id);
        Task<AcademicHonor> CreateAcademicHonorAsync(AcademicHonor academicHonor);
        Task<AcademicHonor> UpdateAcademicHonorAsync(AcademicHonor academicHonor);
        Task<bool> DeleteAcademicHonorAsync(Guid id);
        Task<bool> MemberExistsInFamilyTreeAsync(Guid memberId, Guid familyTreeId);
    }
}
