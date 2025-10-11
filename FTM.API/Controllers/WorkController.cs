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
    public class WorkController : ControllerBase
    {
        private readonly IWorkService _workService;

        public WorkController(IWorkService workService)
        {
            _workService = workService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetWorks()
        {
            try
            {
                var result = await _workService.GetCurrentUserWorkAsync();
                return Ok(ApiResponse.Success(result, "Lấy lịch sử công việc thành công"));
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

        [HttpGet("{workId}")]
        public async Task<ActionResult<ApiResponse>> GetWork(Guid workId)
        {
            try
            {
                var result = await _workService.GetWorkByIdAsync(workId);
                if (result == null) return NotFound(ApiResponse.Fail("Không tìm thấy công việc"));
                return Ok(ApiResponse.Success(result, "Lấy chi tiết công việc thành công"));
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
        public async Task<ActionResult<ApiResponse>> CreateWork([FromBody] CreateWorkRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Dữ liệu không hợp lệ"));
                var created = await _workService.CreateWorkAsync(request);
                return CreatedAtAction(nameof(GetWork), new { workId = created.Id }, ApiResponse.Success(created, "Tạo công việc thành công"));
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

        [HttpPut("{workId}")]
        public async Task<ActionResult<ApiResponse>> UpdateWork(Guid workId, [FromBody] UpdateWorkRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ApiResponse.Fail("Dữ liệu không hợp lệ"));
                var updated = await _workService.UpdateWorkAsync(workId, request);
                if (updated == null) return NotFound(ApiResponse.Fail("Không tìm thấy công việc"));
                return Ok(ApiResponse.Success(updated, "Cập nhật công việc thành công"));
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

        [HttpDelete("{workId}")]
        public async Task<ActionResult<ApiResponse>> DeleteWork(Guid workId)
        {
            try
            {
                var deleted = await _workService.DeleteWorkAsync(workId);
                if (!deleted) return NotFound(ApiResponse.Fail("Không tìm thấy công việc"));
                return Ok(ApiResponse.Success(null, "Xóa công việc thành công"));
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
