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
    public class FTUserRepository : GenericRepository<FTUser>, IFTUserRepository
    {
        private readonly FTMDbContext _context;
        private readonly ICurrentUserResolver _currentUserResolver;

        public FTUserRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) : base(context, currentUserResolver)
        {
            this._context = context;
            this._currentUserResolver = currentUserResolver;
        }

        public async Task<bool> IsUserExistingInFamilyTreeAsync(Guid ftId, Guid userId)
        {
            return await _context.FTUsers.AnyAsync(u => u.FTId == ftId && u.UserId == userId && u.IsDeleted == false);
        }

        public async Task<FTUser?> FindOwnerAsync(Guid ftId)
        {
            return await _context.FTUsers.Where(u => u.FTId == ftId && u.FTRole == FTMRole.FTOwner && u.IsDeleted == false).FirstOrDefaultAsync();
        }

        public async Task<FTUser?> FindAsync(Guid ftId, Guid userId)
        {
            return await _context.FTUsers.Where(u => u.FTId == ftId && u.UserId == userId && u.IsDeleted == false).FirstOrDefaultAsync();
        }

        public async Task<bool> BelongedToAsync(Guid ftId, Guid userId)
        {
            return await _context.FTUsers.AnyAsync(u => u.FTId == ftId && u.UserId == userId && u.IsDeleted == false);
        }

        public async Task<bool> IsOwnerAsync(Guid ftId, Guid userId)
        {
            return await _context.FTUsers.AnyAsync(u => u.FTId == ftId && u.UserId == userId && u.FTRole == FTMRole.FTOwner && u.IsDeleted == false);
        }

        public async Task<bool> IsGuestAsync(Guid ftId, Guid userId)
        {
            return await _context.FTUsers.AnyAsync(u => u.FTId == ftId && u.UserId == userId && u.FTRole == FTMRole.FTGuest && u.IsDeleted == false);
        }
    }
}
