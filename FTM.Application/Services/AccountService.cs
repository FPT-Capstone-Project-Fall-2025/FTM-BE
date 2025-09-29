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
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using XAct;
using XAct.Messages;
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

            if (user == null)
                throw new ArgumentException("Đăng nhập không thành công. Vui lòng kiểm tra lại email và mật khẩu.");

            if (!user.EmailConfirmed)
            {
                return new TokenResult
                {
                    AccountStatus = AccountStatus.DoNotConfirmedEmail
                };
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, password, false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var claims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                    new(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new(CustomJwtClaimTypes.Name, user.UserName ?? string.Empty),
                    new(CustomJwtClaimTypes.EmailConfirmed, user.EmailConfirmed.ToString()),
                    new(CustomJwtClaimTypes.PhoneNumberConfirmed, user.PhoneNumberConfirmed.ToString()),
                    new(CustomJwtClaimTypes.FullName, user.Name ?? string.Empty),
                };

                var roles = await _userManager.GetRolesAsync(user);
                if (roles?.Count > 0)
                {
                    foreach (var role in roles)
                        claims.Add(new Claim(ClaimTypes.Role, role));
                }

                // generate tokens
                var accessToken = _tokenProvider.GenerateJwtToken(claims);
                var newRefreshToken = _tokenProvider.GenerateRefreshToken();

                var userRefreshToken = await _context.UserRefreshTokens
                    .FirstOrDefaultAsync(urt => urt.ApplicationUserId == user.Id);

                if (userRefreshToken == null)
                {
                    userRefreshToken = new ApplicationUserRefreshToken
                    {
                        ApplicationUserId = user.Id,
                        Token = newRefreshToken,
                        ExpiredAt = DateTime.UtcNow.AddDays(7),
                        LastModifiedBy = "System",
                        CreatedBy = "System",
                        CreatedByUserId = Guid.NewGuid()
                    };

                    await _context.UserRefreshTokens.AddAsync(userRefreshToken);
                }
                else
                {
                    userRefreshToken.Token = newRefreshToken;
                    userRefreshToken.ExpiredAt = DateTime.UtcNow.AddDays(7);
                    _context.UserRefreshTokens.Update(userRefreshToken);
                }

                if (!user.IsActive)
                {
                    user.IsActive = true;
                    _context.Update(user);
                }

                await _context.SaveChangesAsync();

                return new TokenResult
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    AccessToken = accessToken,
                    RefreshToken = userRefreshToken.Token,
                    Roles = roles,
                    AccountStatus = AccountStatus.Activated,
                    Picture = user.Picture,
                    Fullname = user.Name,
                };
            }

            if (result.IsLockedOut)
            {
                throw new ArgumentException("Tài khoản của bạn đang tạm khóa. Vui lòng thử lại sau 1 phút.");
            }

            if (result.IsNotAllowed)
            {
                throw new ArgumentException("Tài khoản chưa được xác nhận email. Vui lòng kiểm tra hộp thư đến và xác nhận email.");
            }

            throw new ArgumentException("Đăng nhập không thành công. Vui lòng kiểm tra lại email và mật khẩu.");
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
            //var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

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

        public async Task Logout(string accessToken)
        {
            try
            {
                var principal = _tokenProvider.GetPrincipalFromExpiredToken(accessToken);
                var username = principal.Identity.Name;
                var applicationUser = _userManager.Users.SingleOrDefault(r => r.UserName == username);
                var userRefreshToken = _context.UserRefreshTokens.SingleOrDefault(u => u.ApplicationUserId == applicationUser.Id);

                if (userRefreshToken != null)
                {
                    userRefreshToken.Token = null;
                    await _context.SaveChangesAsync();
                }

                await _signInManager.SignOutAsync();

                return;
            }
            catch(Exception ex)
            {
                throw new ArgumentException("Token Invalid");
            }
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new ArgumentException("Account not found.");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            
            string apiURL = "https://localhost:5001";
            var callbackUrl = $"{apiURL}/api/account/forgot-password?userId={user.Id}&&code={code}";
            var body = "<b>Yêu cầu khôi phục lại mật khẩu</b></br><p>Chào <b>{0}!</b></p></br><p>Bạn đã yêu cầu khôi phục mật khẩu đăng nhập thành công. Vui lòng bấm vào đường dẫn bên dưới đây để khôi phục lại mật khẩu tài khoản của bạn tại GP Application:</p></br><a href=\"{1}\"> Link khôi phục mật khẩu</a></br><p>Nếu bạn không yêu cầu khôi phục mật khẩu, vui lòng bỏ qua.</p></br><p>Chân thành cảm ơn,</p><p>GP application</p>";
            var mailBody = string.Format(body, user.Name, HtmlEncoder.Default.Encode(callbackUrl));
            await _emailSender.SendEmailAsync(user.Email, "Xác nhận đặt lại mật khẩu", mailBody);
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

            var result = await _userManager.ResetPasswordAsync(user, code, request.Password);

            if (result.Succeeded)
            {
                return;
            }

            throw new ArgumentException("Reset password fail.");
        }
    }
}
