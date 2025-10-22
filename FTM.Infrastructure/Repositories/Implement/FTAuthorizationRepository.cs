using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Implement
{
    public class FTAuthorizationRepository : GenericRepository<FTAuthorization>, IFTAuthorizationRepository
    {
        private readonly FTMDbContext _context;
        private readonly ICurrentUserResolver _currentUserResolver;
        public FTAuthorizationRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) : base(context, currentUserResolver)
        {
            this._context = context;
            this._currentUserResolver = currentUserResolver;
        }

        public async Task<bool> IsAuthorizationExisting(Guid ftId,Guid ftMemberId,FeatureType featureType, MethodType methodType)
        {
            return await _context.FTAuthorizations.AnyAsync(a =>
                                                           (a.FeatureCode == featureType || a.FeatureCode == FeatureType.ALL) &&
                                                           (a.MethodCode == methodType || a.MethodCode == MethodType.ALL)
                                                            && a.IsDeleted == false
                                                            &&a.FTId == ftId
                                                            &&a.FTMemberId == ftMemberId);
        }
    }
}
