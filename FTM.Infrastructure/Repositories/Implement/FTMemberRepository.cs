using FTM.Domain.Entities.FamilyTree;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Implement
{
    public class FTMemberRepository : GenericRepository<FTMember>, IFTMemberRepository
    {
        private readonly FTMDbContext _context;
        private readonly ICurrentUserResolver _currentUserResolver;

        public FTMemberRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) : base(context, currentUserResolver)
        {
            this._context = context;
            this._currentUserResolver = currentUserResolver;
        }

        public async Task<FTMember?> GetDetaildedById(Guid id)
        {
            return await _context.FTMembers.Include(m => m.Ethnic)
                              .Include(m => m.Religion)
                              .Include(m => m.Ward)
                              .Include(m => m.Province)
                              .Include(m => m.BurialWard)
                              .Include(m => m.BurialProvince)
                              .Include(m => m.FTMemberFiles)
                              .Where(m => m.IsDeleted == false)
                              .FirstOrDefaultAsync(m =>  m.Id == id);
        }

        public async Task<FTMember?> GetMemberById(Guid id)
        {
            return await _context.FTMembers.Include(m => m.Ethnic) 
                              .Include(m => m.FT)
                              .Include(m => m.FTRelationshipFrom)
                                .ThenInclude(x => x.ToFTMember)
                                    .ThenInclude(x => x.FTRelationshipFrom)
                              .Include(m => m.FTRelationshipTo)
                                .ThenInclude(x => x.FromFTMember)
                                    .ThenInclude(x => x.FTRelationshipFrom)
                                        .ThenInclude(x => x.FromFTMemberPartner)
                                            .ThenInclude(x => x.FTRelationshipFrom)
                              .Include(m => m.FTMemberFiles)
                              .Where(m => m.IsDeleted == false)
                              .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<FTMember>> GetMembersTree(Guid ftId)
        {
            return await _context.FTMembers
                .Include(m => m.FTRelationshipFrom)
                .Include(m => m.FTRelationshipTo)
                .Include(m => m.FTMemberFiles)
                .Where(x => x.FTId == ftId && x.IsDeleted == false)
                .OrderBy(m => m.Birthday)
                .ToListAsync();
        }

        public async Task<List<FTMember>> GetMembersWithoutUserAsync(Guid ftId)
        {
            return await _context.FTMembers.Include(m => m.FTMemberFiles)
                                           .Where(m => m.FTId == ftId 
                                                    && (m.UserId == null || m.UserId == Guid.Empty) 
                                                    && m.IsDeleted == false).ToListAsync();
        }

        public async Task<bool> IsConnectedTo(Guid ftId, Guid userId)   
        {
            return await _context.FTMembers.AnyAsync(m => m.FTId == ftId && m.UserId == userId && m.IsDeleted == false);
        }

        public async Task<bool> IsExisted(Guid ftId, Guid ftMemberId)
        {
            return await _context.FTMembers.AnyAsync(m => m.FTId == ftId && m.Id == ftMemberId && m.IsDeleted == false);
        }
    }
}
