using FTM.Domain.Entities.FamilyTree;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
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
    }
}
