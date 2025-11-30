using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Specification.FamilyTrees;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTM.Application.IServices
{
    public interface IFamilyTreeService
    {
        Task<FamilyTreeDetailsDto> CreateFamilyTreeAsync(UpsertFamilyTreeRequest request);
        Task<FamilyTreeDetailsDto> GetFamilyTreeByIdAsync(Guid id);
        Task<FamilyTreeDetailsDto> UpdateFamilyTreeAsync(Guid id, UpsertFamilyTreeRequest request);
        Task DeleteFamilyTreeAsync(Guid id);
        Task<IReadOnlyList<FamilyTreeDataTableDto>> GetFamilyTreesAsync(FamilyTreeSpecParams specParams);
        Task<int> CountFamilyTreesAsync(FamilyTreeSpecParams specParams);
        Task<int> CountMyFamilyTreesAsync(FamilyTreeSpecParams specParams);
        Task<IReadOnlyList<FamilyTreeDataTableDto>> GetMyFamilyTreesAsync(FamilyTreeSpecParams specParams);
    }
}