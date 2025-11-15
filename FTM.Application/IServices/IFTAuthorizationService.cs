using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
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
        Task<FTAuthorizationDto?> AddAsync(UpsertFTAuthorizationRequest request);
        Task<FTAuthorizationDto?> UpdateAsync(UpsertFTAuthorizationRequest request);
        Task<bool> HasPermissionAsync(Guid ftId, Guid userId, FeatureType feature, MethodType method);
        Task<FTAuthorizationListViewDto?> GetAuthorizationListViewAsync(FTAuthorizationSpecParams specParams);
        Task<int> CountAuthorizationListViewAsync(FTAuthorizationSpecParams specParams);
        Task<bool> IsAccessedToFamilyTreeAsync(Guid ftId, Guid userId);
        Task<bool> IsOwnerAsync(Guid ftId, Guid userId);
        Task<bool> IsGuestAsync(Guid ftId, Guid userId);
        Task SetMemberAuthorizationAsync(Guid ftId, Guid memberId);
        Task DeleteAuthorizationAsync(Guid ftId, Guid ftMemberId);
        Task<FTAuthorizationDto> GetAuthorizationAsync(Guid ftId, Guid ftMemberId);
    }
}
