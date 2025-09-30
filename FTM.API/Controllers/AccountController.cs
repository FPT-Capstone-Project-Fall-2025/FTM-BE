﻿using Azure.Core;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.Entities.Identity;
using FTM.Domain.Models;
using FTM.Domain.Models.Authen;
using FTM.Infrastructure.Data;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XAct.Users;

namespace FTM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<ApplicationUser> _userManager;
        public AccountController(IAccountService accountService, UserManager<ApplicationUser> userManager)
        {
            _accountService = accountService;
            _userManager = userManager;
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

            try
            {
                var result = await _accountService.Login(model.UserName, model.Password);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex) {
                return BadRequest(new ApiError(ex.Message));
            }

        }

        [HttpPost("login/google")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
                var email = payload.Email;
                var fullName = payload.Name + payload.GivenName;

                var result = await _accountService.LoginWithGoogle(fullName, email);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }
            
            try
            {
                await _accountService.RegisterByEmail(request);
                return Ok(new ApiSuccess());
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(Guid userId, string token)
        {
            bool isConfirmed = await _accountService.ConfirmEmail(userId, token);
            if (isConfirmed)
            {
                return Ok(new ApiSuccess("Xác nhận email thành công.", new Object()));
            }
            return Ok(new ApiError("Xác nhận email thất bại.", new Object()));
        }

        [HttpPost("Logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string accessToken)
        {
            try
            {
                await _accountService.Logout(accessToken);
                return Ok(new ApiSuccess());
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }

            try
            {
                await _accountService.ForgotPasswordAsync(model);
                return Ok(new ApiSuccess());
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }

            try
            {
                await _accountService.ResetPasswordAsync(model);
                return Ok(new ApiSuccess());
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
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

        private void ThrowModelErrors()
        {
            var message = string.Join(" | ", ModelState.Values
                                                        .SelectMany(v => v.Errors)
                                                        .Select(e => e.ErrorMessage));
            throw new ArgumentException(message);
        }
    }
}
