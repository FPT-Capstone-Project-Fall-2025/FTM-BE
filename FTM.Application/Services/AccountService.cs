using FTM.Application.IServices;
using FTM.Domain.Constants;
using FTM.Domain.Entities.Identity;
using FTM.Domain.Enums;
using FTM.Domain.Models;
using FTM.Domain.Models.Authen;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XAct;
using static FTM.Domain.Constants.Constants;

namespace FTM.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ICurrentUserResolver _currentUserResolver;
        private readonly ISendOTPTrackingRepository _sendOTPTrackingRepository;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppIdentityDbContext _context;
        private readonly ITokenProvider _tokenProvider;
        private const int MinTimeSendOTP = 2;
        private const int MaxSendOTPPerTenM = 4;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ICurrentUserResolver currentUserResolver,
            IUnitOfWork unitOfWork,
            IEmailSender emailSender,
            ISendOTPTrackingRepository sendOTPTrackingRepository,
            ITokenProvider tokenProvider,
            AppIdentityDbContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenProvider = tokenProvider;
            _currentUserResolver = currentUserResolver;
            _emailSender = emailSender;
            _sendOTPTrackingRepository = sendOTPTrackingRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<TokenResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null && !user.EmailConfirmed)
                return new TokenResult()
                {
                    AccountStatus = AccountStatus.DoNotConfirmedEmail
                };

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(username);
                if (user == null) throw new ArgumentException("Đăng nhập không thành công. Vui lòng kiểm tra lại email, số điện thoại và mật khẩu.");
                if (!user.PhoneNumberConfirmed)
                    return new TokenResult()
                    {
                        AccountStatus = AccountStatus.DoNotConfirmedPhoneNumberRequired
                    };

                if (!user.EmailConfirmed)
                    return new TokenResult()
                    {
                        AccountStatus = AccountStatus.DoNotConfirmedEmail
                    };
            }

            if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
                throw new ArgumentException("Tài khoản của bạn đang tạm khóa.");

            if (await _userManager.CheckPasswordAsync(user, password))
            {
                if (_userManager.SupportsUserLockout && await _userManager.GetAccessFailedCountAsync(user) > 0)
                {
                    await _userManager.ResetAccessFailedCountAsync(user);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);

                if (result.Succeeded)
                {
                    var applicationUser = _userManager.Users.SingleOrDefault(r => r.UserName == user.UserName);

                    var claims = new List<Claim>
                    {
                        new(JwtRegisteredClaimNames.Sub, applicationUser.Id.ToString()),
                        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new(ClaimTypes.NameIdentifier, applicationUser.Id.ToString()),
                        new(JwtRegisteredClaimNames.Email, applicationUser.Email),
                        new(ClaimTypes.Name, applicationUser.UserName),
                        new(CustomJwtClaimTypes.Name, applicationUser.UserName),
                        new(CustomJwtClaimTypes.EmailConfirmed, applicationUser.EmailConfirmed.ToString()),
                        new(CustomJwtClaimTypes.PhoneNumberConfirmed, applicationUser.PhoneNumberConfirmed.ToString()),
                        new(CustomJwtClaimTypes.FullName, user.Name),
                    };
                    
                    var roles = await _userManager.GetRolesAsync(applicationUser);

                    if (roles.Count > 0)
                    {
                        roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));
                    }
                
                    var accessToken = _tokenProvider.GenerateJwtToken(claims);
                    var newRefreshToken = _tokenProvider.GenerateRefreshToken();
                    var userRefreshToken = _context.UserRefreshTokens.FirstOrDefault(urt => urt.ApplicationUserId == applicationUser.Id);

                    if (userRefreshToken == null)
                    {
                        userRefreshToken = new ApplicationUserRefreshToken
                        {
                            ApplicationUserId = user.Id,
                            Token = newRefreshToken,
                        };

                        _context.UserRefreshTokens.Add(userRefreshToken);
                    }
                    else
                    {
                        userRefreshToken.Token = newRefreshToken;
                        _context.Update(userRefreshToken);
                    }

                    if (!applicationUser.IsActive)
                    {
                        applicationUser.IsActive = true;
                        _context.Update(applicationUser);
                    }

                    await _context.SaveChangesAsync();

                    var tokenResult = new TokenResult
                    {
                        UserId = applicationUser.Id,
                        Username = applicationUser.UserName,
                        Email = applicationUser.Email,
                        Phone = applicationUser.PhoneNumber,
                        AccessToken = accessToken,
                        RefreshToken = userRefreshToken.Token,
                        Roles = roles,
                        AccountStatus = AccountStatus.Activated,
                        Picture = user.Picture,
                        Fullname = user.Name,
                    };
                    return tokenResult;
                }

                if (result.IsNotAllowed)
                {
                    var applicationUser = _userManager.Users.SingleOrDefault(r => r.UserName == username);

                    if (!applicationUser.EmailConfirmed)
                    {
                        throw new ArgumentException("Tài khoản chưa được xác nhận email. Vui lòng kiểm tra hộp thư đến và xác nhận email.");
                    }

                    throw new ArgumentException("Đăng nhập không thành công. Vui lòng kiểm tra lại email và mật khẩu.");
                }

                throw new ArgumentException("Đăng nhập không thành công. Vui lòng kiểm tra lại email và mật khẩu.");
            }
            else
            {
                if (_userManager.SupportsUserLockout && await _userManager.GetLockoutEnabledAsync(user))
                {
                    await _userManager.AccessFailedAsync(user);
                }

                if (_userManager.SupportsUserLockout && await _userManager.GetAccessFailedCountAsync(user) == Constants.MAXIMUM_LOGIN_FAIL_NUMBER - 1)
                    return new TokenResult()
                    {
                        AccountStatus = AccountStatus.MaximumFail
                    };
                else throw new ArgumentException("Đăng nhập không thành công. Vui lòng kiểm tra lại email và mật khẩu.");
            }
        }
        public async Task RegisterByEmail(RegisterAccountRequest request)
        {
            // Check if email already exists
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                if (user.EmailConfirmed)
                    throw new ArgumentException("Email đã được đăng ký tài khoản trước đó. Vui lòng đăng nhập.");

                // Optional: remove old unconfirmed account
                await _userManager.DeleteAsync(user);
            }

            // Check if phone number already exists
            user = await _userManager.FindByNameAsync(request.PhoneNumber);
            if (user != null)
            {
                throw new ArgumentException("Số điện thoại đã được đăng ký. Vui lòng đăng nhập.");
            }

            // Create new user
            user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Name = request.Name,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                throw new ArgumentException(string.Join(";", result.Errors.Select(m => m.Description)));

            // Assign default role "User"
            await _userManager.AddToRoleAsync(user, "User");

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string apiURL = "https://localhost:5001";
            var confirmationLink = $"{apiURL}/api/account/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            // Send email
            await _emailSender.SendEmailAsync(
                request.Email,
                "Xác nhận tài khoản của bạn",
                $"Xin chào {request.Name},<br/>" +
                $"Vui lòng nhấn vào liên kết để xác nhận tài khoản: <a href='{confirmationLink}'>Xác nhận Email</a>"
            );

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ConfirmEmail(Guid userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
        }
    }
}
