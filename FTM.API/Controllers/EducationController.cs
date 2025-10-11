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
    public class EducationController : ControllerBase
    {
        private readonly IEducationService _educationService;

        public EducationController(IEducationService educationService)
        {
            _educationService = educationService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetEducations()
        {
            try
            {
                var result = await _educationService.GetCurrentUserEducationsAsync();
                return Ok(ApiResponse.Success(result, "Lấy danh sách học vấn thành công"));
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

        [HttpGet("{educationId}")]
        public async Task<ActionResult<ApiResponse>> GetEducation(Guid educationId)
        {
            try
            {
                var result = await _educationService.GetEducationByIdAsync(educationId);
                if (result == null) return NotFound(ApiResponse.Fail("Không tìm thấy học vấn"));
                return Ok(ApiResponse.Success(result, "Lấy chi tiết học vấn thành công"));
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

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateEducation([FromBody] CreateEducationRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Dữ liệu không hợp lệ"));
                var created = await _educationService.CreateEducationAsync(request);
                return CreatedAtAction(nameof(GetEducation), new { educationId = created.Id }, ApiResponse.Success(created, "Tạo học vấn thành công"));
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

        [HttpPut("{educationId}")]
        public async Task<ActionResult<ApiResponse>> UpdateEducation(Guid educationId, [FromBody] UpdateEducationRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Dữ liệu không hợp lệ"));
                var updated = await _educationService.UpdateEducationAsync(educationId, request);
                if (updated == null) return NotFound(ApiResponse.Fail("Không tìm thấy học vấn"));
                return Ok(ApiResponse.Success(updated, "Cập nhật học vấn thành công"));
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

        [HttpDelete("{educationId}")]
        public async Task<ActionResult<ApiResponse>> DeleteEducation(Guid educationId)
        {
            try
            {
                var deleted = await _educationService.DeleteEducationAsync(educationId);
                if (!deleted) return NotFound(ApiResponse.Fail("Không tìm thấy học vấn"));
                return Ok(ApiResponse.Success(null, "Xóa học vấn thành công"));
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
    }
}
