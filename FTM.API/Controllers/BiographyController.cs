using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.Models.Applications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BiographyController : ControllerBase
    {
        private readonly IBiographyService _biographyService;

        public BiographyController(IBiographyService biographyService)
        {
            _biographyService = biographyService;
        }

        #region Biography Description

        /// <summary>
        /// Lấy mô tả tiểu sử của người dùng hiện tại
        /// </summary>
        [HttpGet("description")]
        public async Task<ActionResult<ApiResponse>> GetBiographyDescription()
        {
            try
            {
                var description = await _biographyService.GetCurrentUserBiographyDescriptionAsync();
                return Ok(ApiResponse.Success(description, "Lấy mô tả tiểu sử thành công"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse.Fail("Không có quyền truy cập"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cập nhật mô tả tiểu sử của người dùng hiện tại
        /// </summary>
        [HttpPut("description")]
        public async Task<ActionResult<ApiResponse>> UpdateBiographyDescription([FromBody] UpdateBiographyDescriptionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.Fail("Dữ liệu không hợp lệ"));

                var updatedDescription = await _biographyService.UpdateCurrentUserBiographyDescriptionAsync(request);
                return Ok(ApiResponse.Success(updatedDescription, "Cập nhật mô tả tiểu sử thành công"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse.Fail("Không có quyền truy cập"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail($"Lỗi server: {ex.Message}"));
            }
        }

        #endregion

        #region Biography Events

        /// <summary>
        /// Lấy danh sách các sự kiện tiểu sử của người dùng hiện tại
        /// </summary>
        [HttpGet("events")]
        public async Task<ActionResult<ApiResponse>> GetBiographyEvents()
        {
            try
            {
                var events = await _biographyService.GetCurrentUserBiographyEventsAsync();
                return Ok(ApiResponse.Success(events, "Lấy danh sách tiểu sử thành công"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse.Fail("Không có quyền truy cập"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy chi tiết một sự kiện tiểu sử
        /// </summary>
        [HttpGet("events/{eventId}")]
        public async Task<ActionResult<ApiResponse>> GetBiographyEvent(Guid eventId)
        {
            try
            {
                var biographyEvent = await _biographyService.GetBiographyEventByIdAsync(eventId);
                
                if (biographyEvent == null)
                    return NotFound(ApiResponse.Fail("Không tìm thấy sự kiện tiểu sử"));

                return Ok(ApiResponse.Success(biographyEvent, "Lấy thông tin sự kiện thành công"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse.Fail("Không có quyền truy cập"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// Tạo sự kiện tiểu sử mới
        /// </summary>
        [HttpPost("events")]
        public async Task<ActionResult<ApiResponse>> CreateBiographyEvent([FromBody] CreateBiographyEventRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.Fail("Dữ liệu không hợp lệ"));

                var createdEvent = await _biographyService.CreateBiographyEventAsync(request);
                return CreatedAtAction(
                    nameof(GetBiographyEvent), 
                    new { eventId = createdEvent.Id }, 
                    ApiResponse.Success(createdEvent, "Tạo sự kiện tiểu sử thành công"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse.Fail("Không có quyền truy cập"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cập nhật sự kiện tiểu sử
        /// </summary>
        [HttpPut("events/{eventId}")]
        public async Task<ActionResult<ApiResponse>> UpdateBiographyEvent(
            Guid eventId, 
            [FromBody] UpdateBiographyEventRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.Fail("Dữ liệu không hợp lệ"));

                var updatedEvent = await _biographyService.UpdateBiographyEventAsync(eventId, request);
                
                if (updatedEvent == null)
                    return NotFound(ApiResponse.Fail("Không tìm thấy sự kiện tiểu sử"));

                return Ok(ApiResponse.Success(updatedEvent, "Cập nhật sự kiện tiểu sử thành công"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse.Fail("Không có quyền truy cập"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail($"Lỗi server: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xóa sự kiện tiểu sử
        /// </summary>
        [HttpDelete("events/{eventId}")]
        public async Task<ActionResult<ApiResponse>> DeleteBiographyEvent(Guid eventId)
        {
            try
            {
                var deleted = await _biographyService.DeleteBiographyEventAsync(eventId);
                
                if (!deleted)
                    return NotFound(ApiResponse.Fail("Không tìm thấy sự kiện tiểu sử"));

                return Ok(ApiResponse.Success(null, "Xóa sự kiện tiểu sử thành công"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse.Fail("Không có quyền truy cập"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail($"Lỗi server: {ex.Message}"));
            }
        }

        #endregion
    }
}