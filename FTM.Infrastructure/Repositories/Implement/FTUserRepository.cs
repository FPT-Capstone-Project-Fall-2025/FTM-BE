using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
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
    public class FTUserRepository : GenericRepository<FTUser>, IFTUserRepository
    {
        private readonly FTMDbContext _context;
        private readonly ICurrentUserResolver _currentUserResolver;

        public FTUserRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) : base(context, currentUserResolver)
        {
            this._context = context;
            this._currentUserResolver = currentUserResolver;
        }

        public async Task<FTUser?> FindOwnerAsync(Guid ftId)
        {
            return await _context.FTUsers.Where(u => u.FTId == ftId && u.FTRole == FTMRole.FTOwner).FirstOrDefaultAsync();
        }
    }
}
