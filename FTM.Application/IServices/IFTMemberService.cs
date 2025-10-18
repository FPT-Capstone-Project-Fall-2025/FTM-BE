using FTM.Domain.DTOs.FamilyTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Application.IServices
{
    public interface IFTMemberService
    {
        Task<FTMemberDetailsDto> GetByUserId(Guid FTId, Guid userId);
        Task<FTMemberDetailsDto> Add(Guid FTId, UpsertFTMemberRequest request);
    }
}
