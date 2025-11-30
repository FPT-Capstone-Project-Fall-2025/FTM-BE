using FTM.Domain.DTOs.Notifications;
using FTM.Domain.Entities.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Interface
{
    public interface IFTNotificationRepository : IGenericRepository<FTNotification>
    {
        Task<List<FTNotification>> FindByuserIdAsync(Guid userId);
        Task<FTNotification?> FindByInvitationIdAsync(Guid invitationId);

    }
}
