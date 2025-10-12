using FTM.Application.IServices;
using FTM.Domain.Constants;
using FTM.Domain.Entities.Identity;
using FTM.Domain.Enums;
using FTM.Domain.Models;
using FTM.Domain.DTOs.Authen;
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
using XAct.Security;
using XAct.Users;
using static FTM.Domain.Constants.Constants;

namespace FTM.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ICurrentUserResolver _currentUserResolver;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppIdentityDbContext _context;
        private readonly ITokenProvider _tokenProvider;
        private readonly IBlobStorageService _blobStorageService;


        public AccountService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ICurrentUserResolver currentUserResolver,
            IUnitOfWork unitOfWork,
            IEmailSender emailSender,
            ITokenProvider tokenProvider,
            AppIdentityDbContext context,
            IBlobStorageService blobStorageService
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenProvider = tokenProvider;
            _currentUserResolver = currentUserResolver;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
            _context = context;
            _blobStorageService = blobStorageService;
        }

        public async Task<TokenResult> Login(string username, string password)
        {
            var user = await _userManager.FindByEmailAsync(username);

            if (user == null)
                throw new ArgumentException("Đăng nhập không thành công. Vui lòng kiểm tra lại email và mật khẩu.");

            if (!user.EmailConfirmed)
            {
                throw new ArgumentException("Đăng nhập không thành công. Vui lòng xác nhận email.");
                //return new TokenResult
                //{
                //    AccountStatus = AccountStatus.DoNotConfirmedEmail
                //};
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
                    new(CustomJwtClaimTypes.IsGoogleLogin, user.IsGoogleLogin? "true":"false"),
                    new(CustomJwtClaimTypes.PhoneNumberConfirmed, user.PhoneNumberConfirmed.ToString()),
                    new(CustomJwtClaimTypes.FullName, user.Name ?? string.Empty)
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
                    //UserId = user.Id,
                    //Username = user.UserName,
                    //Email = user.Email,
                    //Phone = user.PhoneNumber,
                    AccessToken = accessToken,
                    RefreshToken = userRefreshToken.Token,
                    //Roles = roles,
                    //AccountStatus = AccountStatus.Activated,
                    //Picture = user.Picture,
                    //Fullname = user.Name,
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

        public async Task<TokenResult> LoginWithGoogle(string fullName, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            TokenResult tokenResult;
            if (user is null)
            {
                // Create new user
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    Name = fullName,
                    EmailConfirmed = true,
                    IsGoogleLogin = true,
                    PhoneNumberConfirmed = false,
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                    throw new ArgumentException(string.Join(";", result.Errors.Select(m => m.Description)));

                // Assign default role "User"
                await _userManager.AddToRoleAsync(user, "User");
            }
            else if(user is not null && !user.EmailConfirmed)
            {
                user.EmailConfirmed = true;    
            }

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
                    new(CustomJwtClaimTypes.EmailConfirmed, user.EmailConfirmed.ToString()),
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
                //UserId = user.Id,
                //Username = user.UserName,
                //Email = user.Email,
                //Phone = user.PhoneNumber,
                AccessToken = accessToken,
                RefreshToken = userRefreshToken.Token,
                //Roles = roles,
                //AccountStatus = AccountStatus.Activated,
                //Picture = user.Picture,
                //Fullname = user.Name,
            };
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

            string beURL = Environment.GetEnvironmentVariable("BE_URL");
            var confirmationLink = $"{beURL}/api/account/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
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
            var decodedToken = Uri.UnescapeDataString(token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            return result.Succeeded;
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
            catch (Exception ex)
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

            string feURL = Environment.GetEnvironmentVariable("FE_URL");
            var callbackUrl = $"{feURL}/reset-password?userId={user.Id}&&code={code}";
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

        public async Task<UserProfileResponse> GetUserProfileAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.MProvince)
                .Include(u => u.MWard)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new ArgumentException("Không tìm thấy người dùng.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new UserProfileResponse
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Name = user.Name,
                Address = user.Address,
                Nickname = user.Nickname,
                Birthday = user.Birthday,
                Job = user.Job,
                Gender = user.Gender,
                Picture = user.Picture,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Province = user.MProvince != null ? new ProvinceInfo
                {
                    Id = user.MProvince.Id,
                    Code = user.MProvince.Code,
                    Name = user.MProvince.Name,
                    NameWithType = user.MProvince.NameWithType
                } : null,
                Ward = user.MWard != null ? new WardInfo
                {
                    Id = user.MWard.Id,
                    Code = user.MWard.Code,
                    Name = user.MWard.Name,
                    NameWithType = user.MWard.NameWithType,
                    Path = user.MWard.Path,
                    PathWithType = user.MWard.PathWithType
                } : null,
                Roles = roles.ToList(),
                CreatedDate = user.CreatedDate,
                UpdatedDate = user.UpdatedDate
            };
        }

        public async Task<UserProfileResponse> GetCurrentUserProfileAsync()
        {
            var currentUserId = _currentUserResolver.UserId;

            if (currentUserId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("Vui lòng đăng nhập để xem thông tin cá nhân.");
            }

            return await GetUserProfileAsync(currentUserId);
        }

        public async Task<UserProfileResponse> UpdateCurrentUserProfileAsync(UpdateUserProfileRequest request)
        {
            var currentUserId = _currentUserResolver.UserId;

            if (currentUserId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("Vui lòng đăng nhập để cập nhật thông tin cá nhân.");
            }

            var user = await _userManager.Users
                .Include(u => u.MProvince)
                .Include(u => u.MWard)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (user == null)
            {
                throw new ArgumentException("Không tìm thấy thông tin người dùng.");
            }

            // Validate Province and Ward if provided
            if (request.ProvinceId.HasValue)
            {
                var provinceExists = await _context.Mprovinces
                    .AnyAsync(p => p.Id == request.ProvinceId.Value);
                if (!provinceExists)
                {
                    throw new ArgumentException("Tỉnh/Thành phố không hợp lệ.");
                }
            }

            if (request.WardId.HasValue)
            {
                var wardExists = await _context.MWards
                    .AnyAsync(w => w.Id == request.WardId.Value);
                if (!wardExists)
                {
                    throw new ArgumentException("Phường/Xã không hợp lệ.");
                }
            }

            // Update user properties
            if (!string.IsNullOrEmpty(request.Name))
                user.Name = request.Name;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            if (!string.IsNullOrEmpty(request.Address))
                user.Address = request.Address;

            if (!string.IsNullOrEmpty(request.Nickname))
                user.Nickname = request.Nickname;

            if (request.Birthday.HasValue)
                user.Birthday = request.Birthday.Value;

            if (!string.IsNullOrEmpty(request.Job))
                user.Job = request.Job;

            if (request.Gender.HasValue)
                user.Gender = request.Gender.Value;

            if (request.ProvinceId.HasValue)
                user.ProvinceId = request.ProvinceId.Value;

            if (request.WardId.HasValue)
                user.WardId = request.WardId.Value;

            user.UpdatedDate = DateTimeOffset.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Không thể cập nhật thông tin người dùng: {errors}");
            }

            // Return updated profile
            return await GetCurrentUserProfileAsync();
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var currentUserId = _currentUserResolver.UserId;

            if (currentUserId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("Vui lòng đăng nhập để đổi mật khẩu.");
            }

            var user = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (user == null)
            {
                throw new ArgumentException("Không tìm thấy thông tin người dùng.");
            }

            // Verify current password
            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!isCurrentPasswordValid)
            {
                throw new ArgumentException("Mật khẩu hiện tại không đúng.");
            }

            // Change password
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Không thể đổi mật khẩu: {errors}");
            }

            return true;
        }

        public async Task<List<ProvinceListResponse>> GetProvincesAsync()
        {
            var provinces = await _context.Mprovinces
                .Where(p => p.IsDeleted != true)
                .OrderBy(p => p.Name)
                .Select(p => new ProvinceListResponse
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name,
                    NameWithType = p.NameWithType,
                    Slug = p.Slug
                })
                .ToListAsync();

            return provinces;
        }

        public async Task<List<WardListResponse>> GetWardsByProvinceAsync(Guid provinceId)
        {
            var province = await _context.Mprovinces
                .Where(p => p.Id == provinceId && p.IsDeleted != true)
                .FirstOrDefaultAsync();

            if (province == null)
            {
                return new List<WardListResponse>();
            }

           
            var wards = await _context.MWards
                .Where(w => w.IsDeleted != true && 
                           w.Path != null && 
                           (w.Path.Contains(province.Name) || 
                            w.Path.Contains(province.NameWithType) ||
                            w.PathWithType != null && w.PathWithType.Contains(province.Name) ||
                            w.PathWithType != null && w.PathWithType.Contains(province.NameWithType)))
                .OrderBy(w => w.Name)
                .Select(w => new WardListResponse
                {
                    Id = w.Id,
                    Code = w.Code,
                    Name = w.Name,
                    NameWithType = w.NameWithType,
                    Path = w.Path,
                    PathWithType = w.PathWithType,
                    Slug = w.Slug
                })
                .ToListAsync();

            return wards;
        }

        public async Task<UpdateAvatarResponse> UpdateCurrentUserAvatarAsync(UpdateAvatarRequest request)
        {
            var currentUserId = _currentUserResolver.UserId;

            if (currentUserId == Guid.Empty)
            {
                throw new UnauthorizedAccessException("Vui lòng đăng nhập để cập nhật avatar.");
            }

            var user = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (user == null)
            {
                throw new ArgumentException("Không tìm thấy thông tin người dùng.");
            }

            try
            {
                // Delete old avatar if exists
                if (!string.IsNullOrEmpty(user.Picture))
                {
                    try
                    {
                        var oldFileName = Path.GetFileName(new Uri(user.Picture).LocalPath);
                        await _blobStorageService.DeleteFileAsync("avatars", oldFileName);
                    }
                    catch
                    {
                        // Ignore delete errors for old avatar
                    }
                }

                // Upload new avatar
                var avatarUrl = await _blobStorageService.UploadFileAsync(
                    request.Avatar,
                    "avatars",
                    $"avatar_{currentUserId}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(request.Avatar.FileName)}"
                );

                // Update user record
                user.Picture = avatarUrl;
                user.UpdatedDate = DateTimeOffset.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Không thể cập nhật avatar: {errors}");
                }

                return new UpdateAvatarResponse
                {
                    AvatarUrl = avatarUrl,
                    Message = "Cập nhật avatar thành công!"
                };
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException($"Lỗi upload file: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Đã xảy ra lỗi khi cập nhật avatar: {ex.Message}");
            }
        }

        public async Task<TokenResult> RefreshToken(string accessToken, string refreshToken)
        {
            var principal = _tokenProvider.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var applicationUser = _userManager.Users.SingleOrDefault(r => r.UserName == username);
            var userRefreshToken = _context.UserRefreshTokens.SingleOrDefault(u => u.ApplicationUserId == applicationUser.Id);

            if (userRefreshToken == null || userRefreshToken.Token != refreshToken)
            {
                throw new ArgumentException("Vui lòng đăng nhập lại.");
            }

            var newJwtToken = _tokenProvider.GenerateJwtToken(principal.Claims);
            var newRefreshToken = _tokenProvider.GenerateRefreshToken();
            userRefreshToken.Token = newRefreshToken;
            _context.Update(userRefreshToken);

            await _context.SaveChangesAsync();
            var roles = await _userManager.GetRolesAsync(applicationUser);

            return new TokenResult
            {
                //UserId = applicationUser.Id,
                //Username = applicationUser.UserName,
                //Email = applicationUser.Email,
                //Phone = applicationUser.PhoneNumber,
                //AccountStatus = AccountStatus.Activated,
                //Picture = applicationUser.Picture,
                //Fullname = applicationUser.Name,
                AccessToken = newJwtToken,
                RefreshToken = newRefreshToken,
                //Roles = roles,
            };
        }
    }
}
