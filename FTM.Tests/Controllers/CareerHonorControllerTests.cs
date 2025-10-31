using FTM.API.Controllers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.HonorBoard;
using FTM.Domain.Models;
using FTM.Domain.Specification.HonorBoard;
using FTM.API.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace FTM.Tests.Controllers
{
    /// <summary>
    /// Test các API: GetCareerHonors, GetCareerHonorById, CreateCareerHonor, UpdateCareerHonor, DeleteCareerHonor
    /// </summary>
    public class CareerHonorControllerTests
    {
        private readonly Mock<ICareerHonorService> _mockCareerHonorService;
        private readonly CareerHonorController _controller;
        private readonly ITestOutputHelper _output;

        public CareerHonorControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockCareerHonorService = new Mock<ICareerHonorService>();
            _controller = new CareerHonorController(_mockCareerHonorService.Object);
        }

        #region GetCareerHonors Tests

        [Fact(DisplayName = "GetCareerHonors - Thành công - Trả về danh sách career honors")]
        public async Task GetCareerHonors_Success_ReturnsCareerHonors()
        {
            // Arrange
            var requestParams = new FTM.API.Helpers.SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var specParams = new CareerHonorSpecParams { Skip = 0, Take = 10 };
            var expectedResult = new FTM.Domain.Models.Pagination<CareerHonorDto>(1, 10, 0, new List<CareerHonorDto>());

            _mockCareerHonorService
                .Setup(s => s.GetCareerHonorsAsync(It.IsAny<CareerHonorSpecParams>()))
                .Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _controller.GetCareerHonors(null, null, null, null, null, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedResult, apiSuccess.Data);
        }

        [Fact(DisplayName = "GetCareerHonors - Thất bại - Lỗi server")]
        public async Task GetCareerHonors_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var requestParams = new FTM.API.Helpers.SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };

            _mockCareerHonorService
                .Setup(s => s.GetCareerHonorsAsync(It.IsAny<CareerHonorSpecParams>()))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetCareerHonors(null, null, null, null, null, requestParams);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion

        #region GetCareerHonorById Tests

        [Fact(DisplayName = "GetCareerHonorById - Thành công - Trả về career honor theo ID")]
        public async Task GetCareerHonorById_Success_ReturnsCareerHonor()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedResult = new CareerHonorDto { Id = id, AchievementTitle = "Test Honor" };

            _mockCareerHonorService
                .Setup(s => s.GetCareerHonorByIdAsync(id))
                .Returns(Task.FromResult<CareerHonorDto?>(expectedResult));

            // Act
            var result = await _controller.GetCareerHonorById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedResult, apiSuccess.Data);
        }

        [Fact(DisplayName = "GetCareerHonorById - Thất bại - Career honor không tồn tại")]
        public async Task GetCareerHonorById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockCareerHonorService
                .Setup(s => s.GetCareerHonorByIdAsync(id))
                .Returns(Task.FromResult<CareerHonorDto?>(null));

            // Act
            var result = await _controller.GetCareerHonorById(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal("Career honor not found", apiError.Message);
        }

        [Fact(DisplayName = "GetCareerHonorById - Thất bại - Lỗi server")]
        public async Task GetCareerHonorById_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockCareerHonorService
                .Setup(s => s.GetCareerHonorByIdAsync(id))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetCareerHonorById(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion

        #region CreateCareerHonor Tests

        [Fact(DisplayName = "CreateCareerHonor - Thành công - Tạo career honor mới")]
        public async Task CreateCareerHonor_Success_ReturnsCreated()
        {
            // Arrange
            var request = new CreateCareerHonorRequest { AchievementTitle = "New Honor" };
            var expectedResult = new CareerHonorDto { Id = Guid.NewGuid(), AchievementTitle = "New Honor" };

            _mockCareerHonorService
                .Setup(s => s.CreateCareerHonorAsync(request))
                .Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _controller.CreateCareerHonor(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(createdResult.Value);
            Assert.Equal(expectedResult, apiSuccess.Data);
            Assert.Equal(nameof(_controller.GetCareerHonorById), createdResult.ActionName);
        }

        [Fact(DisplayName = "CreateCareerHonor - Thất bại - Dữ liệu không hợp lệ")]
        public async Task CreateCareerHonor_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateCareerHonorRequest(); // Invalid data
            _controller.ModelState.AddModelError("AchievementTitle", "Required");

            // Act
            var result = await _controller.CreateCareerHonor(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal("Invalid request data", apiError.Message);
        }

        [Fact(DisplayName = "CreateCareerHonor - Thất bại - Không có quyền")]
        public async Task CreateCareerHonor_Unauthorized_ReturnsForbid()
        {
            // Arrange
            var request = new CreateCareerHonorRequest { AchievementTitle = "New Honor" };

            _mockCareerHonorService
                .Setup(s => s.CreateCareerHonorAsync(request))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.CreateCareerHonor(request);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact(DisplayName = "CreateCareerHonor - Thất bại - Invalid operation")]
        public async Task CreateCareerHonor_InvalidOperation_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateCareerHonorRequest { AchievementTitle = "New Honor" };

            _mockCareerHonorService
                .Setup(s => s.CreateCareerHonorAsync(request))
                .ThrowsAsync(new InvalidOperationException("Invalid operation"));

            // Act
            var result = await _controller.CreateCareerHonor(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal("Invalid operation", apiError.Message);
        }

        [Fact(DisplayName = "CreateCareerHonor - Thất bại - Lỗi server")]
        public async Task CreateCareerHonor_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateCareerHonorRequest { AchievementTitle = "New Honor" };

            _mockCareerHonorService
                .Setup(s => s.CreateCareerHonorAsync(request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.CreateCareerHonor(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion

        #region UpdateCareerHonor Tests

        [Fact(DisplayName = "UpdateCareerHonor - Thành công")]
        public async Task UpdateCareerHonor_Success_ReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateCareerHonorRequest { AchievementTitle = "Updated Honor" };
            var expectedResult = new CareerHonorDto { Id = id, AchievementTitle = "Updated Honor" };

            _mockCareerHonorService
                .Setup(s => s.UpdateCareerHonorAsync(id, request))
                .Returns(Task.FromResult((CareerHonorDto?)expectedResult));

            // Act
            var result = await _controller.UpdateCareerHonor(id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedResult, apiSuccess.Data);
        }

        [Fact(DisplayName = "UpdateCareerHonor - Không tìm thấy")]
        public async Task UpdateCareerHonor_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateCareerHonorRequest { AchievementTitle = "Updated Honor" };

            _mockCareerHonorService
                .Setup(s => s.UpdateCareerHonorAsync(id, request))
                .Returns(Task.FromResult((CareerHonorDto?)null));

            // Act
            var result = await _controller.UpdateCareerHonor(id, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Contains("not found", apiError.Message.ToLower());
        }

        [Fact(DisplayName = "UpdateCareerHonor - Validation thất bại")]
        public async Task UpdateCareerHonor_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateCareerHonorRequest(); // Invalid data
            _controller.ModelState.AddModelError("AchievementTitle", "Required");

            // Act
            var result = await _controller.UpdateCareerHonor(id, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal("Invalid request data", apiError.Message);
        }

        [Fact(DisplayName = "UpdateCareerHonor - Thất bại - Không có quyền")]
        public async Task UpdateCareerHonor_Unauthorized_ReturnsForbid()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateCareerHonorRequest { AchievementTitle = "Updated Honor" };

            _mockCareerHonorService
                .Setup(s => s.UpdateCareerHonorAsync(id, request))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.UpdateCareerHonor(id, request);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact(DisplayName = "UpdateCareerHonor - Thất bại - Lỗi server")]
        public async Task UpdateCareerHonor_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateCareerHonorRequest { AchievementTitle = "Updated Honor" };

            _mockCareerHonorService
                .Setup(s => s.UpdateCareerHonorAsync(id, request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.UpdateCareerHonor(id, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion

        #region DeleteCareerHonor Tests

        [Fact(DisplayName = "DeleteCareerHonor - Thành công - Xóa career honor")]
        public async Task DeleteCareerHonor_Success_ReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockCareerHonorService
                .Setup(s => s.DeleteCareerHonorAsync(id))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _controller.DeleteCareerHonor(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Success", apiSuccess.Message);
        }

        [Fact(DisplayName = "DeleteCareerHonor - Thất bại - Career honor không tồn tại")]
        public async Task DeleteCareerHonor_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockCareerHonorService
                .Setup(s => s.DeleteCareerHonorAsync(id))
                .Returns(Task.FromResult(false));

            // Act
            var result = await _controller.DeleteCareerHonor(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal("Career honor not found", apiError.Message);
        }

        [Fact(DisplayName = "DeleteCareerHonor - Thất bại - Không có quyền")]
        public async Task DeleteCareerHonor_Unauthorized_ReturnsForbid()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockCareerHonorService
                .Setup(s => s.DeleteCareerHonorAsync(id))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.DeleteCareerHonor(id);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact(DisplayName = "DeleteCareerHonor - Thất bại - Lỗi server")]
        public async Task DeleteCareerHonor_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockCareerHonorService
                .Setup(s => s.DeleteCareerHonorAsync(id))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.DeleteCareerHonor(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion
    }
}

