using FTM.Domain.Models;
using FTM.Domain.Models.Authen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Application.IServices
{
    public interface IAccountService
    {
        Task<TokenResult> Login(string username, string password);
        Task<SendOTPTracking> Register(RegisterAccountRequest request);
        Task<SendOTPTracking> ValidateSendOTPLimit(SendOtpRequest request);
        Task<string> GenerateTwoFactorTokenConfirmOTP(string username, string tokenOptions);
        Task<SendOTPTracking> SendSMSOtp(SendOtpRequest request);
        
        // Profile methods
        Task<UserProfileResponse> GetUserProfileAsync(Guid userId);
        Task<UserProfileResponse> GetCurrentUserProfileAsync();
        Task<UserProfileResponse> UpdateCurrentUserProfileAsync(UpdateUserProfileRequest request);
        Task<UpdateAvatarResponse> UpdateCurrentUserAvatarAsync(UpdateAvatarRequest request);
        Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
        
        // Location methods
        Task<List<ProvinceListResponse>> GetProvincesAsync();
        Task<List<WardListResponse>> GetWardsByProvinceAsync(Guid provinceId);
    }
}
