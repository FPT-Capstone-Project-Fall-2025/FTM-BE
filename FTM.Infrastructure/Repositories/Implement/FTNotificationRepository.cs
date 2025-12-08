using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Entities.Notifications;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Implement
{
    public class FTNotificationRepository : GenericRepository<FTNotification>, IFTNotificationRepository
    {
        private readonly FTMDbContext _context;
        private readonly ICurrentUserResolver _currentUserResolver;

        public FTNotificationRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) : base(context, currentUserResolver)
        {
            this._context = context;
            this._currentUserResolver = currentUserResolver;
        }

        public async Task<FTNotification?> FindByInvitationIdAsync(Guid invitationId)
        {
            return await _context.FTNotifications
                .Where(n => n.RelatedId == invitationId && n.IsDeleted == false && n.IsRead == false)
                .FirstOrDefaultAsync();
        }


        public async Task<List<FTNotification>> FindByuserIdAsync(Guid userId)
        {
            return _context.FTNotifications.Where(n => n.UserId == userId && n.IsDeleted == false && n.IsRead == false).OrderByDescending(n => n.CreatedOn).ToList();
        }
    }
}
