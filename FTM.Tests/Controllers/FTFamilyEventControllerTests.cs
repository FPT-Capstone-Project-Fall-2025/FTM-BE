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
    /// Test các API của FTFamilyEventController:
    /// - CRUD Operations: CreateEvent, UpdateEvent, DeleteEvent, GetEventById
    /// - Query Operations: GetEventsByGP, GetUpcomingEvents, GetEventsByDateRange, GetEventsByMember, FilterEvents
    /// - Time-based Queries: GetEventsGroupedByYear, GetEventsGroupedByMonth, GetEventsGroupedByWeek
    /// - User-specific: GetMyEvents, GetMyUpcomingEvents
    /// - Member Management: AddMemberToEvent, RemoveMemberFromEvent, GetEventMembers
    /// </summary>
    public class FTFamilyEventControllerTests
    {
        private readonly Mock<IFTFamilyEventService> _mockEventService;
        private readonly FTFamilyEventController _controller;
        private readonly ITestOutputHelper _output;

        public FTFamilyEventControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockEventService = new Mock<IFTFamilyEventService>();
            _controller = new FTFamilyEventController(_mockEventService.Object);
        }

        #region CreateEvent Tests

        [Fact(DisplayName = "CreateEvent - Thành công - Tạo sự kiện mới")]
        public async Task CreateEvent_Success_ReturnsCreated()
        {
            // Arrange
            var request = new CreateFTFamilyEventRequest { Name = "New Event", StartTime = DateTimeOffset.Now };
            var expectedEvent = new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "New Event" };

            _mockEventService
                .Setup(s => s.CreateEventAsync(request))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.CreateEvent(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedEvent, apiSuccess.Data);
            Assert.Equal("Event created successfully", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - CreateEvent - Thành công - Tạo sự kiện mới");
        }

        [Fact(DisplayName = "CreateEvent - Thất bại - Lỗi server")]
        public async Task CreateEvent_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateFTFamilyEventRequest { Name = "New Event" };

            _mockEventService
                .Setup(s => s.CreateEventAsync(request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.CreateEvent(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - CreateEvent - Thất bại - Lỗi server");
        }

        #endregion

        #region UpdateEvent Tests

        [Fact(DisplayName = "UpdateEvent - Thành công - Cập nhật sự kiện")]
        public async Task UpdateEvent_Success_ReturnsUpdated()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var request = new UpdateFTFamilyEventRequest { Name = "Updated Event" };
            var expectedEvent = new FTFamilyEventDto { Id = eventId, Name = "Updated Event" };

            _mockEventService
                .Setup(s => s.UpdateEventAsync(eventId, request))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.UpdateEvent(eventId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedEvent, apiSuccess.Data);
            Assert.Equal("Event updated successfully", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - UpdateEvent - Thành công - Cập nhật sự kiện");
        }

        [Fact(DisplayName = "UpdateEvent - Thất bại - Lỗi server")]
        public async Task UpdateEvent_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var request = new UpdateFTFamilyEventRequest { Name = "Updated Event" };

            _mockEventService
                .Setup(s => s.UpdateEventAsync(eventId, request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.UpdateEvent(eventId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UpdateEvent - Thất bại - Lỗi server");
        }

        #endregion

        #region DeleteEvent Tests

        [Fact(DisplayName = "DeleteEvent - Thành công - Xóa sự kiện")]
        public async Task DeleteEvent_Success_ReturnsOk()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.DeleteEventAsync(eventId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteEvent(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Event deleted successfully", apiSuccess.Message);
            Assert.True((bool)apiSuccess.Data);

            _output.WriteLine("✅ PASSED - DeleteEvent - Thành công - Xóa sự kiện");
        }

        [Fact(DisplayName = "DeleteEvent - Thất bại - Sự kiện không tồn tại")]
        public async Task DeleteEvent_NotFound_ReturnsNotFound()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.DeleteEventAsync(eventId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteEvent(eventId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal("Event not found", apiError.Message);

            _output.WriteLine("✅ PASSED - DeleteEvent - Thất bại - Sự kiện không tồn tại");
        }

        [Fact(DisplayName = "DeleteEvent - Thất bại - Lỗi server")]
        public async Task DeleteEvent_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.DeleteEventAsync(eventId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.DeleteEvent(eventId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - DeleteEvent - Thất bại - Lỗi server");
        }

        #endregion

        #region GetEventById Tests

        [Fact(DisplayName = "GetEventById - Thành công - Trả về chi tiết sự kiện")]
        public async Task GetEventById_Success_ReturnsEvent()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var expectedEvent = new FTFamilyEventDto { Id = eventId, Name = "Test Event" };

            _mockEventService
                .Setup(s => s.GetEventByIdAsync(eventId))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.GetEventById(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedEvent, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - GetEventById - Thành công - Trả về chi tiết sự kiện");
        }

        [Fact(DisplayName = "GetEventById - Thất bại - Sự kiện không tồn tại")]
        public async Task GetEventById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.GetEventByIdAsync(eventId))
                .ReturnsAsync((FTFamilyEventDto?)null);

            // Act
            var result = await _controller.GetEventById(eventId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal("Event not found", apiError.Message);

            _output.WriteLine("✅ PASSED - GetEventById - Thất bại - Sự kiện không tồn tại");
        }

        [Fact(DisplayName = "GetEventById - Thất bại - Lỗi server")]
        public async Task GetEventById_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.GetEventByIdAsync(eventId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetEventById(eventId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - GetEventById - Thất bại - Lỗi server");
        }

        #endregion

        #region GetEventsByGP Tests

        [Fact(DisplayName = "GetEventsByGP - Thành công - Trả về danh sách sự kiện theo gia phả")]
        public async Task GetEventsByGP_Success_ReturnsEvents()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var expectedEvents = new List<FTFamilyEventDto> { new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Test Event" } };

            _mockEventService
                .Setup(s => s.GetEventsByFTIdAsync(ftId, 0, 10))
                .ReturnsAsync(expectedEvents);

            _mockEventService
                .Setup(s => s.CountEventsByFTIdAsync(ftId))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.GetEventsByGP(ftId, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Events retrieved successfully", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - GetEventsByGP - Thành công - Trả về danh sách sự kiện theo gia phả");
        }

        [Fact(DisplayName = "GetEventsByGP - Thất bại - Lỗi server")]
        public async Task GetEventsByGP_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };

            _mockEventService
                .Setup(s => s.GetEventsByFTIdAsync(ftId, 0, 10))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetEventsByGP(ftId, requestParams);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - GetEventsByGP - Thất bại - Lỗi server");
        }

        #endregion

        #region GetUpcomingEvents Tests

        [Fact(DisplayName = "GetUpcomingEvents - Thành công - Trả về sự kiện sắp tới")]
        public async Task GetUpcomingEvents_Success_ReturnsUpcomingEvents()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var expectedEvents = new List<FTFamilyEventDto> { new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Upcoming Event" } };

            _mockEventService
                .Setup(s => s.GetUpcomingEventsAsync(ftId, 30))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetUpcomingEvents(ftId, 30);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Upcoming events retrieved successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - GetUpcomingEvents - Thành công - Trả về sự kiện sắp tới");
        }

        [Fact(DisplayName = "GetUpcomingEvents - Thất bại - Lỗi server")]
        public async Task GetUpcomingEvents_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var ftId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.GetUpcomingEventsAsync(ftId, 30))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetUpcomingEvents(ftId, 30);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - GetUpcomingEvents - Thất bại - Lỗi server");
        }

        #endregion

        #region GetEventsByDateRange Tests

        [Fact(DisplayName = "GetEventsByDateRange - Thành công - Trả về sự kiện theo khoảng thời gian")]
        public async Task GetEventsByDateRange_Success_ReturnsEvents()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var startDate = DateTimeOffset.Now.AddDays(-7);
            var endDate = DateTimeOffset.Now.AddDays(7);
            var expectedEvents = new List<FTFamilyEventDto> { new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Event in range" } };

            _mockEventService
                .Setup(s => s.GetEventsByDateRangeAsync(ftId, startDate, endDate))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetEventsByDateRange(ftId, startDate, endDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Events retrieved successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - GetEventsByDateRange - Thành công - Trả về sự kiện theo khoảng thời gian");
        }

        [Fact(DisplayName = "GetEventsByDateRange - Thất bại - Lỗi server")]
        public async Task GetEventsByDateRange_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var startDate = DateTimeOffset.Now.AddDays(-7);
            var endDate = DateTimeOffset.Now.AddDays(7);

            _mockEventService
                .Setup(s => s.GetEventsByDateRangeAsync(ftId, startDate, endDate))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetEventsByDateRange(ftId, startDate, endDate);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - GetEventsByDateRange - Thất bại - Lỗi server");
        }

        #endregion

        #region GetEventsByMember Tests

        [Fact(DisplayName = "GetEventsByMember - Thành công - Trả về sự kiện theo thành viên")]
        public async Task GetEventsByMember_Success_ReturnsEvents()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var expectedEvents = new List<FTFamilyEventDto> { new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Member Event" } };

            _mockEventService
                .Setup(s => s.GetEventsByMemberIdAsync(memberId, 0, 10))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetEventsByMember(memberId, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Events retrieved successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - GetEventsByMember - Thành công - Trả về sự kiện theo thành viên");
        }

        [Fact(DisplayName = "GetEventsByMember - Thất bại - Lỗi server")]
        public async Task GetEventsByMember_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };

            _mockEventService
                .Setup(s => s.GetEventsByMemberIdAsync(memberId, 0, 10))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetEventsByMember(memberId, requestParams);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - GetEventsByMember - Thất bại - Lỗi server");
        }

        #endregion

        #region FilterEvents Tests

        [Fact(DisplayName = "FilterEvents - Thành công - Lọc sự kiện theo tiêu chí")]
        public async Task FilterEvents_Success_ReturnsFilteredEvents()
        {
            // Arrange
            var request = new EventFilterRequest { FTId = Guid.NewGuid() };
            var expectedEvents = new List<FTFamilyEventDto> { new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Filtered Event" } };

            _mockEventService
                .Setup(s => s.FilterEventsAsync(request))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.FilterEvents(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Events filtered successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - FilterEvents - Thành công - Lọc sự kiện theo tiêu chí");
        }

        [Fact(DisplayName = "FilterEvents - Thất bại - Lỗi server")]
        public async Task FilterEvents_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var request = new EventFilterRequest { FTId = Guid.NewGuid() };

            _mockEventService
                .Setup(s => s.FilterEventsAsync(request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.FilterEvents(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - FilterEvents - Thất bại - Lỗi server");
        }

        #endregion

        #region Time-based Query Tests

        [Fact(DisplayName = "GetEventsGroupedByYear - Thành công - Trả về sự kiện theo năm")]
        public async Task GetEventsGroupedByYear_Success_ReturnsEvents()
        {
            // Arrange
            var year = 2024;
            var ftId = Guid.NewGuid();
            var expectedEvents = new List<FTFamilyEventDto> { new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Year Event" } };

            _mockEventService
                .Setup(s => s.GetEventsGroupedByYearAsync(ftId, year))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetEventsGroupedByYear(year, ftId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Events in year successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - GetEventsGroupedByYear - Thành công - Trả về sự kiện theo năm");
        }

        [Fact(DisplayName = "GetEventsGroupedByMonth - Thành công - Trả về sự kiện theo tháng")]
        public async Task GetEventsGroupedByMonth_Success_ReturnsEvents()
        {
            // Arrange
            var year = 2024;
            var month = 10;
            var ftId = Guid.NewGuid();
            var expectedEvents = new List<FTFamilyEventDto> { new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Month Event" } };

            _mockEventService
                .Setup(s => s.GetEventsGroupedByMonthAsync(ftId, year, month))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetEventsGroupedByMonth(year, month, ftId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Events in month successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - GetEventsGroupedByMonth - Thành công - Trả về sự kiện theo tháng");
        }

        [Fact(DisplayName = "GetEventsGroupedByWeek - Thành công - Trả về sự kiện theo tuần")]
        public async Task GetEventsGroupedByWeek_Success_ReturnsEvents()
        {
            // Arrange
            var year = 2024;
            var month = 10;
            var week = 3;
            var ftId = Guid.NewGuid();
            var expectedEvents = new List<FTFamilyEventDto> { new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Week Event" } };

            _mockEventService
                .Setup(s => s.GetEventsGroupedByWeekAsync(ftId, year, month, week))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetEventsGroupedByWeek(year, month, week, ftId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Events in week successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - GetEventsGroupedByWeek - Thành công - Trả về sự kiện theo tuần");
        }

        #endregion

        #region Member Management Tests

        [Fact(DisplayName = "AddMemberToEvent - Thành công - Thêm thành viên vào sự kiện")]
        public async Task AddMemberToEvent_Success_ReturnsOk()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.AddMemberToEventAsync(eventId, memberId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AddMemberToEvent(eventId, memberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Member added to event successfully", apiSuccess.Message);
            Assert.True((bool)apiSuccess.Data);

            _output.WriteLine("✅ PASSED - AddMemberToEvent - Thành công - Thêm thành viên vào sự kiện");
        }

        [Fact(DisplayName = "AddMemberToEvent - Thất bại - Thành viên đã có trong sự kiện")]
        public async Task AddMemberToEvent_AlreadyInEvent_ReturnsBadRequest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.AddMemberToEventAsync(eventId, memberId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AddMemberToEvent(eventId, memberId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal("Member already in event or event not found", apiError.Message);

            _output.WriteLine("✅ PASSED - AddMemberToEvent - Thất bại - Thành viên đã có trong sự kiện");
        }

        [Fact(DisplayName = "RemoveMemberFromEvent - Thành công - Xóa thành viên khỏi sự kiện")]
        public async Task RemoveMemberFromEvent_Success_ReturnsOk()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.RemoveMemberFromEventAsync(eventId, memberId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveMemberFromEvent(eventId, memberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Member removed from event successfully", apiSuccess.Message);
            Assert.True((bool)apiSuccess.Data);

            _output.WriteLine("✅ PASSED - RemoveMemberFromEvent - Thành công - Xóa thành viên khỏi sự kiện");
        }

        [Fact(DisplayName = "RemoveMemberFromEvent - Thất bại - Thành viên không có trong sự kiện")]
        public async Task RemoveMemberFromEvent_NotInEvent_ReturnsNotFound()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.RemoveMemberFromEventAsync(eventId, memberId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveMemberFromEvent(eventId, memberId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal("Member not found in event", apiError.Message);

            _output.WriteLine("✅ PASSED - RemoveMemberFromEvent - Thất bại - Thành viên không có trong sự kiện");
        }

        [Fact(DisplayName = "GetEventMembers - Thành công - Trả về danh sách thành viên sự kiện")]
        public async Task GetEventMembers_Success_ReturnsMembers()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var expectedMembers = new List<FTFamilyEventMemberDto> { new FTFamilyEventMemberDto { Id = Guid.NewGuid(), FTMemberId = Guid.NewGuid(), MemberName = "Test Member" } };

            _mockEventService
                .Setup(s => s.GetEventMembersAsync(eventId))
                .ReturnsAsync(expectedMembers);

            // Act
            var result = await _controller.GetEventMembers(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Event members retrieved successfully", apiSuccess.Message);
            Assert.Equal(expectedMembers, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - GetEventMembers - Thành công - Trả về danh sách thành viên sự kiện");
        }

        #endregion
    }
}
