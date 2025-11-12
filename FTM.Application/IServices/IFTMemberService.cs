using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Specification.FamilyTrees;
using FTM.Domain.Specification.FTMembers;
using FTM.Domain.Specification.FTUsers;
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
        Task<FTMemberTreeDto> GetMembersTree(Guid ftId);
        Task<MemberRelationshipDto> CheckRelationship(Guid ftMemberId);
        Task Delete(Guid ftMemberId);
        Task<FTMemberDetailsDto> GetByMemberId(Guid ftid, Guid memberId);
        Task<FTMemberDetailsDto> UpdateDetailsByMemberId(Guid ftId, UpdateFTMemberRequest request);
        Task<IReadOnlyList<FTMemberSimpleDto>> GetListOfMembers(FTMemberSpecParams specParams);
        Task<IReadOnlyList<FTUserDto>> GetListOfFTUsers(FTUserSpecParams specParams);
        Task<IReadOnlyList<FTMemberSimpleDto>> GetListOfMembersWithoutUser(Guid ftId);
        Task<int> CountMembers(FTMemberSpecParams specParams);
        Task<int> CountFTUsers(FTUserSpecParams specParams);
    }
}
