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

            await _accountService.RegisterByEmail(request);
            return Ok(new ApiSuccess());
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

        private void ThrowModelErrors()
        {
            var message = string.Join(" | ", ModelState.Values
                                                        .SelectMany(v => v.Errors)
                                                        .Select(e => e.ErrorMessage));
            throw new ArgumentException(message);
        }
    }
}
