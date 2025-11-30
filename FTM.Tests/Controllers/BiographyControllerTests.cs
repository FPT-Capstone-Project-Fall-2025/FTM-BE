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
    /// Test các API: GetBiographyDescription, UpdateBiographyDescription, GetBiographyEvents, GetBiographyEvent, CreateBiographyEvent, UpdateBiographyEvent, DeleteBiographyEvent
    /// </summary>
    public class BiographyControllerTests
    {
        private readonly Mock<IBiographyService> _mockBiographyService;
        private readonly BiographyController _controller;
        private readonly ITestOutputHelper _output;

        public BiographyControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockBiographyService = new Mock<IBiographyService>();
            _controller = new BiographyController(_mockBiographyService.Object);
        }

        #region GetBiographyDescription Tests

        [Fact(DisplayName = "GetBiographyDescription - Thành công - Trả về mô tả tiểu sử")]
        public async Task GetBiographyDescription_Success_ReturnsDescription()
        {
            // Arrange
            var expectedDescription = new BiographyDescriptionResponse { Description = "Test biography description" };

            _mockBiographyService
                .Setup(s => s.GetCurrentUserBiographyDescriptionAsync())
                .ReturnsAsync(expectedDescription);

            // Act
            var result = await _controller.GetBiographyDescription();

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedDescription, apiResponse.Data);
            Assert.Equal("Lấy mô tả tiểu sử thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "GetBiographyDescription - Thất bại - Không có quyền truy cập")]
        public async Task GetBiographyDescription_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _mockBiographyService
                .Setup(s => s.GetCurrentUserBiographyDescriptionAsync())
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetBiographyDescription();

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "GetBiographyDescription - Thất bại - Lỗi server")]
        public async Task GetBiographyDescription_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            _mockBiographyService
                .Setup(s => s.GetCurrentUserBiographyDescriptionAsync())
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetBiographyDescription();

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

        #region UpdateBiographyDescription Tests

        [Fact(DisplayName = "UpdateBiographyDescription - Thành công - Cập nhật mô tả tiểu sử")]
        public async Task UpdateBiographyDescription_Success_ReturnsUpdated()
        {
            // Arrange
            var request = new UpdateBiographyDescriptionRequest { Description = "Updated description" };
            var expectedResult = new BiographyDescriptionResponse { Description = "Updated description" };

            _mockBiographyService
                .Setup(s => s.UpdateCurrentUserBiographyDescriptionAsync(request))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.UpdateBiographyDescription(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedResult, apiResponse.Data);
            Assert.Equal("Cập nhật mô tả tiểu sử thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateBiographyDescription - Thất bại - Dữ liệu không hợp lệ")]
        public async Task UpdateBiographyDescription_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var request = new UpdateBiographyDescriptionRequest(); // Invalid data
            _controller.ModelState.AddModelError("Description", "Required");

            // Act
            var result = await _controller.UpdateBiographyDescription(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Dữ liệu không hợp lệ", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateBiographyDescription - Thất bại - Không có quyền truy cập")]
        public async Task UpdateBiographyDescription_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var request = new UpdateBiographyDescriptionRequest { Description = "Updated description" };

            _mockBiographyService
                .Setup(s => s.UpdateCurrentUserBiographyDescriptionAsync(request))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.UpdateBiographyDescription(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateBiographyDescription - Thất bại - Lỗi server")]
        public async Task UpdateBiographyDescription_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var request = new UpdateBiographyDescriptionRequest { Description = "Updated description" };

            _mockBiographyService
                .Setup(s => s.UpdateCurrentUserBiographyDescriptionAsync(request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.UpdateBiographyDescription(request);

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

        #region GetBiographyEvents Tests

        [Fact(DisplayName = "GetBiographyEvents - Thành công - Trả về danh sách sự kiện tiểu sử")]
        public async Task GetBiographyEvents_Success_ReturnsEvents()
        {
            // Arrange
            var expectedEvents = new List<BiographyEventResponse> { new BiographyEventResponse { Id = Guid.NewGuid(), Title = "Event 1" } };

            _mockBiographyService
                .Setup(s => s.GetCurrentUserBiographyEventsAsync())
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetBiographyEvents();

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedEvents, apiResponse.Data);
            Assert.Equal("Lấy danh sách tiểu sử thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "GetBiographyEvents - Thất bại - Không có quyền truy cập")]
        public async Task GetBiographyEvents_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            _mockBiographyService
                .Setup(s => s.GetCurrentUserBiographyEventsAsync())
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetBiographyEvents();

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "GetBiographyEvents - Thất bại - Lỗi server")]
        public async Task GetBiographyEvents_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            _mockBiographyService
                .Setup(s => s.GetCurrentUserBiographyEventsAsync())
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetBiographyEvents();

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

        #region GetBiographyEvent Tests

        [Fact(DisplayName = "GetBiographyEvent - Thành công - Trả về chi tiết sự kiện tiểu sử")]
        public async Task GetBiographyEvent_Success_ReturnsEvent()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var expectedEvent = new BiographyEventResponse { Id = eventId, Title = "Test Event" };

            _mockBiographyService
                .Setup(s => s.GetBiographyEventByIdAsync(eventId))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.GetBiographyEvent(eventId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedEvent, apiResponse.Data);
            Assert.Equal("Lấy thông tin sự kiện thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "GetBiographyEvent - Thất bại - Sự kiện không tồn tại")]
        public async Task GetBiographyEvent_NotFound_ReturnsNotFound()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockBiographyService
                .Setup(s => s.GetBiographyEventByIdAsync(eventId))
                .ReturnsAsync((BiographyEventResponse?)null);

            // Act
            var result = await _controller.GetBiographyEvent(eventId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không tìm thấy sự kiện tiểu sử", apiResponse.Message);
        }

        [Fact(DisplayName = "GetBiographyEvent - Thất bại - Không có quyền truy cập")]
        public async Task GetBiographyEvent_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockBiographyService
                .Setup(s => s.GetBiographyEventByIdAsync(eventId))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.GetBiographyEvent(eventId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "GetBiographyEvent - Thất bại - Lỗi server")]
        public async Task GetBiographyEvent_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockBiographyService
                .Setup(s => s.GetBiographyEventByIdAsync(eventId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetBiographyEvent(eventId);

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

        #region CreateBiographyEvent Tests

        [Fact(DisplayName = "CreateBiographyEvent - Thành công - Tạo sự kiện tiểu sử mới")]
        public async Task CreateBiographyEvent_Success_ReturnsCreated()
        {
            // Arrange
            var request = new CreateBiographyEventRequest { Title = "New Event" };
            var expectedEvent = new BiographyEventResponse { Id = Guid.NewGuid(), Title = "New Event" };

            _mockBiographyService
                .Setup(s => s.CreateBiographyEventAsync(request))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.CreateBiographyEvent(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(createdResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedEvent, apiResponse.Data);
            Assert.Equal("Tạo sự kiện tiểu sử thành công", apiResponse.Message);
            Assert.Equal(nameof(_controller.GetBiographyEvent), createdResult.ActionName);
        }

        [Fact(DisplayName = "CreateBiographyEvent - Thất bại - Dữ liệu không hợp lệ")]
        public async Task CreateBiographyEvent_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateBiographyEventRequest(); // Invalid data
            _controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await _controller.CreateBiographyEvent(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Dữ liệu không hợp lệ", apiResponse.Message);
        }

        [Fact(DisplayName = "CreateBiographyEvent - Thất bại - Không có quyền truy cập")]
        public async Task CreateBiographyEvent_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var request = new CreateBiographyEventRequest { Title = "New Event" };

            _mockBiographyService
                .Setup(s => s.CreateBiographyEventAsync(request))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.CreateBiographyEvent(request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "CreateBiographyEvent - Thất bại - Lỗi server")]
        public async Task CreateBiographyEvent_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var request = new CreateBiographyEventRequest { Title = "New Event" };

            _mockBiographyService
                .Setup(s => s.CreateBiographyEventAsync(request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.CreateBiographyEvent(request);

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

        #region UpdateBiographyEvent Tests

        [Fact(DisplayName = "UpdateBiographyEvent - Thành công - Cập nhật sự kiện tiểu sử")]
        public async Task UpdateBiographyEvent_Success_ReturnsUpdated()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var request = new UpdateBiographyEventRequest { Title = "Updated Event" };
            var expectedEvent = new BiographyEventResponse { Id = eventId, Title = "Updated Event" };

            _mockBiographyService
                .Setup(s => s.UpdateBiographyEventAsync(eventId, request))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.UpdateBiographyEvent(eventId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal(expectedEvent, apiResponse.Data);
            Assert.Equal("Cập nhật sự kiện tiểu sử thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateBiographyEvent - Thất bại - Sự kiện không tồn tại")]
        public async Task UpdateBiographyEvent_NotFound_ReturnsNotFound()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var request = new UpdateBiographyEventRequest { Title = "Updated Event" };

            _mockBiographyService
                .Setup(s => s.UpdateBiographyEventAsync(eventId, request))
                .ReturnsAsync((BiographyEventResponse?)null);

            // Act
            var result = await _controller.UpdateBiographyEvent(eventId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không tìm thấy sự kiện tiểu sử", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateBiographyEvent - Thất bại - Dữ liệu không hợp lệ")]
        public async Task UpdateBiographyEvent_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var request = new UpdateBiographyEventRequest(); // Invalid data
            _controller.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await _controller.UpdateBiographyEvent(eventId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Dữ liệu không hợp lệ", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateBiographyEvent - Thất bại - Không có quyền truy cập")]
        public async Task UpdateBiographyEvent_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var request = new UpdateBiographyEventRequest { Title = "Updated Event" };

            _mockBiographyService
                .Setup(s => s.UpdateBiographyEventAsync(eventId, request))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.UpdateBiographyEvent(eventId, request);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "UpdateBiographyEvent - Thất bại - Lỗi server")]
        public async Task UpdateBiographyEvent_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var request = new UpdateBiographyEventRequest { Title = "Updated Event" };

            _mockBiographyService
                .Setup(s => s.UpdateBiographyEventAsync(eventId, request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.UpdateBiographyEvent(eventId, request);

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

        #region DeleteBiographyEvent Tests

        [Fact(DisplayName = "DeleteBiographyEvent - Thành công - Xóa sự kiện tiểu sử")]
        public async Task DeleteBiographyEvent_Success_ReturnsOk()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockBiographyService
                .Setup(s => s.DeleteBiographyEventAsync(eventId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBiographyEvent(eventId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Status);
            Assert.Equal("Xóa sự kiện tiểu sử thành công", apiResponse.Message);
        }

        [Fact(DisplayName = "DeleteBiographyEvent - Thất bại - Sự kiện không tồn tại")]
        public async Task DeleteBiographyEvent_NotFound_ReturnsNotFound()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockBiographyService
                .Setup(s => s.DeleteBiographyEventAsync(eventId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBiographyEvent(eventId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không tìm thấy sự kiện tiểu sử", apiResponse.Message);
        }

        [Fact(DisplayName = "DeleteBiographyEvent - Thất bại - Không có quyền truy cập")]
        public async Task DeleteBiographyEvent_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockBiographyService
                .Setup(s => s.DeleteBiographyEventAsync(eventId))
                .ThrowsAsync(new UnauthorizedAccessException());

            // Act
            var result = await _controller.DeleteBiographyEvent(eventId);

            // Assert
            Assert.IsType<ActionResult<ApiResponse>>(result);
            var actionResult = (ActionResult<ApiResponse>)result;
            Assert.NotNull(actionResult.Result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.False(apiResponse.Status);
            Assert.Equal("Không có quyền truy cập", apiResponse.Message);
        }

        [Fact(DisplayName = "DeleteBiographyEvent - Thất bại - Lỗi server")]
        public async Task DeleteBiographyEvent_ServerError_ReturnsInternalServerError()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockBiographyService
                .Setup(s => s.DeleteBiographyEventAsync(eventId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.DeleteBiographyEvent(eventId);

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