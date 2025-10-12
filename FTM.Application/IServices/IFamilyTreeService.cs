using FTM.Domain.DTOs.FamilyTree;
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
        Task<List<FamilyTreeDataTableDto>> GetFamilyTreesAsync();
        Task<List<FamilyTreeDataTableDto>> GetMyFamilyTreesAsync();
    }
}