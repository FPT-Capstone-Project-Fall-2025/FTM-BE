using FTM.API.Controllers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.Models.Applications;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FTM.Tests.Controllers
{
    /// <summary>
    /// Test các API: GetWorks, GetWork, CreateWork, UpdateWork, DeleteWork
    /// </summary>
    public class WorkControllerTests
    {
        private readonly Mock<IWorkService> _mockWorkService;
        private readonly WorkController _controller;
        private readonly ITestOutputHelper _output;

        public WorkControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockWorkService = new Mock<IWorkService>();
            _controller = new WorkController(_mockWorkService.Object);
        }

        #region GetWorks Tests

        [Fact(DisplayName = "GetWorks - Thành công - Trả về danh sách công việc")]
        public async Task GetWorks_Success_ReturnsWorks()
        {
            // Arrange
            var expectedWorks = new List<WorkResponse> { new WorkResponse { Id = Guid.NewGuid(), CompanyName = "Test Company" } };

            _mockWorkService
                .Setup(s => s.GetCurrentUserWorkAsync())
                .Returns(Task.FromResult((IEnumerable<WorkResponse>)expectedWorks));

            // Act
            var result = await _controller.GetWorks();

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedWorks, apiResponse.Data);
            Assert.Equal("Lấy lịch sử công việc thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "GetWorks - Thất bại - Không có quyền truy cập")]
        public async Task GetWorks_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _mockWorkService
                .Setup(s => s.GetCurrentUserWorkAsync())
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetWorks();

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "GetWorks - Thất bại - Lỗi server")]
        public async Task GetWorks_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            _mockWorkService
                .Setup(s => s.GetCurrentUserWorkAsync())
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetWorks();

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var statusCodeResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse>(statusCodeResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Contains("Lỗi server: Server error", apiResponse.Message);
        }

        #endregion

        #region GetWork Tests

        [Fact(DisplayName = "GetWork - Thành công - Trả về chi tiết công việc")]
        public async Task GetWork_Success_ReturnsWork()
        {
            // Arrange
            var workId = Guid.NewGuid();
            var expectedWork = new WorkResponse { Id = workId, CompanyName = "Test Company" };

            _mockWorkService
                .Setup(s => s.GetWorkByIdAsync(workId))
                .Returns(Task.FromResult((WorkResponse?)expectedWork));

            // Act
            var result = await _controller.GetWork(workId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedWork, apiResponse.Data);
            Assert.Equal("Lấy chi tiết công việc thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "GetWork - Thất bại - Công việc không tồn tại")]
        public async Task GetWork_NotFound_ReturnsNotFound()
        {
            // Arrange
            var workId = Guid.NewGuid();

            _mockWorkService
                .Setup(s => s.GetWorkByIdAsync(workId))
                .Returns(Task.FromResult((WorkResponse?)null));

            // Act
            var result = await _controller.GetWork(workId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không tìm thấy công việc", apiResponse.Message);
        }

        [Fact(DisplayName = "GetWork - Thất bại - Không có quyền truy cập")]
        public async Task GetWork_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var workId = Guid.NewGuid();

            _mockWorkService
                .Setup(s => s.GetWorkByIdAsync(workId))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetWork(workId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "GetWork - Thất bại - Lỗi server")]
        public async Task GetWork_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var workId = Guid.NewGuid();

            _mockWorkService
                .Setup(s => s.GetWorkByIdAsync(workId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetWork(workId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var statusCodeResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse>(statusCodeResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Contains("Lỗi server: Server error", apiResponse.Message);
        }

        #endregion

        #region CreateWork Tests

        [Fact(DisplayName = "CreateWork - Thành công - Tạo công việc mới")]
        public async Task CreateWork_Success_ReturnsCreated()
        {
            // Arrange
            var request = new CreateWorkRequest { CompanyName = "New Company" };
            var expectedWork = new WorkResponse { Id = Guid.NewGuid(), CompanyName = "New Company" };

            _mockWorkService
                .Setup(s => s.CreateWorkAsync(request))
                .Returns(Task.FromResult(expectedWork));

            // Act
            var result = await _controller.CreateWork(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(createdResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedWork, apiResponse.Data);
            Assert.Equal("Tạo công việc thành công", apiResponse.Message);
            Assert.Equal(nameof(_controller.GetWork), createdResult.ActionName);
        }

        [Fact(DisplayName = "CreateWork - Thất bại - Dữ liệu không hợp lệ")]
        public async Task CreateWork_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateWorkRequest(); // Invalid data
            _controller.ModelState.AddModelError("CompanyName", "Required");

            // Act
            var result = await _controller.CreateWork(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Dữ liệu không hợp lệ", apiResponse.Message);
        }

        [Fact(DisplayName = "CreateWork - Thất bại - Không có quyền truy cập")]
        public async Task CreateWork_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var request = new CreateWorkRequest { CompanyName = "New Company" };

            _mockWorkService
                .Setup(s => s.CreateWorkAsync(request))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.CreateWork(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "CreateWork - Thất bại - Lỗi server")]
        public async Task CreateWork_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var request = new CreateWorkRequest { CompanyName = "New Company" };

            _mockWorkService
                .Setup(s => s.CreateWorkAsync(request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.CreateWork(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var statusCodeResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse>(statusCodeResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Contains("Lỗi server: Server error", apiResponse.Message);
        }

        #endregion

        #region UpdateWork Tests

        [Fact(DisplayName = "UpdateWork - Thành công - Cập nhật công việc")]
        public async Task UpdateWork_Success_ReturnsUpdated()
        {
            // Arrange
            var workId = Guid.NewGuid();
            var request = new UpdateWorkRequest { CompanyName = "Updated Company" };
            var expectedWork = new WorkResponse { Id = workId, CompanyName = "Updated Company" };

            _mockWorkService
                .Setup(s => s.UpdateWorkAsync(workId, request))
                .Returns(Task.FromResult((WorkResponse?)expectedWork));

            // Act
            var result = await _controller.UpdateWork(workId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedWork, apiResponse.Data);
            Assert.Equal("Cập nhật công việc thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateWork - Thất bại - Công việc không tồn tại")]
        public async Task UpdateWork_NotFound_ReturnsNotFound()
        {
            // Arrange
            var workId = Guid.NewGuid();
            var request = new UpdateWorkRequest { CompanyName = "Updated Company" };

            _mockWorkService
                .Setup(s => s.UpdateWorkAsync(workId, request))
                .Returns(Task.FromResult((WorkResponse?)null));

            // Act
            var result = await _controller.UpdateWork(workId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không tìm thấy công việc", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateWork - Thất bại - Dữ liệu không hợp lệ")]
        public async Task UpdateWork_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var workId = Guid.NewGuid();
            var request = new UpdateWorkRequest(); // Invalid data
            _controller.ModelState.AddModelError("CompanyName", "Required");

            // Act
            var result = await _controller.UpdateWork(workId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Dữ liệu không hợp lệ", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateWork - Thất bại - Không có quyền truy cập")]
        public async Task UpdateWork_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var workId = Guid.NewGuid();
            var request = new UpdateWorkRequest { CompanyName = "Updated Company" };

            _mockWorkService
                .Setup(s => s.UpdateWorkAsync(workId, request))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.UpdateWork(workId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateWork - Thất bại - Lỗi server")]
        public async Task UpdateWork_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var workId = Guid.NewGuid();
            var request = new UpdateWorkRequest { CompanyName = "Updated Company" };

            _mockWorkService
                .Setup(s => s.UpdateWorkAsync(workId, request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.UpdateWork(workId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var statusCodeResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse>(statusCodeResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Contains("Lỗi server: Server error", apiResponse.Message);
        }

        #endregion

        #region DeleteWork Tests

        [Fact(DisplayName = "DeleteWork - Thành công - Xóa công việc")]
        public async Task DeleteWork_Success_ReturnsOk()
        {
            // Arrange
            var workId = Guid.NewGuid();

            _mockWorkService
                .Setup(s => s.DeleteWorkAsync(workId))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _controller.DeleteWork(workId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal("Xóa công việc thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "DeleteWork - Thất bại - Công việc không tồn tại")]
        public async Task DeleteWork_NotFound_ReturnsNotFound()
        {
            // Arrange
            var workId = Guid.NewGuid();

            _mockWorkService
                .Setup(s => s.DeleteWorkAsync(workId))
                .Returns(Task.FromResult(false));

            // Act
            var result = await _controller.DeleteWork(workId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không tìm thấy công việc", apiResponse.Message);
        }

        [Fact(DisplayName = "DeleteWork - Thất bại - Không có quyền truy cập")]
        public async Task DeleteWork_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var workId = Guid.NewGuid();

            _mockWorkService
                .Setup(s => s.DeleteWorkAsync(workId))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.DeleteWork(workId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "DeleteWork - Thất bại - Lỗi server")]
        public async Task DeleteWork_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var workId = Guid.NewGuid();

            _mockWorkService
                .Setup(s => s.DeleteWorkAsync(workId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.DeleteWork(workId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var statusCodeResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse>(statusCodeResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Contains("Lỗi server: Server error", apiResponse.Message);
        }

        #endregion
    }
}
