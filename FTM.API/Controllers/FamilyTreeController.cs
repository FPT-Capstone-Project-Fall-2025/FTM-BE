using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.API.Reponses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] UpsertFamilyTreeRequest request)
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
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] UpsertFamilyTreeRequest request)
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
        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _familyTreeService.DeleteFamilyTreeAsync(id);
                return Ok(new ApiSuccess("Xóa gia phả thành công"));
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
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _familyTreeService.GetFamilyTreesAsync();
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiError($"Lỗi hệ thống: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy danh sách gia phả của người dùng hiện tại
        /// </summary>
        /// <returns>Danh sách gia phả của tôi</returns>
        [HttpGet("my-family-trees")]
        public async Task<IActionResult> GetMyFamilyTrees()
        {
            try
            {
                var result = await _familyTreeService.GetMyFamilyTreesAsync();
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiError($"Lỗi hệ thống: {ex.Message}"));
            }
        }
    }
}