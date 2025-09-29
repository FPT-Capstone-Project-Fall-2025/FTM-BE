using Azure.Core;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.Entities.Identity;
using FTM.Domain.Models;
using FTM.Domain.Models.Authen;
using FTM.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        private void ThrowModelErrors()
        {
            var message = string.Join(" | ", ModelState.Values
                                                        .SelectMany(v => v.Errors)
                                                        .Select(e => e.ErrorMessage));
            throw new ArgumentException(message);
        }
    }
}
