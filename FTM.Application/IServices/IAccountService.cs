using FTM.Domain.Models;
using FTM.Domain.DTOs.Authen;
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
        Task<TokenResult> LoginWithGoogle(string fullName, string email);
        Task<TokenResult> RefreshToken(string accessToken, string refreshToken);
        Task RegisterByEmail(RegisterAccountRequest request);
        Task<bool> ConfirmEmail(Guid userId, string token);
        Task Logout(string accessToken);
        Task ForgotPasswordAsync(ForgotPasswordRequest request);
        Task ResetPasswordAsync(ResetPasswordRequest request);

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
