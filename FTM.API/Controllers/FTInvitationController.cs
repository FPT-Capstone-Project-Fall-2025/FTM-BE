using FTM.API.Helpers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Application.Services;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Specification.FTInvitations;
using FTM.Domain.Specification.FTUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FTM.API.Controllers
{
    [Route("api/invitation")]
    [ApiController]
    //[Authorize(Roles = "User")]
    public class FTInvitationController : ControllerBase
    {
        private IFTInvitationService _fTInvitationService;
        public FTInvitationController(IFTInvitationService fTInvitationService)
        {
            _fTInvitationService = fTInvitationService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetList([FromQuery] SearchWithPaginationRequest requestParams)
        {
            var specParams = new FTInvitationSpecParams()
            {
                Search = requestParams.Search ?? string.Empty,
                PropertyFilters = requestParams.PropertyFilters ?? string.Empty,
                OrderBy = requestParams.OrderBy ?? string.Empty,
                Skip = ((requestParams.PageIndex) - 1) * (requestParams.PageSize),
                Take = requestParams.PageSize
            };

            var data = await _fTInvitationService.ListAsync(specParams);
            var totalItems = await _fTInvitationService.CountListAsync(specParams);

            return Ok(new ApiSuccess("Lấy danh sách lời mời thành công", new Pagination<SimpleFTInvitationDto>(requestParams.PageIndex,
                requestParams.PageSize, totalItems, data)));
        }

        [HttpPost("guest")]
        public async Task<IActionResult> InviteToGuest(GuestInvitationDto request)
        {
            await _fTInvitationService.InviteToGuestAsync(request);
            return Ok(new ApiSuccess("Gửi lời mời thành công"));
        }

        [HttpPost("member")]
        public async Task<IActionResult> InviteToMember(MemberInvitationDto request)
        {
            await _fTInvitationService.InviteToMemberAsync(request);
            return Ok(new ApiSuccess("Gửi lời mời thành công"));
        }

        [HttpGet("respond")]
        [AllowAnonymous]
        public async Task<IActionResult> RespondInvitation([FromQuery(Name = "relatedId")] Guid invitationId, [FromQuery] bool accepted)
        {
            await _fTInvitationService.HandleRespondAsync(invitationId, accepted);
            string msg = accepted
                ? "Bạn đã chấp nhận lời mời thành công."
                : "Bạn đã từ chối lời mời.";

            string html = $@"
                <div style='font-family:Arial;text-align:center;margin-top:50px'>
                    <h3>{msg}</h3>
                    <p>Bạn có thể đóng tab này lại.</p>
                </div>";

            return Content(html, "text/html; charset=utf-8");
        }
    }
}
