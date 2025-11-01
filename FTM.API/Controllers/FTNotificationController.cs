using FTM.API.Reponses;
using FTM.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XAct.Security;

namespace FTM.API.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    [Authorization]
    public class FTNotificationController : ControllerBase
    {

        private readonly IFTNotificationService _fTNotificationService;
        public FTNotificationController(IFTNotificationService fTNotificationService)
        {
            _fTNotificationService = fTNotificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications() {
            var result = await _fTNotificationService.FindByuserIdAsync();
            return Ok(new ApiSuccess("Lấy thông báo thành công", result));
        }
    }
}
