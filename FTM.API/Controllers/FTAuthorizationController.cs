using FTM.API.Reponses;
using FTM.Domain.DTOs.FamilyTree;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FTAuthorizationController : ControllerBase
    {
        public FTAuthorizationController()
        {
            
        }

        [HttpPost("{ftId}/add")]
        public async Task<IActionResult> Add(Guid ftId, [FromBody] UpsertFTAuthorizationRequest request)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }

           // var result = await _fTMemberService.Add(ftId, request);

            return Ok(new ApiSuccess());
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
