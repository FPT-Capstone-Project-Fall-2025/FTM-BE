using FTM.Application.IServices;
using FTM.Domain.Entities.FamilyTree;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly ICurrentUserResolver _currentUserResolver;
        private readonly IFTInvitationRepository _fTInvitationRepository;
        public FTInvitationService(IEmailSender emailSender, ICurrentUserResolver currentUserResolver, IFTInvitationRepository fTInvitationRepository)
        {
            _emailSender = emailSender;
            _currentUserResolver = currentUserResolver;
            _fTInvitationRepository = fTInvitationRepository;
        }

        public async Task AddAsync(FTInvitation invitation)
        {
            await _fTInvitationRepository.AddAsync(invitation);
        }

        public async Task SendAsync(FTInvitation invitation)
        {
            string beURL = Environment.GetEnvironmentVariable("BE_URL");
            string acceptUrl = $"{beURL}/api/invitation/respond?token={invitation.Token}&accepted=true";
            string rejectUrl = $"{beURL}/api/invitation/respond?token={invitation.Token}&accepted=false";

            string body = $@"
            <div style='font-family: Arial; color:#333'>
                <h2>Lời mời liên kết thành viên trong cây gia phả</h2>
                <p><b>{_currentUserResolver.Name}</b> đã mời bạn liên kết với một thành viên trong cây gia phả.</p>
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
        }


    }
}
