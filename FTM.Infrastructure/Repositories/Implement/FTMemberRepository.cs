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
                              .FirstOrDefaultAsync(m =>  m.Id == id);
        }
    }
}
