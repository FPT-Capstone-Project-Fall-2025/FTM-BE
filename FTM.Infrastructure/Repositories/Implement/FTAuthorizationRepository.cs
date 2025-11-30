using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using FTM.Domain.Models;
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

        public async Task<FTAuthorizationDto?> GetAuthorizationAsync(Guid ftId, Guid ftMemberId)
        {
            return await _context.FTAuthorizations
                                    .Where(a => a.FTId == ftId && a.FTMemberId == ftMemberId)
                                    .Include(a => a.AuthorizedMember)
                                    .GroupBy(a => new { a.FTId, a.FTMemberId })
                                    .Select(gr => new FTAuthorizationDto
                                    {
                                        FTId = gr.Key.FTId,
                                        FTMemberId = gr.Key.FTMemberId,
                                        AuthorizationList = gr
                                            .GroupBy(x => x.FeatureCode)
                                            .Select(featureGroup => new AuthorizationModel
                                            {
                                                FeatureCode = featureGroup.Key,
                                                MethodsList = featureGroup
                                                    .Select(x => x.MethodCode)
                                                    .Distinct()
                                                    .ToHashSet()
                                            })
                                            .ToHashSet()
                                    })
                                    .FirstOrDefaultAsync();
        }

        public async Task<List<FTAuthorization>> GetListAsync(Guid ftId, Guid ftMemberId)
        {
            return await _context.FTAuthorizations
                                    .Where(a => a.FTId == ftId && a.FTMemberId == ftMemberId).ToListAsync();
        }

        public async Task<bool> HasPermissionAsync(Guid ftId, Guid userId, FeatureType feature, MethodType method)
        {
            return await _context.FTAuthorizations
                .AnyAsync(a =>
                            a.FTId == ftId
                            && a.AuthorizedMember.UserId == userId
                            && (a.FeatureCode == FeatureType.ALL || a.FeatureCode == feature)
                            && (a.MethodCode == MethodType.ALL || a.MethodCode == method)
                            && a.IsDeleted == false
                        );
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
