using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Specification.HonorBoard;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface ICareerHonorRepository
    {
        Task<IEnumerable<CareerHonor>> GetCareerHonorsAsync(CareerHonorSpecParams specParams);
        Task<int> GetTotalCountAsync(CareerHonorSpecParams specParams);
        Task<CareerHonor?> GetCareerHonorByIdAsync(Guid id);
        Task<CareerHonor> CreateCareerHonorAsync(CareerHonor careerHonor);
        Task<CareerHonor> UpdateCareerHonorAsync(CareerHonor careerHonor);
        Task<bool> DeleteCareerHonorAsync(Guid id);
        Task<bool> MemberExistsInFamilyTreeAsync(Guid memberId, Guid familyTreeId);
    }
}
