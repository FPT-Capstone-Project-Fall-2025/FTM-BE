using FTM.Domain.Entities.FamilyTree;
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
    }
}
