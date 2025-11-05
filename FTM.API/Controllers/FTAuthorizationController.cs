using FTM.API.Helpers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Specification.FamilyTrees;
using FTM.Domain.Specification.FTAuthorizations;
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

        [HttpPost]
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

            var result = await _fTAuthorizationService.GetAuthorizationListViewAsync(specParams);

            return Ok(new ApiSuccess("Lấy danh sách quyền thành công", result));
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
