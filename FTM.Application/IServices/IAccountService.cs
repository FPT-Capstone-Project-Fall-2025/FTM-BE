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
        //Task<SendOTPTracking> Register(RegisterAccountRequest request);
        Task RegisterByEmail(RegisterAccountRequest request);
        Task<bool> ConfirmEmail(Guid userId, string token);
        Task Logout(string accessToken);
        Task ForgotPasswordAsync(ForgotPasswordRequest request);
        Task ResetPasswordAsync(ResetPasswordRequest request);
    }
}
