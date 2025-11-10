using AutoMapper;
using FTM.Application.Hubs;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.DTOs.Notifications;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Entities.Identity;
using FTM.Domain.Entities.Notifications;
using FTM.Domain.Enums;
using FTM.Infrastructure.Repositories.Interface;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Application.Services
{
    public class FTInvitationService : IFTInvitationService
    {
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserResolver _currentUserResolver;
        private readonly IFTInvitationRepository _fTInvitationRepository;
        private readonly IFTUserRepository _fTUserRepository;
        private readonly IFTMemberRepository _fTMemberRepository;
        private readonly IFTNotificationRepository _fTNotificationRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMapper _mapper;

        public FTInvitationService(
            IEmailSender emailSender,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            ICurrentUserResolver currentUserResolver,
            IFTInvitationRepository fTInvitationRepository,
            IFTUserRepository fTUserRepository,
            IFTMemberRepository fTmemberRepository,
            IFTNotificationRepository fTNotificationRepository,
            IHubContext<NotificationHub> hubContext,
            IMapper mapper
            )
        {
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _currentUserResolver = currentUserResolver;
            _fTInvitationRepository = fTInvitationRepository;
            _fTUserRepository = fTUserRepository;
            _fTMemberRepository = fTmemberRepository;
            _fTNotificationRepository = fTNotificationRepository;
            _hubContext = hubContext;
            _mapper = mapper;
        }

        public async Task AddAsync(FTInvitation invitation)
        {
            await _fTInvitationRepository.AddAsync(invitation);
        }

        public async Task HandleRespondAsync(Guid invitationId, bool accepted)
        {

            var invitation = await _fTInvitationRepository.GetByIdAsync(invitationId);

            if (invitation == null)
                throw new ArgumentException("Lời mời không tồn tại.");

            if (invitation.ExpirationDate < DateTime.UtcNow)
                throw new ArgumentException("Lời mời đã hết hạn.");

            if (invitation.Status != InvitationStatus.PENDING)
                throw new ArgumentException("Lời mời này đã được phản hồi.");

            invitation.Status = accepted ? InvitationStatus.ACCEPTED : InvitationStatus.REJECTED;

            if (invitation.Status == InvitationStatus.ACCEPTED)
            {
                var invitedUser = await _userManager.FindByIdAsync(invitation.InvitedUserId.ToString());
                var isUserExistedInFamilyTree = await _fTUserRepository.IsUserExistingInFamilyTreeAsync(invitation.FTId, invitation.InvitedUserId);
                var connectedFTMember = await _fTMemberRepository.GetByIdAsync(invitation.FTMemberId);
                var isConnectedToMember = await _fTMemberRepository.IsConnectedTo(invitation.FTId, invitation.InvitedUserId);

                if (invitedUser != null && connectedFTMember != null)
                {
                    if (isConnectedToMember)
                    {
                        throw new ArgumentException("Nguời dùng đã được liên kết với một thành viên khác trong gia phả");
                    }

                    connectedFTMember.UserId = invitedUser.Id;

                    if (!isUserExistedInFamilyTree)
                    {
                        var ftUser = new FTUser
                        {
                            UserId = invitedUser.Id,
                            Name = invitedUser.Name,
                            Username = invitedUser.UserName,
                            FTId = invitation.FTId,
                            FTRole = FTMRole.FTMember,
                        };
                        await _fTUserRepository.AddAsync(ftUser);
                    }
                    else
                    {
                        var ftUser =  await _fTUserRepository.FindAsync(invitation.FTId, invitation.InvitedUserId);
                        if(ftUser != null && ftUser.FTRole == FTMRole.FTGuest)
                        {
                            ftUser.FTRole = FTMRole.FTMember;
                            _fTUserRepository.Update(ftUser);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Người được mời hoặc thành viên trong cây gia phả không tồn tại.");
                }
            }

            // Send invitation response to inviter
            var invitationResponse = accepted ? "chấp nhận" : "từ chối";
            var notification = new FTNotification
            {
                UserId = invitation.InviterUserId,
                Title = $"Phản hồi lời mời từ {invitation.InvitedName}",
                Message = $"<p><b>{invitation.InvitedName}</b> đã <b>{invitationResponse}</b> lời mời của bạn tham gia gia phả <em>{invitation.FTName}</em>.</p>",
                Type = NotificationType.INVITE,
                IsRead = false,
                IsActionable = false,
                RelatedId = invitation.Id,
            };
            await _fTNotificationRepository.AddAsync(notification);

            var fTNotificationDto = _mapper.Map<FTNotificationDto>(notification);

            await _hubContext.Clients.User(invitation.InviterUserId.ToString())
                .SendAsync("ReceiveNotification", fTNotificationDto);

            await _unitOfWork.CompleteAsync();
        }

        public async Task SendAsync(FTInvitation invitation)
        {
            // Send By Email
                string beURL = Environment.GetEnvironmentVariable("BE_URL");
            string acceptUrl = $"{beURL}/api/invitation/respond?relatedId={invitation.Id}&accepted=true";
            string rejectUrl = $"{beURL}/api/invitation/respond?relatedId={invitation.Id}&accepted=false";

            string body = $@"
            <div style='font-family: Arial; color:#333'>
                <h2>Lời mời liên kết thành viên trong cây gia phả</h2>
                <p><b>{_currentUserResolver.Name}</b> đã mời bạn tham gia gia phả <b>Dòng họ {invitation.FTName}</b> với thành viên <b>{invitation.FTMemberName}</b>.</p>
                <p>Bạn có muốn chấp nhận lời mời này không?</p>
                <div style='margin-top:20px'>
                    <a href='{acceptUrl}' 
                       style='background-color:#4CAF50;color:white;padding:10px 15px;text-decoration:none;border-radius:5px;margin-right:10px;'>
                        ✅ Chấp nhận
                    </a>
                    <a href='{rejectUrl}' 
                       style='background-color:#f44336;color:white;padding:10px 15px;text-decoration:none;border-radius:5px;'>
                        ❌ Từ chối
                    </a>
                </div>
                <p style='margin-top:30px;color:#888'>Trân trọng,<br>Đội ngũ Gia Phả Trực Tuyến</p>
            </div>";

            await _emailSender.SendEmailAsync(invitation.Email, "Lời mời tham gia gia phả", body);

            //Send By Signal R
            var notification = new FTNotification
            {
                UserId = invitation.InvitedUserId,
                Title = "Bạn đã được mời",
                Message = $"<p><b>{invitation.InviterName}</b> mời bạn liên kết với <b>{invitation.FTMemberName}</b> trong cây <em>{invitation.FTName}</em>.</p>",
                Type = NotificationType.INVITE,
                IsRead = false,
                IsActionable = true,
                RelatedId = invitation.Id,
            };
            await _fTNotificationRepository.AddAsync(notification); 
            var fTNotificationDto = _mapper.Map<FTNotificationDto>(notification);
            await _hubContext.Clients.User(notification.UserId.ToString())
            .SendAsync("ReceiveNotification", fTNotificationDto);

            await _unitOfWork.CompleteAsync();
        }
    }
}
