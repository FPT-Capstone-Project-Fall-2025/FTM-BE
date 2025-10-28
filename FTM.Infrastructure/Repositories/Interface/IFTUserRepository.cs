using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Interface
{
    public interface IFTUserRepository : IGenericRepository<FTUser>
    {
        Task<FTUser?> FindOwnerAsync(Guid ftId);
        Task<bool> IsUserExistingInFamilyTreeAsync(Guid ftId, Guid userId);
        Task<FTUser?> FindAsync(Guid ftId, Guid userId);

    }
}
