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
    }
}
