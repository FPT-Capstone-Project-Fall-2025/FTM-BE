using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Specification;
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
    public class FTMemberFileRepository : GenericRepository<FTMemberFile>, IFTMemberFileRepository
    {

        private readonly FTMDbContext _context;
        private readonly ICurrentUserResolver _currentUserResolver;

        public FTMemberFileRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) : base(context, currentUserResolver)
        {
            this._context = context;
            this._currentUserResolver = currentUserResolver;
        }

        public Task<FTMemberFile?> FindAvatarAsync(Guid ftMemberId)
        {
            return _context.FTMemberFiles.Where(f => f.FTMemberId == ftMemberId && f.Title.Equals($"Avatar{ftMemberId}") && f.IsDeleted == false).FirstOrDefaultAsync();
        }
    }
}
