using FTM.API.Controllers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.HonorBoard;
using FTM.Domain.Specification.HonorBoard;
using FTM.API.Helpers;
using FTM.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace FTM.Tests.Controllers
{
    /// <summary>
    /// Test các API: GetAcademicHonors, GetAcademicHonorById, CreateAcademicHonor, UpdateAcademicHonor, DeleteAcademicHonor
    /// </summary>
    public class AcademicHonorControllerTests
    {
        private readonly Mock<IAcademicHonorService> _mockAcademicHonorService;
        private readonly AcademicHonorController _controller;
        private readonly ITestOutputHelper _output;

        public AcademicHonorControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockAcademicHonorService = new Mock<IAcademicHonorService>();
            _controller = new AcademicHonorController(_mockAcademicHonorService.Object);
        }

        #region GetAcademicHonors Tests

        [Fact(DisplayName = "GetAcademicHonors - Thành công - Trả về danh sách academic honors")]
        public async Task GetAcademicHonors_Success_ReturnsAcademicHonors()
        {
            // Arrange
            var requestParams = new FTM.API.Helpers.SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var specParams = new AcademicHonorSpecParams { Skip = 0, Take = 10 };
            var expectedResult = new FTM.Domain.Models.Pagination<AcademicHonorDto>(1, 10, 0, new List<AcademicHonorDto>());

            _mockAcademicHonorService
                .Setup(s => s.GetAcademicHonorsAsync(It.IsAny<AcademicHonorSpecParams>()))
                .Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _controller.GetAcademicHonors(null, null, null, null, null, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedResult, apiSuccess.Data);
        }

        [Fact(DisplayName = "GetAcademicHonors - Thất bại - Lỗi server")]
        public async Task GetAcademicHonors_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var requestParams = new FTM.API.Helpers.SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };

            _mockAcademicHonorService
                .Setup(s => s.GetAcademicHonorsAsync(It.IsAny<AcademicHonorSpecParams>()))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetAcademicHonors(null, null, null, null, null, requestParams);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion

        #region GetAcademicHonorById Tests

        [Fact(DisplayName = "GetAcademicHonorById - Thành công - Trả về academic honor theo ID")]
        public async Task GetAcademicHonorById_Success_ReturnsAcademicHonor()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedResult = new AcademicHonorDto { Id = id, AchievementTitle = "Test Honor" };

            _mockAcademicHonorService
                .Setup(s => s.GetAcademicHonorByIdAsync(id))
                .Returns(Task.FromResult<AcademicHonorDto?>(expectedResult));

            // Act
            var result = await _controller.GetAcademicHonorById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedResult, apiSuccess.Data);
        }

        [Fact(DisplayName = "GetAcademicHonorById - Thất bại - Academic honor không tồn tại")]
        public async Task GetAcademicHonorById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockAcademicHonorService
                .Setup(s => s.GetAcademicHonorByIdAsync(id))
                .Returns(Task.FromResult<AcademicHonorDto?>(null));

            // Act
            var result = await _controller.GetAcademicHonorById(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal("Academic honor not found", apiError.Message);
        }

        [Fact(DisplayName = "GetAcademicHonorById - Thất bại - Lỗi server")]
        public async Task GetAcademicHonorById_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockAcademicHonorService
                .Setup(s => s.GetAcademicHonorByIdAsync(id))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetAcademicHonorById(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion

        #region CreateAcademicHonor Tests

        [Fact(DisplayName = "CreateAcademicHonor - Thành công - Tạo academic honor mới")]
        public async Task CreateAcademicHonor_Success_ReturnsCreated()
        {
            // Arrange
            var request = new CreateAcademicHonorRequest { AchievementTitle = "New Honor" };
            var expectedResult = new AcademicHonorDto { Id = Guid.NewGuid(), AchievementTitle = "New Honor" };

            _mockAcademicHonorService
                .Setup(s => s.CreateAcademicHonorAsync(request))
                .Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _controller.CreateAcademicHonor(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(createdResult.Value);
            Assert.Equal(expectedResult, apiSuccess.Data);
            Assert.Equal(nameof(_controller.GetAcademicHonorById), createdResult.ActionName);
        }

        [Fact(DisplayName = "CreateAcademicHonor - Thất bại - Dữ liệu không hợp lệ")]
        public async Task CreateAcademicHonor_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateAcademicHonorRequest(); // Invalid data
            _controller.ModelState.AddModelError("AchievementTitle", "Required");

            // Act
            var result = await _controller.CreateAcademicHonor(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal("Invalid request data", apiError.Message);
        }

        [Fact(DisplayName = "CreateAcademicHonor - Thất bại - Không có quyền")]
        public async Task CreateAcademicHonor_Unauthorized_ReturnsForbid()
        {
            // Arrange
            var request = new CreateAcademicHonorRequest { AchievementTitle = "New Honor" };

            _mockAcademicHonorService
                .Setup(s => s.CreateAcademicHonorAsync(request))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.CreateAcademicHonor(request);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact(DisplayName = "CreateAcademicHonor - Thất bại - Invalid operation")]
        public async Task CreateAcademicHonor_InvalidOperation_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateAcademicHonorRequest { AchievementTitle = "New Honor" };

            _mockAcademicHonorService
                .Setup(s => s.CreateAcademicHonorAsync(request))
                .ThrowsAsync(new InvalidOperationException("Invalid operation"));

            // Act
            var result = await _controller.CreateAcademicHonor(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal("Invalid operation", apiError.Message);
        }

        [Fact(DisplayName = "CreateAcademicHonor - Thất bại - Lỗi server")]
        public async Task CreateAcademicHonor_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateAcademicHonorRequest { AchievementTitle = "New Honor" };

            _mockAcademicHonorService
                .Setup(s => s.CreateAcademicHonorAsync(request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.CreateAcademicHonor(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion

        #region UpdateAcademicHonor Tests

        [Fact(DisplayName = "UpdateAcademicHonor - Thành công - Cập nhật academic honor")]
        public async Task UpdateAcademicHonor_Success_ReturnsUpdated()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateAcademicHonorRequest { AchievementTitle = "Updated Honor" };
            var expectedResult = new AcademicHonorDto { Id = id, AchievementTitle = "Updated Honor" };

            _mockAcademicHonorService
                .Setup(s => s.UpdateAcademicHonorAsync(id, request))
                .Returns(Task.FromResult<AcademicHonorDto?>(expectedResult));

            // Act
            var result = await _controller.UpdateAcademicHonor(id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedResult, apiSuccess.Data);
        }

        [Fact(DisplayName = "UpdateAcademicHonor - Thất bại - Academic honor không tồn tại")]
        public async Task UpdateAcademicHonor_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateAcademicHonorRequest { AchievementTitle = "Updated Honor" };

            _mockAcademicHonorService
                .Setup(s => s.UpdateAcademicHonorAsync(id, request))
                .Returns(Task.FromResult<AcademicHonorDto?>(null));

            // Act
            var result = await _controller.UpdateAcademicHonor(id, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal("Academic honor not found", apiError.Message);
        }

        [Fact(DisplayName = "UpdateAcademicHonor - Thất bại - Dữ liệu không hợp lệ")]
        public async Task UpdateAcademicHonor_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateAcademicHonorRequest(); // Invalid data
            _controller.ModelState.AddModelError("AchievementTitle", "Required");

            // Act
            var result = await _controller.UpdateAcademicHonor(id, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal("Invalid request data", apiError.Message);
        }

        [Fact(DisplayName = "UpdateAcademicHonor - Thất bại - Không có quyền")]
        public async Task UpdateAcademicHonor_Unauthorized_ReturnsForbid()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateAcademicHonorRequest { AchievementTitle = "Updated Honor" };

            _mockAcademicHonorService
                .Setup(s => s.UpdateAcademicHonorAsync(id, request))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.UpdateAcademicHonor(id, request);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact(DisplayName = "UpdateAcademicHonor - Thất bại - Lỗi server")]
        public async Task UpdateAcademicHonor_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateAcademicHonorRequest { AchievementTitle = "Updated Honor" };

            _mockAcademicHonorService
                .Setup(s => s.UpdateAcademicHonorAsync(id, request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.UpdateAcademicHonor(id, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion

        #region DeleteAcademicHonor Tests

        [Fact(DisplayName = "DeleteAcademicHonor - Thành công - Xóa academic honor")]
        public async Task DeleteAcademicHonor_Success_ReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockAcademicHonorService
                .Setup(s => s.DeleteAcademicHonorAsync(id))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAcademicHonor(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Success", apiSuccess.Message);
        }

        [Fact(DisplayName = "DeleteAcademicHonor - Thất bại - Academic honor không tồn tại")]
        public async Task DeleteAcademicHonor_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockAcademicHonorService
                .Setup(s => s.DeleteAcademicHonorAsync(id))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteAcademicHonor(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal("Academic honor not found", apiError.Message);
        }

        [Fact(DisplayName = "DeleteAcademicHonor - Thất bại - Không có quyền")]
        public async Task DeleteAcademicHonor_Unauthorized_ReturnsForbid()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockAcademicHonorService
                .Setup(s => s.DeleteAcademicHonorAsync(id))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.DeleteAcademicHonor(id);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact(DisplayName = "DeleteAcademicHonor - Thất bại - Lỗi server")]
        public async Task DeleteAcademicHonor_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockAcademicHonorService
                .Setup(s => s.DeleteAcademicHonorAsync(id))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.DeleteAcademicHonor(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion
    }
}