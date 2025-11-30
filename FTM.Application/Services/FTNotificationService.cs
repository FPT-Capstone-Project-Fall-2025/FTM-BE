using AutoMapper;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.DTOs.Notifications;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace FTM.Application.Services
{
    public class FTNotificationService : IFTNotificationService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserResolver _currentUserResolver;
        private readonly IFTNotificationRepository _fTNotificationRepository;
        public FTNotificationService(IMapper mapper, IUnitOfWork unitOfWork, ICurrentUserResolver currentUserResolver, IFTNotificationRepository fTNotificationRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _currentUserResolver = currentUserResolver;
            _fTNotificationRepository = fTNotificationRepository;
        }

        public async Task<IReadOnlyList<FTNotificationDto>> FindByuserIdAsync()
        {
            var userId = _currentUserResolver.UserId;
            if (userId == null) throw new ArgumentNullException("Người dùng chưa đăng nhập vào hệ thống");
            var notis = await _fTNotificationRepository.FindByuserIdAsync(userId);
            return _mapper.Map<IReadOnlyList<FTNotificationDto>>(notis);
        }

        public async Task DeleteAsync(Guid relatedId)
        {
            var invitationNotification = await _fTNotificationRepository.FindByInvitationIdAsync(relatedId);
            if (invitationNotification != null && invitationNotification.IsDeleted == false)
            {
                invitationNotification.IsRead = true;
                invitationNotification.IsDeleted = true;
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                new ArgumentNullException("Thông báo đã được xóa trước đây");
            }
        }
    }
}
