using FTM.Domain.DTOs.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Application.IServices
{
    public interface IFTNotificationService
    {
        Task<IReadOnlyList<FTNotificationDto>> FindByuserIdAsync();
        Task DeleteAsync(Guid relatedId);
    }
}
