using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Specification.FTAuthorizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Application.IServices
{
    public interface IFTAuthorizationService
    {
        Task<FTAuthorizationDto> AddAsync(UpsertFTAuthorizationRequest request);
        Task<FTAuthorizationDto> UpdateAsync(UpsertFTAuthorizationRequest request);
        Task<bool> DeleteAsync(Guid authorizationId);
        Task<IReadOnlyList<FTAuthorizationDto>> GetListDetailsWithSpecificationAsync(FTAuthorizationSpecParams specParams);
        Task<FTAuthorizationListViewDto> GetAuthorizationListViewAsync(FTAuthorizationSpecParams specParams);
    }
}
