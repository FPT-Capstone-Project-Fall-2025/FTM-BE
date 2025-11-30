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
    /// Test các API: GetEducations, GetEducation, CreateEducation, UpdateEducation, DeleteEducation
    /// </summary>
    public class EducationControllerTests
    {
        private readonly Mock<IEducationService> _mockEducationService;
        private readonly EducationController _controller;
        private readonly ITestOutputHelper _output;

        public EducationControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockEducationService = new Mock<IEducationService>();
            _controller = new EducationController(_mockEducationService.Object);
        }

        #region GetEducations Tests

        [Fact(DisplayName = "GetEducations - Thành công - Trả về danh sách học vấn")]
        public async Task GetEducations_Success_ReturnsEducations()
        {
            // Arrange
            var expectedEducations = new List<EducationResponse> { new EducationResponse { Id = Guid.NewGuid(), InstitutionName = "Test University" } };

            _mockEducationService
                .Setup(s => s.GetCurrentUserEducationsAsync())
                .ReturnsAsync(expectedEducations);

            // Act
            var result = await _controller.GetEducations();

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedEducations, apiResponse.Data);
            Assert.Equal("Lấy danh sách học vấn thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "GetEducations - Thất bại - Không có quyền truy cập")]
        public async Task GetEducations_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _mockEducationService
                .Setup(s => s.GetCurrentUserEducationsAsync())
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetEducations();

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "GetEducations - Thất bại - Lỗi server")]
        public async Task GetEducations_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            _mockEducationService
                .Setup(s => s.GetCurrentUserEducationsAsync())
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetEducations();

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

        #region GetEducation Tests

        [Fact(DisplayName = "GetEducation - Thành công - Trả về chi tiết học vấn")]
        public async Task GetEducation_Success_ReturnsEducation()
        {
            // Arrange
            var educationId = Guid.NewGuid();
            var expectedEducation = new EducationResponse { Id = educationId, InstitutionName = "Test University" };

            _mockEducationService
                .Setup(s => s.GetEducationByIdAsync(educationId))
                .ReturnsAsync(expectedEducation);

            // Act
            var result = await _controller.GetEducation(educationId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedEducation, apiResponse.Data);
            Assert.Equal("Lấy chi tiết học vấn thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "GetEducation - Thất bại - Học vấn không tồn tại")]
        public async Task GetEducation_NotFound_ReturnsNotFound()
        {
            // Arrange
            var educationId = Guid.NewGuid();

            _mockEducationService
                .Setup(s => s.GetEducationByIdAsync(educationId))
                .ReturnsAsync((EducationResponse?)null);

            // Act
            var result = await _controller.GetEducation(educationId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không tìm thấy học vấn", apiResponse.Message);
        }

        [Fact(DisplayName = "GetEducation - Thất bại - Không có quyền truy cập")]
        public async Task GetEducation_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var educationId = Guid.NewGuid();

            _mockEducationService
                .Setup(s => s.GetEducationByIdAsync(educationId))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetEducation(educationId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "GetEducation - Thất bại - Lỗi server")]
        public async Task GetEducation_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var educationId = Guid.NewGuid();

            _mockEducationService
                .Setup(s => s.GetEducationByIdAsync(educationId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetEducation(educationId);

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

        #region CreateEducation Tests

        [Fact(DisplayName = "CreateEducation - Thành công - Tạo học vấn mới")]
        public async Task CreateEducation_Success_ReturnsCreated()
        {
            // Arrange
            var request = new CreateEducationRequest { InstitutionName = "New University" };
            var expectedEducation = new EducationResponse { Id = Guid.NewGuid(), InstitutionName = "New University" };

            _mockEducationService
                .Setup(s => s.CreateEducationAsync(request))
                .ReturnsAsync(expectedEducation);

            // Act
            var result = await _controller.CreateEducation(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(createdResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedEducation, apiResponse.Data);
            Assert.Equal("Tạo học vấn thành công", apiResponse.Message);
            Assert.Equal(nameof(_controller.GetEducation), createdResult.ActionName);
        }

        [Fact(DisplayName = "CreateEducation - Thất bại - Dữ liệu không hợp lệ")]
        public async Task CreateEducation_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateEducationRequest(); // Invalid data
            _controller.ModelState.AddModelError("InstitutionName", "Required");

            // Act
            var result = await _controller.CreateEducation(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Dữ liệu không hợp lệ", apiResponse.Message);
        }

        [Fact(DisplayName = "CreateEducation - Thất bại - Không có quyền truy cập")]
        public async Task CreateEducation_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var request = new CreateEducationRequest { InstitutionName = "New University" };

            _mockEducationService
                .Setup(s => s.CreateEducationAsync(request))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.CreateEducation(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "CreateEducation - Thất bại - Lỗi server")]
        public async Task CreateEducation_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var request = new CreateEducationRequest { InstitutionName = "New University" };

            _mockEducationService
                .Setup(s => s.CreateEducationAsync(request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.CreateEducation(request);

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

        #region UpdateEducation Tests

        [Fact(DisplayName = "UpdateEducation - Thành công - Cập nhật học vấn")]
        public async Task UpdateEducation_Success_ReturnsUpdated()
        {
            // Arrange
            var educationId = Guid.NewGuid();
            var request = new UpdateEducationRequest { InstitutionName = "Updated University" };
            var expectedEducation = new EducationResponse { Id = educationId, InstitutionName = "Updated University" };

            _mockEducationService
                .Setup(s => s.UpdateEducationAsync(educationId, request))
                .ReturnsAsync(expectedEducation);

            // Act
            var result = await _controller.UpdateEducation(educationId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedEducation, apiResponse.Data);
            Assert.Equal("Cập nhật học vấn thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateEducation - Thất bại - Học vấn không tồn tại")]
        public async Task UpdateEducation_NotFound_ReturnsNotFound()
        {
            // Arrange
            var educationId = Guid.NewGuid();
            var request = new UpdateEducationRequest { InstitutionName = "Updated University" };

            _mockEducationService
                .Setup(s => s.UpdateEducationAsync(educationId, request))
                .ReturnsAsync((EducationResponse?)null);

            // Act
            var result = await _controller.UpdateEducation(educationId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không tìm thấy học vấn", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateEducation - Thất bại - Dữ liệu không hợp lệ")]
        public async Task UpdateEducation_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var educationId = Guid.NewGuid();
            var request = new UpdateEducationRequest(); // Invalid data
            _controller.ModelState.AddModelError("InstitutionName", "Required");

            // Act
            var result = await _controller.UpdateEducation(educationId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Dữ liệu không hợp lệ", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateEducation - Thất bại - Không có quyền truy cập")]
        public async Task UpdateEducation_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var educationId = Guid.NewGuid();
            var request = new UpdateEducationRequest { InstitutionName = "Updated University" };

            _mockEducationService
                .Setup(s => s.UpdateEducationAsync(educationId, request))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.UpdateEducation(educationId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateEducation - Thất bại - Lỗi server")]
        public async Task UpdateEducation_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var educationId = Guid.NewGuid();
            var request = new UpdateEducationRequest { InstitutionName = "Updated University" };

            _mockEducationService
                .Setup(s => s.UpdateEducationAsync(educationId, request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.UpdateEducation(educationId, request);

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

        #region DeleteEducation Tests

        [Fact(DisplayName = "DeleteEducation - Thành công - Xóa học vấn")]
        public async Task DeleteEducation_Success_ReturnsOk()
        {
            // Arrange
            var educationId = Guid.NewGuid();

            _mockEducationService
                .Setup(s => s.DeleteEducationAsync(educationId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteEducation(educationId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal("Xóa học vấn thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "DeleteEducation - Thất bại - Học vấn không tồn tại")]
        public async Task DeleteEducation_NotFound_ReturnsNotFound()
        {
            // Arrange
            var educationId = Guid.NewGuid();

            _mockEducationService
                .Setup(s => s.DeleteEducationAsync(educationId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteEducation(educationId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không tìm thấy học vấn", apiResponse.Message);
        }

        [Fact(DisplayName = "DeleteEducation - Thất bại - Không có quyền truy cập")]
        public async Task DeleteEducation_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var educationId = Guid.NewGuid();

            _mockEducationService
                .Setup(s => s.DeleteEducationAsync(educationId))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.DeleteEducation(educationId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "DeleteEducation - Thất bại - Lỗi server")]
        public async Task DeleteEducation_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var educationId = Guid.NewGuid();

            _mockEducationService
                .Setup(s => s.DeleteEducationAsync(educationId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.DeleteEducation(educationId);

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