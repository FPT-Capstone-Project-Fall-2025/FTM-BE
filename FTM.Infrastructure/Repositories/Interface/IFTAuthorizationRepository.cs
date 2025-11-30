using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Interface
{
    public interface IFTAuthorizationRepository : IGenericRepository<FTAuthorization>
    {
        Task<bool> IsAuthorizationExisting(Guid ftId, Guid ftMemberId, FeatureType featureType, MethodType methodType);
        Task<FTAuthorizationDto?> GetAuthorizationAsync(Guid ftId, Guid ftMemberId);
        Task<bool> HasPermissionAsync(Guid ftId, Guid userId, FeatureType feature, MethodType method);
        Task<List<FTAuthorization>> GetListAsync(Guid ftId, Guid ftMemberId);
    }
}
