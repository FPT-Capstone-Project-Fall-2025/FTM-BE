using FTM.API.Helpers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using FTM.Domain.Specification.FamilyTrees;
using FTM.Domain.Specification.FTAuthorizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FTM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "User")]
    public class FTAuthorizationController : ControllerBase
    {
        private IFTAuthorizationService _fTAuthorizationService;
        public FTAuthorizationController(IFTAuthorizationService fTAuthorizationService)
        {
            _fTAuthorizationService = fTAuthorizationService;
        }

        [HttpPost]
        //[FTAuthorizeOwner]
        public async Task<IActionResult> Add([FromBody] UpsertFTAuthorizationRequest request)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }

            var result = await _fTAuthorizationService.AddAsync(request);

            return Ok(new ApiSuccess("Thêm quyền thành công", result));
        }

        [HttpPut]
        //[FTAuthorizeOwner]
        public async Task<IActionResult> Update([FromBody] UpsertFTAuthorizationRequest request)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }

            var result = await _fTAuthorizationService.UpdateAsync(request);

            return Ok(new ApiSuccess("Thêm quyền thành công", result));
        }


        [HttpGet("list")]
        //[FTAuthorizeOwner]
        public async Task<IActionResult> ViewAuthorizationList([FromQuery] SearchWithPaginationRequest requestParams)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }

            var specParams = new FTAuthorizationSpecParams()
            {
                Search = requestParams.Search ?? string.Empty,
                PropertyFilters = requestParams.PropertyFilters ?? string.Empty,
                OrderBy = requestParams.OrderBy ?? string.Empty,
                Skip = ((requestParams.PageIndex) - 1) * (requestParams.PageSize),
                Take = requestParams.PageSize
            };

            var data = await _fTAuthorizationService.GetAuthorizationListViewAsync(specParams);
            var totalItems = await _fTAuthorizationService.CountAuthorizationListViewAsync(specParams);

            IReadOnlyList<FTAuthorizationListViewDto> simpleData = new List<FTAuthorizationListViewDto> { data };

            return Ok(new ApiSuccess(
                "Lấy danh sách quyền của gia phả thành công",
                new Pagination<FTAuthorizationListViewDto>(
                    requestParams.PageIndex,
                    requestParams.PageSize,
                    totalItems,
                    simpleData)));
        }

        [HttpGet("{ftId}/member/{ftMemberId}/list")]
        //[FTAuthorizeOwner]
        public async Task<IActionResult> GetAuthorizationOfFtMember(Guid ftId, Guid ftMemberId)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }

            var ftMemberAuthorization = await _fTAuthorizationService.GetAuthorizationAsync(ftId, ftMemberId);

            return Ok(new ApiSuccess("Lấy quyền thành công", ftMemberAuthorization));
        }

        [HttpDelete("{ftId}/member/{ftMemberId}")]
        //[FTAuthorizeOwner]
        public async Task<IActionResult> Delete(Guid ftId, Guid ftMemberId)
        {
            if (!ModelState.IsValid)
            {
                ThrowModelErrors();
            }

            await _fTAuthorizationService.DeleteAuthorizationAsync(ftId, ftMemberId);

            return Ok(new ApiSuccess("Xóa quyền thành công"));
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
