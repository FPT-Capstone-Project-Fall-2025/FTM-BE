using FTM.Application.IServices;
using FTM.Domain.Constants;
using FTM.Domain.Entities.Identity;
using FTM.Domain.Enums;
using FTM.Domain.Models;
using FTM.Domain.Models.Authen;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
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
        private readonly ISendSMSService _sendSMSService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppIdentityDbContext _context;
        private readonly ITokenProvider _tokenProvider;
        private readonly IBlobStorageService _blobStorageService;
        private const int MinTimeSendOTP = 2;
        private const int MaxSendOTPPerTenM = 4;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ICurrentUserResolver currentUserResolver,
            ISendSMSService sendSMSService,
            IUnitOfWork unitOfWork,
            ISendOTPTrackingRepository sendOTPTrackingRepository,
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
            _sendSMSService = sendSMSService;
            _sendOTPTrackingRepository = sendOTPTrackingRepository;
            _unitOfWork = unitOfWork;
            _context = context;
            _blobStorageService = blobStorageService;
        }

        public async Task<TokenResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null && !user.PhoneNumberConfirmed)
                return new TokenResult()
                {
                    AccountStatus = AccountStatus.DoNotConfirmPhoneNumber
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

        public async Task<SendOTPTracking> Register(RegisterAccountRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.PhoneNumber);

            if (user != null)
            {
                if (user.PhoneNumberConfirmed) throw new ArgumentException("Số điện thoại đã được đăng ký tài khoản trước đó. Vui lòng sử dụng chức năng đăng nhập.");
                await _userManager.DeleteAsync(user);
            }

            user = await _userManager.FindByEmailAsync(request.Email);

            if (user != null)
            {
                throw new ArgumentException("Email đã được đăng ký tài khoản trước đó. Vui lòng sử dụng chức năng đăng nhập.");
                //await _userManager.DeleteAsync(user);
            }

            user = new ApplicationUser
            {
                UserName = request.PhoneNumber,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Name = request.Name,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                var otpRequest = new SendOtpRequest()
                {
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    RemoteIpAddress = _currentUserResolver.RemoteIpAddress,
                };

                var otpTracking = await ValidateSendOTPLimit(otpRequest);

                if (otpTracking != null)
                {
                    return otpTracking;
                }

                var code = await GenerateTwoFactorTokenConfirmOTP(request.PhoneNumber, TokenOptions.DefaultPhoneProvider);

                otpTracking = await SendSMSOtp(new SendOtpRequest()
                {
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Content = code,
                    RemoteIpAddress = _currentUserResolver.RemoteIpAddress
                });

                //var refreshUser = _context.UserRefreshTokens.SingleOrDefault(u => u.ApplicationUserId == user.Id);

                //if (refreshUser != null)
                //{
                //    _context.UserRefreshTokens.Remove(refreshUser);
                //}

                //refreshUser = new ApplicationUserRefreshToken
                //{
                //    ApplicationUserId = user.Id
                //};

                //_context.UserRefreshTokens.Add(refreshUser);
                await _context.SaveChangesAsync();

                return otpTracking;
            }

            throw new ArgumentException(string.Join(";", result.Errors.Select(m => m.Description)));
        }

        public async Task<SendOTPTracking> ValidateSendOTPLimit(SendOtpRequest request)
        {
            var otpTracking = await _sendOTPTrackingRepository.GetSendOTPTrackingAsync(request.RemoteIpAddress, request.Email, request.PhoneNumber);

            var lastSentOTP = otpTracking.OrderByDescending(x => x.LastModifiedOn).FirstOrDefault();

            if (otpTracking.Count >= MaxSendOTPPerTenM || (lastSentOTP?.LastModifiedOn != null && lastSentOTP.LastModifiedOn.AddMinutes(MinTimeSendOTP) > DateTime.UtcNow))
            {
                return lastSentOTP;
            }

            return null;
        }

        public async Task<string> GenerateTwoFactorTokenConfirmOTP(string providerValue, string tokenOptions)
        {
            ApplicationUser currentUser = null;
            if (tokenOptions == TokenOptions.DefaultEmailProvider)
            {
                currentUser = await _userManager.FindByEmailAsync(providerValue);
            }

            if (tokenOptions == TokenOptions.DefaultPhoneProvider)
            {
                currentUser = await _userManager.FindByNameAsync(providerValue);
            }

            if (currentUser == null)
            {
                throw new ArgumentException("Không tìm thấy tài khoản.");
            }

            var code = await _userManager.GenerateTwoFactorTokenAsync(currentUser, tokenOptions);

            return code;
        }

        public async Task<SendOTPTracking> SendSMSOtp(SendOtpRequest request)
        {
            _sendSMSService.SendSMS(request.PhoneNumber, request.Content);

            var otpTracking = new SendOTPTracking()
            {
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RemoteIpAddress = request.RemoteIpAddress,
            };

            await _sendOTPTrackingRepository.AddAsync(otpTracking);
            await _unitOfWork.CompleteAsync();

            return otpTracking;
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
                UserName = user.UserName,
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
                    ProvinceId = user.MProvince.Id,
                    Code = user.MProvince.Code,
                    Name = user.MProvince.Name,
                    NameWithType = user.MProvince.NameWithType
                } : null,
                Ward = user.MWard != null ? new WardInfo
                {
                    WardId = user.MWard.Id,
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
            var wards = await _context.MWards
                .Where(w => w.IsDeleted != true && w.Path != null && w.Path.Contains(provinceId.ToString()))
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

    }
}
