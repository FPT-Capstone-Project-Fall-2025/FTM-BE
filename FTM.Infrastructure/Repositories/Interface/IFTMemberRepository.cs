using FTM.Domain.Entities.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Interface
{
    public interface IFTMemberRepository : IGenericRepository<FTMember>
    {
        Task<FTMember?> GetDetaildedById(Guid id);
        Task<List<FTMember>>  GetMembersTree(Guid ftId);
        Task<FTMember?> GetMemberById(Guid id);
        Task<bool> IsConnectedTo(Guid ftId, Guid userId);
        Task<bool> IsExisted(Guid ftId, Guid ftMemberId);
        Task<List<FTMember>> GetMembersWithoutUserAsync(Guid ftId);
    }
}
