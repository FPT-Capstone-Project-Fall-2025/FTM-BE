using AutoMapper.Execution;
using FTM.API.Helpers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Application.Services;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Specification.FamilyTrees;
using FTM.Domain.Specification.FTMembers;
using FTM.Domain.Specification.FTUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "User")]
    public class FTMemberController : ControllerBase
    {
        private readonly IFTMemberService _fTMemberService;
        public FTMemberController(IFTMemberService fTMemberService)
        {
            _fTMemberService = fTMemberService;
        }

        [HttpPost("{ftId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Add([FromRoute] Guid ftId, [FromForm] UpsertFTMemberRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ThrowModelErrors();
            }

            var result = await _fTMemberService.Add(ftId, request);

            return Ok(new ApiSuccess("Tạo thành viên gia phả thành công", result));
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetListOfMembers([FromQuery] SearchWithPaginationRequest requestParams)
        {
            var specParams = new FTMemberSpecParams()
            {
                Search = requestParams.Search ?? string.Empty,
                PropertyFilters = requestParams.PropertyFilters ?? string.Empty,
                OrderBy = requestParams.OrderBy ?? string.Empty,
                Skip = ((requestParams.PageIndex) - 1) * (requestParams.PageSize),
                Take = requestParams.PageSize
            };

            var data = await _fTMemberService.GetListOfMembers(specParams);
            var totalItems = await _fTMemberService.CountMembers(specParams);

            return Ok(new ApiSuccess("Lấy danh sách thành viên của gia phả thành công", new Pagination<FTMemberSimpleDto>(requestParams.PageIndex,
                requestParams.PageSize, totalItems, data)));
        }

        [HttpGet("list-of-ftusers")]
        public async Task<IActionResult> GetListOfFtUser([FromQuery] SearchWithPaginationRequest requestParams)
        {
            var specParams = new FTUserSpecParams()
            {
                Search = requestParams.Search ?? string.Empty,
                PropertyFilters = requestParams.PropertyFilters ?? string.Empty,
                OrderBy = requestParams.OrderBy ?? string.Empty,
                Skip = ((requestParams.PageIndex) - 1) * (requestParams.PageSize),
                Take = requestParams.PageSize
            };

            var data = await _fTMemberService.GetListOfFTUsers(specParams);
            var totalItems = await _fTMemberService.CountFTUsers(specParams);

            return Ok(new ApiSuccess("Lấy danh sách người dùng trong gia phả thành công", new Pagination<FTUserDto>(requestParams.PageIndex,
                requestParams.PageSize, totalItems, data)));
        }

        [HttpGet("list-without-user")]  
        public async Task<IActionResult> GetListOfMembersWithoutUser(Guid ftId)
        {
            var data = await _fTMemberService.GetListOfMembersWithoutUser(ftId);
            return Ok(new ApiSuccess("Lấy danh sách thành viên của gia phả thành công", data));
        }

        [HttpGet("{ftid}/get-by-userid")]
        public async Task<IActionResult> GetDetailedMemberOfFamilyTreeByUserId([FromRoute] Guid ftid, [FromQuery] Guid userId)
        {
            var result = await _fTMemberService.GetByUserId(ftid, userId);
            return Ok(new ApiSuccess("Lấy thông tin của thành viên gia phả thành công", result));
        }

        [HttpGet("{ftid}/get-by-memberid")]
        public async Task<IActionResult> GetDetailedMemberOfFamilyTreeByMemberId([FromRoute] Guid ftid, [FromQuery] Guid memberId)
        {
            var result = await _fTMemberService.GetByMemberId(ftid, memberId);
            return Ok(new ApiSuccess("Lấy thông tin của thành viên gia phả thành công", result));
        }

        [HttpGet("member-tree")]
        public async Task<IActionResult> GetMembersTreeViewAsync([FromQuery] Guid ftId)
        {
            var members = await _fTMemberService.GetMembersTree(ftId);
            return Ok(new ApiSuccess("Lấy cây gia phả thành công",members));
        }

        [HttpPut("{ftId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateMemberDetails([FromRoute] Guid ftId, [FromForm] UpdateFTMemberRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ThrowModelErrors();
            }
            var result = await _fTMemberService.UpdateDetailsByMemberId(ftId, request);
            return Ok(new ApiSuccess("Cập nhật thông tin thành viên thành công", result));
        }

         
        [HttpDelete("{ftMemberId}")]
        public async Task<IActionResult> DeleteMember([FromRoute] Guid ftMemberId)
        {
            await _fTMemberService.Delete(ftMemberId);
            return Ok(new ApiSuccess("Xoá thành viên của gia phả thành công"));
        }

        [HttpGet("{ftMemberId}/relationship")]
        public async Task<IActionResult> GetRelationship([FromRoute] Guid ftMemberId)
        {
            var result = await _fTMemberService.CheckRelationship(ftMemberId);
            return Ok(new ApiSuccess("lấy mối quan hệ của thành viên trong gia phả thành công", result));
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
