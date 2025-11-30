using FTM.Domain.DTOs.FamilyTree;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using XAct.Users;

namespace FTM.Application.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            string userId = Context.UserIdentifier!; 
            _logger.LogInformation($"User {userId} connected to NotificationHub");
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userId = Context.UserIdentifier!;
            _logger.LogInformation($"User {userId} disconnected");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Send Invitation Notification To A Specific User
        /// </summary>
        public async Task SendInvitationNotification(FTInvitationDto invitation)
        {
            await Clients.User(invitation.InvitedUserId.ToString()).SendAsync("ReceiveInvitation", invitation);
        }

        public async Task SendNotificationToUser(Guid userId, string message)
        {
            await Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }
    }
}
