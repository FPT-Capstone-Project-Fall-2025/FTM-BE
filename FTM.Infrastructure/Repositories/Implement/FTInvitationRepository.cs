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
    public class FTInvitationRepository : GenericRepository<FTInvitation>, IFTInvitationRepository
    {
        private readonly FTMDbContext _context;
        private readonly ICurrentUserResolver _currentUserResolver;

        public FTInvitationRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) : base(context, currentUserResolver)
        {
            this._context = context;
            this._currentUserResolver = currentUserResolver;
        }

        public async Task<FTInvitation?> GetInvitationAsync(string token)
        {
            return await _context.FTInvitations.FirstOrDefaultAsync(i => i.Token.Equals(token));
        }
    }
}
