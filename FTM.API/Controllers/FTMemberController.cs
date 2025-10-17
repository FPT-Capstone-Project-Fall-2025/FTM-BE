using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FTMemberController : ControllerBase
    {
        private IFTMemberService _fTMemberService;
        public FTMemberController(IFTMemberService fTMemberService)
        {
            _fTMemberService = fTMemberService;
        }
        [HttpPost("{ftId}/add")]
        public async Task<IActionResult> Add(Guid ftId, [FromBody] UpsertFTMemberRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ThrowModelErrors();
            }

            var result = _fTMemberService.Add(ftId, request);

            return Ok(result);
        }

        private IActionResult ThrowModelErrors()
        {
            var message = string.Join(" | ", ModelState.Values
                                                        .SelectMany(v => v.Errors)
                                                        .Select(e => e.ErrorMessage));
            return BadRequest(new ApiError(message));
        }
    }
}
