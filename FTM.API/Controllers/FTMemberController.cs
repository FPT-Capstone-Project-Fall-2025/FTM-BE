using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Application.Services;
using FTM.Domain.DTOs.FamilyTree;
using Microsoft.AspNetCore.Authorization;
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

            var result = await _fTMemberService.Add(ftId, request);

            return Ok(new ApiSuccess("Tạo thành viên gia phả thành công", result));
        }

        [HttpGet("{ftid}/get-by-userid")]
        public async Task<IActionResult> GetDetailedMemberOfFamilyTreeByUserId([FromRoute] Guid ftid, [FromQuery] Guid userId)
        {
            var result = await _fTMemberService.GetByUserId(ftid, userId);
            return Ok(new ApiSuccess("Lấy thông tin của thành viên gia phả thành công", result));
        }

        [HttpGet("member-tree")]
        public async Task<IActionResult> GetMembersTreeViewAsync([FromQuery] Guid ftId)
        {
            var members = await _fTMemberService.GetMembersTree(ftId);
            return Ok(new ApiSuccess("Lấy cây gia phả thành công",members));
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
