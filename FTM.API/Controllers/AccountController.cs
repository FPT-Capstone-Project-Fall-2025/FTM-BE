using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.Models;
using FTM.Domain.Models.Authen;
using FTM.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }

            var result = await _accountService.Login(model.UserName, model.Password);
            return Ok(new ApiSuccess(result));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }

            var result = await _accountService.Register(request);
            return Ok(new ApiSuccess(result));
        }

        [HttpGet("profile")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var profile = await _accountService.GetCurrentUserProfileAsync();
                return Ok(new ApiSuccess(profile));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse.Fail(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResponse.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail("Đã xảy ra lỗi khi lấy thông tin cá nhân."));
            }
        }

        [HttpGet("profile/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            try
            {
                var profile = await _accountService.GetUserProfileAsync(userId);
                return Ok(new ApiSuccess(profile));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResponse.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail("Đã xảy ra lỗi khi lấy thông tin người dùng."));
            }
        }

        [HttpPut("profile")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ThrowModelErrors();
                }

                var updatedProfile = await _accountService.UpdateCurrentUserProfileAsync(request);
                return Ok(new ApiSuccess(updatedProfile));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse.Fail(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail("Đã xảy ra lỗi khi cập nhật thông tin cá nhân."));
            }
        }

        [HttpGet("provinces")]
        [Authorize]
        [ProducesResponseType(typeof(List<ProvinceListResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProvinces()
        {
            try
            {
                var provinces = await _accountService.GetProvincesAsync();
                return Ok(new ApiSuccess(provinces));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail("Đã xảy ra lỗi khi lấy danh sách tỉnh/thành phố."));
            }
        }

        [HttpGet("provinces/{provinceId}/wards")]
        [Authorize]
        [ProducesResponseType(typeof(List<WardListResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWardsByProvince(Guid provinceId)
        {
            try
            {
                var wards = await _accountService.GetWardsByProvinceAsync(provinceId);
                return Ok(new ApiSuccess(wards));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail("Đã xảy ra lỗi khi lấy danh sách phường/xã."));
            }
        }

        [HttpPut("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ThrowModelErrors();
                }

                var result = await _accountService.ChangePasswordAsync(request);
                return Ok(new ApiSuccess("Đổi mật khẩu thành công!", result));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse.Fail(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail("Đã xảy ra lỗi khi đổi mật khẩu."));
            }
        }

        [HttpPost("upload-avatar")]
        [Authorize]
        [ProducesResponseType(typeof(UpdateAvatarResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadAvatar([FromForm] UpdateAvatarRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ThrowModelErrors();
                }

                var result = await _accountService.UpdateCurrentUserAvatarAsync(request);
                return Ok(new ApiSuccess("Cập nhật avatar thành công!", result));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse.Fail(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail("Đã xảy ra lỗi khi cập nhật avatar."));
            }
        }

        private void ThrowModelErrors()
        {
            var message = string.Join(" | ", ModelState.Values
                                                        .SelectMany(v => v.Errors)
                                                        .Select(e => e.ErrorMessage));
            throw new ArgumentException(message);
        }
    }
}
