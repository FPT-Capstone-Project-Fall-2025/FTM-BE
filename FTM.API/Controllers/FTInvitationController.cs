using FTM.API.Reponses;
using FTM.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [Route("api/invitation")]
    [ApiController]
    public class FTInvitationController : ControllerBase
    {
        private IFTInvitationService _fTInvitationService;
        public FTInvitationController(IFTInvitationService fTInvitationService)
        {
            _fTInvitationService = fTInvitationService;
        }

        [HttpGet("respond")]
        public async Task<IActionResult> RespondByEmail([FromQuery] string token, [FromQuery] bool accepted)
        {
            await _fTInvitationService.HandleRespondAsync(token, accepted);
            string msg = accepted ? "Bạn đã chấp nhận lời mời thành công." : "Bạn đã từ chối lời mời.";
            string html = $"<div style='font-family:Arial;text-align:center;margin-top:50px'><h3>{msg}</h3><p>Bạn có thể đóng tab này lại.</p></div>";
            return Content(html, "text/html");
        }
    }
}
