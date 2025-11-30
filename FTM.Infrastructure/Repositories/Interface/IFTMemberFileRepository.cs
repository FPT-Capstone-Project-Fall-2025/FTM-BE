using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Interface
{
    public interface IFTMemberFileRepository : IGenericRepository<FTMemberFile>
    {
        Task<FTMemberFile?> FindAvatarAsync(Guid ftMemberId);
    }
}
