using FTM.API.Helpers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Specification.FamilyTrees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FTM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "User")]
    public class FamilyTreeController : ControllerBase
    {
        private readonly IFamilyTreeService _familyTreeService;

        public FamilyTreeController(IFamilyTreeService familyTreeService)
        {
            _familyTreeService = familyTreeService;
        }

        /// <summary>
        /// Tạo gia phả mới
        /// </summary>
        /// <param name="request">Thông tin gia phả</param>
        /// <returns>Chi tiết gia phả vừa tạo</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Add([FromForm] UpsertFamilyTreeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();
                    foreach (var modelError in ModelState.Values)
                    {
                        foreach (var error in modelError.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }
                    return BadRequest(new ApiError(string.Join(", ", errors)));
                }

                var result = await _familyTreeService.CreateFamilyTreeAsync(request);
                return Ok(new ApiSuccess(result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiError($"Lỗi hệ thống: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy chi tiết gia phả theo ID
        /// </summary>
        /// <param name="id">ID gia phả</param>
        /// <returns>Chi tiết gia phả</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var result = await _familyTreeService.GetFamilyTreeByIdAsync(id);
                return Ok(new ApiSuccess(result));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiError($"Lỗi hệ thống: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cập nhật thông tin gia phả
        /// </summary>
        /// <param name="id">ID gia phả</param>
        /// <param name="request">Thông tin cập nhật</param>
        /// <returns>Chi tiết gia phả sau khi cập nhật</returns>
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Edit(Guid id, [FromForm] UpsertFamilyTreeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();
                    foreach (var modelError in ModelState.Values)
                    {
                        foreach (var error in modelError.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }
                    return BadRequest(new ApiError(string.Join(", ", errors)));
                }

                var result = await _familyTreeService.UpdateFamilyTreeAsync(id, request);
                return Ok(new ApiSuccess(result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiError($"Lỗi hệ thống: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xóa gia phả (soft delete)
        /// </summary>
        /// <param name="id">ID gia phả</param>
        /// <returns>Kết quả xóa</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _familyTreeService.DeleteFamilyTreeAsync(id);
                return Ok(new ApiSuccess("Xóa gia phả thành công", string.Empty));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiError(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiError($"Lỗi hệ thống: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả gia phả
        /// </summary>
        /// <returns>Danh sách gia phả</returns>
        [HttpGet]
        public async Task<ActionResult<Pagination<FamilyTreeDataTableDto>>> GetAll([FromQuery] SearchWithPaginationRequest requestParams)
        {
            var specParams = new FamilyTreeSpecParams()
            {
                Search = requestParams.Search ?? string.Empty,
                PropertyFilters = requestParams.PropertyFilters ?? string.Empty,
                OrderBy = requestParams.OrderBy ?? string.Empty,
                Skip = ((requestParams.PageIndex) - 1) * (requestParams.PageSize),
                Take = requestParams.PageSize
            };

            var data = await _familyTreeService.GetFamilyTreesAsync(specParams);
            var totalItems = await _familyTreeService.CountFamilyTreesAsync(specParams);

            return Ok(new ApiSuccess("Lấy danh sách gia phả thành công", new Pagination<FamilyTreeDataTableDto>(requestParams.PageIndex,
                requestParams.PageSize, totalItems, data)));
        }

        /// <summary>
        /// Lấy danh sách gia phả của người dùng hiện tại
        /// </summary>
        /// <returns>Danh sách gia phả của tôi</returns>
        [HttpGet("my-family-trees")]
        public async Task<ActionResult<Pagination<FamilyTreeDataTableDto>>> GetMyFamilyTrees([FromQuery] SearchWithPaginationRequest requestParams)
        {
            var specParams = new FamilyTreeSpecParams()
            {
                Search = requestParams.Search ?? string.Empty,
                PropertyFilters = requestParams.PropertyFilters ?? string.Empty,
                OrderBy = requestParams.OrderBy ?? string.Empty,
                Skip = ((requestParams.PageIndex) - 1) * (requestParams.PageSize),
                Take = requestParams.PageSize
            };

            var data = await _familyTreeService.GetMyFamilyTreesAsync(specParams);
            var totalItems = await _familyTreeService.CountMyFamilyTreesAsync(specParams);

            return Ok(new ApiSuccess("Lấy danh sách gia phả của tôi thành công", new Pagination<FamilyTreeDataTableDto>(requestParams.PageIndex,
                requestParams.PageSize, totalItems, data)));
        }

    }
}