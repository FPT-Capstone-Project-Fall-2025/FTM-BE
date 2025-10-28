using FTM.API.Controllers;
using FTM.API.Reponses;
using FTM.API.Helpers;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Specification.FamilyTrees;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FTM.Tests.Controllers
{
    /// <summary>
    /// Test các API của FamilyTreeController:
    /// - Add: Tạo gia phả mới
    /// - GetById: Lấy chi tiết gia phả theo ID
    /// - Edit: Cập nhật thông tin gia phả
    /// - Delete: Xóa gia phả (soft delete)
    /// - GetAll: Lấy danh sách tất cả gia phả
    /// - GetMyFamilyTrees: Lấy danh sách gia phả của người dùng hiện tại
    /// </summary>
    public class FamilyTreeControllerTests
    {
        private readonly Mock<IFamilyTreeService> _mockFamilyTreeService;
        private readonly FamilyTreeController _controller;
        private readonly ITestOutputHelper _output;

        public FamilyTreeControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockFamilyTreeService = new Mock<IFamilyTreeService>();
            _controller = new FamilyTreeController(_mockFamilyTreeService.Object);
        }

        #region Add Tests

        [Fact(DisplayName = "Add - Thành công - Tạo gia phả mới")]
        public async Task Add_Success_ReturnsCreated()
        {
            // Arrange
            var request = new UpsertFamilyTreeRequest { Name = "New Family Tree", Description = "Test Description" };
            var expectedFamilyTree = new FamilyTreeDetailsDto { Id = Guid.NewGuid(), Name = "New Family Tree" };

            _mockFamilyTreeService
                .Setup(s => s.CreateFamilyTreeAsync(request))
                .ReturnsAsync(expectedFamilyTree);

            // Act
            var result = await _controller.Add(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedFamilyTree, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - Add - Thành công - Tạo gia phả mới");
        }

        [Fact(DisplayName = "Add - Thất bại - Dữ liệu không hợp lệ")]
        public async Task Add_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpsertFamilyTreeRequest(); // Invalid data
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.Add(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Required", apiError.Message);

            _output.WriteLine("✅ PASSED - Add - Thất bại - Dữ liệu không hợp lệ");
        }

        [Fact(DisplayName = "Add - Thất bại - ArgumentException")]
        public async Task Add_ArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpsertFamilyTreeRequest { Name = "Test Tree" };

            _mockFamilyTreeService
                .Setup(s => s.CreateFamilyTreeAsync(request))
                .ThrowsAsync(new ArgumentException("Invalid argument"));

            // Act
            var result = await _controller.Add(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal("Invalid argument", apiError.Message);

            _output.WriteLine("✅ PASSED - Add - Thất bại - ArgumentException");
        }

        [Fact(DisplayName = "Add - Thất bại - UnauthorizedAccessException")]
        public async Task Add_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var request = new UpsertFamilyTreeRequest { Name = "Test Tree" };

            _mockFamilyTreeService
                .Setup(s => s.CreateFamilyTreeAsync(request))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.Add(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(unauthorizedResult.Value);
            Assert.Equal("Unauthorized", apiError.Message);

            _output.WriteLine("✅ PASSED - Add - Thất bại - UnauthorizedAccessException");
        }

        [Fact(DisplayName = "Add - Thất bại - Lỗi server")]
        public async Task Add_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var request = new UpsertFamilyTreeRequest { Name = "Test Tree" };

            _mockFamilyTreeService
                .Setup(s => s.CreateFamilyTreeAsync(request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.Add(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusCodeResult.Value);
            Assert.Contains("Lỗi hệ thống: Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - Add - Thất bại - Lỗi server");
        }

        #endregion

        #region GetById Tests

        [Fact(DisplayName = "GetById - Thành công - Trả về chi tiết gia phả")]
        public async Task GetById_Success_ReturnsFamilyTree()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();
            var expectedFamilyTree = new FamilyTreeDetailsDto { Id = familyTreeId, Name = "Test Family Tree" };

            _mockFamilyTreeService
                .Setup(s => s.GetFamilyTreeByIdAsync(familyTreeId))
                .ReturnsAsync(expectedFamilyTree);

            // Act
            var result = await _controller.GetById(familyTreeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedFamilyTree, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - GetById - Thành công - Trả về chi tiết gia phả");
        }

        [Fact(DisplayName = "GetById - Thất bại - Gia phả không tồn tại")]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();

            _mockFamilyTreeService
                .Setup(s => s.GetFamilyTreeByIdAsync(familyTreeId))
                .ThrowsAsync(new ArgumentException("Family tree not found"));

            // Act
            var result = await _controller.GetById(familyTreeId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal("Family tree not found", apiError.Message);

            _output.WriteLine("✅ PASSED - GetById - Thất bại - Gia phả không tồn tại");
        }

        [Fact(DisplayName = "GetById - Thất bại - Lỗi server")]
        public async Task GetById_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();

            _mockFamilyTreeService
                .Setup(s => s.GetFamilyTreeByIdAsync(familyTreeId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetById(familyTreeId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusCodeResult.Value);
            Assert.Contains("Lỗi hệ thống: Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - GetById - Thất bại - Lỗi server");
        }

        #endregion

        #region Edit Tests

        [Fact(DisplayName = "Edit - Thành công - Cập nhật gia phả")]
        public async Task Edit_Success_ReturnsUpdated()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();
            var request = new UpsertFamilyTreeRequest { Name = "Updated Family Tree" };
            var expectedFamilyTree = new FamilyTreeDetailsDto { Id = familyTreeId, Name = "Updated Family Tree" };

            _mockFamilyTreeService
                .Setup(s => s.UpdateFamilyTreeAsync(familyTreeId, request))
                .ReturnsAsync(expectedFamilyTree);

            // Act
            var result = await _controller.Edit(familyTreeId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedFamilyTree, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - Edit - Thành công - Cập nhật gia phả");
        }

        [Fact(DisplayName = "Edit - Thất bại - Dữ liệu không hợp lệ")]
        public async Task Edit_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();
            var request = new UpsertFamilyTreeRequest(); // Invalid data
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.Edit(familyTreeId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Required", apiError.Message);

            _output.WriteLine("✅ PASSED - Edit - Thất bại - Dữ liệu không hợp lệ");
        }

        [Fact(DisplayName = "Edit - Thất bại - ArgumentException")]
        public async Task Edit_ArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();
            var request = new UpsertFamilyTreeRequest { Name = "Updated Tree" };

            _mockFamilyTreeService
                .Setup(s => s.UpdateFamilyTreeAsync(familyTreeId, request))
                .ThrowsAsync(new ArgumentException("Invalid argument"));

            // Act
            var result = await _controller.Edit(familyTreeId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal("Invalid argument", apiError.Message);

            _output.WriteLine("✅ PASSED - Edit - Thất bại - ArgumentException");
        }

        [Fact(DisplayName = "Edit - Thất bại - UnauthorizedAccessException")]
        public async Task Edit_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();
            var request = new UpsertFamilyTreeRequest { Name = "Updated Tree" };

            _mockFamilyTreeService
                .Setup(s => s.UpdateFamilyTreeAsync(familyTreeId, request))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.Edit(familyTreeId, request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(unauthorizedResult.Value);
            Assert.Equal("Unauthorized", apiError.Message);

            _output.WriteLine("✅ PASSED - Edit - Thất bại - UnauthorizedAccessException");
        }

        [Fact(DisplayName = "Edit - Thất bại - Lỗi server")]
        public async Task Edit_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();
            var request = new UpsertFamilyTreeRequest { Name = "Updated Tree" };

            _mockFamilyTreeService
                .Setup(s => s.UpdateFamilyTreeAsync(familyTreeId, request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.Edit(familyTreeId, request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusCodeResult.Value);
            Assert.Contains("Lỗi hệ thống: Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - Edit - Thất bại - Lỗi server");
        }

        #endregion

        #region Delete Tests

        [Fact(DisplayName = "Delete - Thành công - Xóa gia phả")]
        public async Task Delete_Success_ReturnsOk()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();

            _mockFamilyTreeService
                .Setup(s => s.DeleteFamilyTreeAsync(familyTreeId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(familyTreeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Xóa gia phả thành công", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - Delete - Thành công - Xóa gia phả");
        }

        [Fact(DisplayName = "Delete - Thất bại - Gia phả không tồn tại")]
        public async Task Delete_NotFound_ReturnsNotFound()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();

            _mockFamilyTreeService
                .Setup(s => s.DeleteFamilyTreeAsync(familyTreeId))
                .ThrowsAsync(new ArgumentException("Family tree not found"));

            // Act
            var result = await _controller.Delete(familyTreeId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal("Family tree not found", apiError.Message);

            _output.WriteLine("✅ PASSED - Delete - Thất bại - Gia phả không tồn tại");
        }

        [Fact(DisplayName = "Delete - Thất bại - UnauthorizedAccessException")]
        public async Task Delete_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();

            _mockFamilyTreeService
                .Setup(s => s.DeleteFamilyTreeAsync(familyTreeId))
                .ThrowsAsync(new UnauthorizedAccessException("Unauthorized"));

            // Act
            var result = await _controller.Delete(familyTreeId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(unauthorizedResult.Value);
            Assert.Equal("Unauthorized", apiError.Message);

            _output.WriteLine("✅ PASSED - Delete - Thất bại - UnauthorizedAccessException");
        }

        [Fact(DisplayName = "Delete - Thất bại - Lỗi server")]
        public async Task Delete_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();

            _mockFamilyTreeService
                .Setup(s => s.DeleteFamilyTreeAsync(familyTreeId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.Delete(familyTreeId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusCodeResult.Value);
            Assert.Contains("Lỗi hệ thống: Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - Delete - Thất bại - Lỗi server");
        }

        #endregion

        #region GetAll Tests

        [Fact(DisplayName = "GetAll - Thành công - Trả về danh sách gia phả")]
        public async Task GetAll_Success_ReturnsFamilyTrees()
        {
            // Arrange
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var expectedFamilyTrees = new List<FamilyTreeDataTableDto>
            {
                new FamilyTreeDataTableDto { Id = Guid.NewGuid(), Name = "Family Tree 1" },
                new FamilyTreeDataTableDto { Id = Guid.NewGuid(), Name = "Family Tree 2" }
            };

            _mockFamilyTreeService
                .Setup(s => s.GetFamilyTreesAsync(It.IsAny<FamilyTreeSpecParams>()))
                .ReturnsAsync(expectedFamilyTrees);

            _mockFamilyTreeService
                .Setup(s => s.CountFamilyTreesAsync(It.IsAny<FamilyTreeSpecParams>()))
                .ReturnsAsync(2);

            // Act
            var result = await _controller.GetAll(requestParams);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Pagination<FamilyTreeDataTableDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Lấy danh sách gia phả thành công", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - GetAll - Thành công - Trả về danh sách gia phả");
        }

        #endregion

        #region GetMyFamilyTrees Tests

        [Fact(DisplayName = "GetMyFamilyTrees - Thành công - Trả về danh sách gia phả của tôi")]
        public async Task GetMyFamilyTrees_Success_ReturnsMyFamilyTrees()
        {
            // Arrange
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var expectedFamilyTrees = new List<FamilyTreeDataTableDto>
            {
                new FamilyTreeDataTableDto { Id = Guid.NewGuid(), Name = "My Family Tree 1" },
                new FamilyTreeDataTableDto { Id = Guid.NewGuid(), Name = "My Family Tree 2" }
            };

            _mockFamilyTreeService
                .Setup(s => s.GetMyFamilyTreesAsync(It.IsAny<FamilyTreeSpecParams>()))
                .ReturnsAsync(expectedFamilyTrees);

            _mockFamilyTreeService
                .Setup(s => s.CountMyFamilyTreesAsync(It.IsAny<FamilyTreeSpecParams>()))
                .ReturnsAsync(2);

            // Act
            var result = await _controller.GetMyFamilyTrees(requestParams);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Pagination<FamilyTreeDataTableDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Lấy danh sách gia phả của tôi thành công", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - GetMyFamilyTrees - Thành công - Trả về danh sách gia phả của tôi");
        }

        #endregion
    }
}