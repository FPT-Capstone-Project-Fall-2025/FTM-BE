using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FTAuthorizationController : ControllerBase
    {
        private IFTAuthorizationService _fTAuthorizationService;
        public FTAuthorizationController(IFTAuthorizationService fTAuthorizationService)
        {
            _fTAuthorizationService = fTAuthorizationService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] UpsertFTAuthorizationRequest request)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }

            var result = await _fTAuthorizationService.AddAsync(request);

            return Ok(new ApiSuccess("Thêm quyền thành công", result));
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
