using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Specification.FTInvitations;
using FTM.Domain.Specification.FTMembers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Application.IServices
{
    public interface IFTInvitationService
    {
        Task SendAsync(FTInvitation invitation);
        Task AddAsync(FTInvitation invitation);
        Task HandleRespondAsync(Guid invitationId, bool accepted);
        Task InviteToGuestAsync(GuestInvitationDto request);
        Task InviteToMemberAsync(MemberInvitationDto request);
        Task<IReadOnlyList<SimpleFTInvitationDto>> ListAsync(FTInvitationSpecParams specParams);
        Task<int> CountListAsync(FTInvitationSpecParams specParams);
    }
}
