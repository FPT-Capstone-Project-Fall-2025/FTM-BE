using FTM.API.Controllers;
using FTM.API.Reponses;
using FTM.API.Helpers;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Specification.FamilyTrees;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        #region CreateEvent Tests - POST /api/ftfamilyevent

        [Fact(DisplayName = "UTCID01 - CreateEvent - Thành công - Tạo sự kiện mới với dữ liệu hợp lệ")]
        public async Task CreateEvent_Success_ReturnsOk()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new CreateFTFamilyEventRequest 
            { 
                Name = "Sự kiện gia đình",
                EventType = 0,
                StartTime = DateTimeOffset.Now.AddDays(7),
                EndTime = DateTimeOffset.Now.AddDays(7).AddHours(2),
                FTId = ftId,
                Location = "Hà Nội",
                Description = "Mô tả sự kiện",
                IsPublic = true
            };
            var expectedEvent = new FTFamilyEventDto 
            { 
                Id = Guid.NewGuid(), 
                Name = "Sự kiện gia đình",
                FTId = ftId
            };

            _mockEventService
                .Setup(s => s.CreateEventAsync(request))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.CreateEvent(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedEvent, apiSuccess.Data);
            Assert.Equal("Event created successfully", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - UTCID01 - CreateEvent - Thành công - Tạo sự kiện mới với dữ liệu hợp lệ");
        }

        [Fact(DisplayName = "UTCID02 - CreateEvent - Thất bại - Family tree không tồn tại")]
        public async Task CreateEvent_FamilyTreeNotFound_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateFTFamilyEventRequest 
            { 
                Name = "Sự kiện",
                EventType = 0,
                StartTime = DateTimeOffset.Now,
                FTId = Guid.NewGuid()
            };

            _mockEventService
                .Setup(s => s.CreateEventAsync(request))
                .ThrowsAsync(new Exception("Family tree with ID"));

            // Act
            var result = await _controller.CreateEvent(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(apiError.Message);
            Assert.Contains("Family tree", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - CreateEvent - Thất bại - Family tree không tồn tại");
        }

        [Fact(DisplayName = "UTCID03 - CreateEvent - Thất bại - Invalid recurrence type")]
        public async Task CreateEvent_InvalidRecurrenceType_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateFTFamilyEventRequest 
            { 
                Name = "Sự kiện",
                EventType = 0,
                StartTime = DateTimeOffset.Now,
                FTId = Guid.NewGuid(),
                RecurrenceType = 5 // Invalid value
            };

            _mockEventService
                .Setup(s => s.CreateEventAsync(request))
                .ThrowsAsync(new Exception("Invalid recurrence type"));

            // Act
            var result = await _controller.CreateEvent(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Invalid recurrence type", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - CreateEvent - Thất bại - Invalid recurrence type");
        }

        [Fact(DisplayName = "UTCID04 - CreateEvent - Thất bại - Start time must be before end time")]
        public async Task CreateEvent_InvalidTimeRange_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreateFTFamilyEventRequest 
            { 
                Name = "Sự kiện",
                EventType = 0,
                StartTime = DateTimeOffset.Now.AddDays(7),
                EndTime = DateTimeOffset.Now, // End time before start time
                FTId = Guid.NewGuid()
            };

            _mockEventService
                .Setup(s => s.CreateEventAsync(request))
                .ThrowsAsync(new Exception("Start time must be before end time"));

            // Act
            var result = await _controller.CreateEvent(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Start time must be before end time", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - CreateEvent - Thất bại - Start time must be before end time");
        }

        #endregion

        #region UpdateEvent Tests - PUT /api/ftfamilyevent/{id}

        [Fact(DisplayName = "UTCID01 - UpdateEvent - Thành công - Cập nhật sự kiện với dữ liệu hợp lệ")]
        public async Task UpdateEvent_Success_ReturnsOk()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var request = new UpdateFTFamilyEventRequest 
            { 
                Name = "Sự kiện đã cập nhật",
                Location = "TP.HCM",
                Description = "Mô tả mới"
            };
            var expectedEvent = new FTFamilyEventDto 
            { 
                Id = eventId, 
                Name = "Sự kiện đã cập nhật"
            };

            _mockEventService
                .Setup(s => s.UpdateEventAsync(eventId, request))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.UpdateEvent(eventId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedEvent, apiSuccess.Data);
            Assert.Equal("Event updated successfully", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - UTCID01 - UpdateEvent - Thành công - Cập nhật sự kiện với dữ liệu hợp lệ");
        }

        [Fact(DisplayName = "UTCID02 - UpdateEvent - Thất bại - Sự kiện không tồn tại")]
        public async Task UpdateEvent_EventNotFound_ReturnsBadRequest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var request = new UpdateFTFamilyEventRequest { Name = "Updated Event" };

            _mockEventService
                .Setup(s => s.UpdateEventAsync(eventId, request))
                .ThrowsAsync(new Exception("Event not found"));

            // Act
            var result = await _controller.UpdateEvent(eventId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Event not found", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - UpdateEvent - Thất bại - Sự kiện không tồn tại");
        }

        [Fact(DisplayName = "UTCID03 - UpdateEvent - Thất bại - Invalid recurrence type")]
        public async Task UpdateEvent_InvalidRecurrenceType_ReturnsBadRequest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var request = new UpdateFTFamilyEventRequest 
            { 
                Name = "Updated Event",
                RecurrenceType = 5 // Invalid value
            };

            _mockEventService
                .Setup(s => s.UpdateEventAsync(eventId, request))
                .ThrowsAsync(new Exception("Invalid recurrence type"));

            // Act
            var result = await _controller.UpdateEvent(eventId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Invalid recurrence type", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - UpdateEvent - Thất bại - Invalid recurrence type");
        }

        [Fact(DisplayName = "UTCID04 - UpdateEvent - Thất bại - Start time must be before end time")]
        public async Task UpdateEvent_InvalidTimeRange_ReturnsBadRequest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var request = new UpdateFTFamilyEventRequest 
            { 
                Name = "Updated Event",
                StartTime = DateTimeOffset.Now.AddDays(7),
                EndTime = DateTimeOffset.Now // End time before start time
            };

            _mockEventService
                .Setup(s => s.UpdateEventAsync(eventId, request))
                .ThrowsAsync(new Exception("Start time must be before end time"));

            // Act
            var result = await _controller.UpdateEvent(eventId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Start time must be before end time", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - UpdateEvent - Thất bại - Start time must be before end time");
        }

        #endregion

        #region DeleteEvent Tests - DELETE /api/ftfamilyevent/{id}

        [Fact(DisplayName = "UTCID01 - DeleteEvent - Thành công - Xóa sự kiện")]
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
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Event deleted successfully", apiSuccess.Message);
            Assert.True((bool)apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - DeleteEvent - Thành công - Xóa sự kiện");
        }

        [Fact(DisplayName = "UTCID02 - DeleteEvent - Thất bại - Sự kiện không tồn tại")]
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
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Event not found", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - DeleteEvent - Thất bại - Sự kiện không tồn tại");
        }

        [Fact(DisplayName = "UTCID03 - DeleteEvent - Thất bại - Lỗi server")]
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
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - DeleteEvent - Thất bại - Lỗi server");
        }

        #endregion

        #region GetEventById Tests - GET /api/ftfamilyevent/event/{id}

        [Fact(DisplayName = "UTCID01 - GetEventById - Thành công - Trả về chi tiết sự kiện")]
        public async Task GetEventById_Success_ReturnsEvent()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var expectedEvent = new FTFamilyEventDto 
            { 
                Id = eventId, 
                Name = "Sự kiện gia đình",
                StartTime = DateTimeOffset.Now.AddDays(7),
                Location = "Hà Nội"
            };

            _mockEventService
                .Setup(s => s.GetEventByIdAsync(eventId))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _controller.GetEventById(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedEvent, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetEventById - Thành công - Trả về chi tiết sự kiện");
        }

        [Fact(DisplayName = "UTCID02 - GetEventById - Thất bại - Sự kiện không tồn tại")]
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
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Event not found", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - GetEventById - Thất bại - Sự kiện không tồn tại");
        }

        [Fact(DisplayName = "UTCID03 - GetEventById - Thất bại - Lỗi server")]
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
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetEventById - Thất bại - Lỗi server");
        }

        #endregion

        #region GetEventsByGP Tests - GET /api/ftfamilyevent/by-gp/{FTId}

        [Fact(DisplayName = "UTCID01 - GetEventsByGP - Thành công - Trả về danh sách sự kiện theo gia phả")]
        public async Task GetEventsByGP_Success_ReturnsEvents()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện 1", FTId = ftId },
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện 2", FTId = ftId }
            };

            _mockEventService
                .Setup(s => s.GetEventsByFTIdAsync(ftId, 0, 10))
                .ReturnsAsync(expectedEvents);

            _mockEventService
                .Setup(s => s.CountEventsByFTIdAsync(ftId))
                .ReturnsAsync(2);

            // Act
            var result = await _controller.GetEventsByGP(ftId, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Events retrieved successfully", apiSuccess.Message);
            var pagination = Assert.IsType<Pagination<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Equal(2, pagination.TotalItems);

            _output.WriteLine("✅ PASSED - UTCID01 - GetEventsByGP - Thành công - Trả về danh sách sự kiện theo gia phả");
        }

        [Fact(DisplayName = "UTCID02 - GetEventsByGP - Thành công - Trả về danh sách rỗng")]
        public async Task GetEventsByGP_Success_ReturnsEmptyList()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var emptyEvents = new List<FTFamilyEventDto>();

            _mockEventService
                .Setup(s => s.GetEventsByFTIdAsync(ftId, 0, 10))
                .ReturnsAsync(emptyEvents);

            _mockEventService
                .Setup(s => s.CountEventsByFTIdAsync(ftId))
                .ReturnsAsync(0);

            // Act
            var result = await _controller.GetEventsByGP(ftId, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var pagination = Assert.IsType<Pagination<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Equal(0, pagination.TotalItems);
            Assert.Empty(pagination.Data);

            _output.WriteLine("✅ PASSED - UTCID02 - GetEventsByGP - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetEventsByGP - Thất bại - Lỗi server")]
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
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetEventsByGP - Thất bại - Lỗi server");
        }

        [Fact(DisplayName = "UTCID04 - GetEventsByGP - Boundary - Pagination page 2")]
        public async Task GetEventsByGP_Boundary_Page2()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 2, PageSize = 5 };
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện trang 2", FTId = ftId }
            };

            _mockEventService
                .Setup(s => s.GetEventsByFTIdAsync(ftId, 5, 5))
                .ReturnsAsync(expectedEvents);

            _mockEventService
                .Setup(s => s.CountEventsByFTIdAsync(ftId))
                .ReturnsAsync(10);

            // Act
            var result = await _controller.GetEventsByGP(ftId, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var pagination = Assert.IsType<Pagination<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Equal(10, pagination.TotalItems);
            Assert.Equal(2, pagination.PageIndex);
            Assert.Equal(5, pagination.PageSize);

            _output.WriteLine("✅ PASSED - UTCID04 - GetEventsByGP - Boundary - Pagination page 2");
        }

        #endregion

        #region GetUpcomingEvents Tests - GET /api/ftfamilyevent/upcoming

        [Fact(DisplayName = "UTCID01 - GetUpcomingEvents - Thành công - Trả về sự kiện sắp tới")]
        public async Task GetUpcomingEvents_Success_ReturnsUpcomingEvents()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện sắp tới", StartTime = DateTimeOffset.Now.AddDays(5) }
            };

            _mockEventService
                .Setup(s => s.GetUpcomingEventsAsync(ftId, 30))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetUpcomingEvents(ftId, 30);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Upcoming events retrieved successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetUpcomingEvents - Thành công - Trả về sự kiện sắp tới");
        }

        [Fact(DisplayName = "UTCID02 - GetUpcomingEvents - Thành công - Trả về danh sách rỗng")]
        public async Task GetUpcomingEvents_Success_ReturnsEmptyList()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var emptyEvents = new List<FTFamilyEventDto>();

            _mockEventService
                .Setup(s => s.GetUpcomingEventsAsync(ftId, 30))
                .ReturnsAsync(emptyEvents);

            // Act
            var result = await _controller.GetUpcomingEvents(ftId, 30);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Empty(events);

            _output.WriteLine("✅ PASSED - UTCID02 - GetUpcomingEvents - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetUpcomingEvents - Thất bại - Lỗi server")]
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
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetUpcomingEvents - Thất bại - Lỗi server");
        }

        [Fact(DisplayName = "UTCID04 - GetUpcomingEvents - Boundary - Days parameter là 7")]
        public async Task GetUpcomingEvents_Boundary_Days7()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var days = 7;
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện trong 7 ngày", StartTime = DateTimeOffset.Now.AddDays(3) }
            };

            _mockEventService
                .Setup(s => s.GetUpcomingEventsAsync(ftId, days))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetUpcomingEvents(ftId, days);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Single(events);

            _output.WriteLine("✅ PASSED - UTCID04 - GetUpcomingEvents - Boundary - Days parameter là 7");
        }

        #endregion

        #region GetEventsByDateRange Tests - GET /api/ftfamilyevent/by-date

        [Fact(DisplayName = "UTCID01 - GetEventsByDateRange - Thành công - Trả về sự kiện theo khoảng thời gian")]
        public async Task GetEventsByDateRange_Success_ReturnsEvents()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var startDate = DateTimeOffset.Now.AddDays(-7);
            var endDate = DateTimeOffset.Now.AddDays(7);
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện trong khoảng", StartTime = DateTimeOffset.Now }
            };

            _mockEventService
                .Setup(s => s.GetEventsByDateRangeAsync(ftId, startDate, endDate))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetEventsByDateRange(ftId, startDate, endDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Events retrieved successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetEventsByDateRange - Thành công - Trả về sự kiện theo khoảng thời gian");
        }

        [Fact(DisplayName = "UTCID02 - GetEventsByDateRange - Thành công - Trả về danh sách rỗng")]
        public async Task GetEventsByDateRange_Success_ReturnsEmptyList()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var startDate = DateTimeOffset.Now.AddDays(30);
            var endDate = DateTimeOffset.Now.AddDays(37);
            var emptyEvents = new List<FTFamilyEventDto>();

            _mockEventService
                .Setup(s => s.GetEventsByDateRangeAsync(ftId, startDate, endDate))
                .ReturnsAsync(emptyEvents);

            // Act
            var result = await _controller.GetEventsByDateRange(ftId, startDate, endDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Empty(events);

            _output.WriteLine("✅ PASSED - UTCID02 - GetEventsByDateRange - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetEventsByDateRange - Thất bại - Lỗi server")]
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
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetEventsByDateRange - Thất bại - Lỗi server");
        }

        [Fact(DisplayName = "UTCID04 - GetEventsByDateRange - Boundary - StartDate bằng EndDate")]
        public async Task GetEventsByDateRange_Boundary_SameStartAndEndDate()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var sameDate = DateTimeOffset.Now;
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện trong ngày", StartTime = sameDate }
            };

            _mockEventService
                .Setup(s => s.GetEventsByDateRangeAsync(ftId, sameDate, sameDate))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetEventsByDateRange(ftId, sameDate, sameDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Single(events);

            _output.WriteLine("✅ PASSED - UTCID04 - GetEventsByDateRange - Boundary - StartDate bằng EndDate");
        }

        #endregion

        #region GetEventsByMember Tests - GET /api/ftfamilyevent/by-member/{memberId}

        [Fact(DisplayName = "UTCID01 - GetEventsByMember - Thành công - Trả về sự kiện theo thành viên")]
        public async Task GetEventsByMember_Success_ReturnsEvents()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện thành viên", TargetMemberId = memberId }
            };

            _mockEventService
                .Setup(s => s.GetEventsByMemberIdAsync(memberId, 0, 10))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetEventsByMember(memberId, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Events retrieved successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetEventsByMember - Thành công - Trả về sự kiện theo thành viên");
        }

        [Fact(DisplayName = "UTCID02 - GetEventsByMember - Thành công - Trả về danh sách rỗng")]
        public async Task GetEventsByMember_Success_ReturnsEmptyList()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var emptyEvents = new List<FTFamilyEventDto>();

            _mockEventService
                .Setup(s => s.GetEventsByMemberIdAsync(memberId, 0, 10))
                .ReturnsAsync(emptyEvents);

            // Act
            var result = await _controller.GetEventsByMember(memberId, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Empty(events);

            _output.WriteLine("✅ PASSED - UTCID02 - GetEventsByMember - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetEventsByMember - Thất bại - Lỗi server")]
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
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetEventsByMember - Thất bại - Lỗi server");
        }

        [Fact(DisplayName = "UTCID04 - GetEventsByMember - Boundary - Pagination với page size lớn")]
        public async Task GetEventsByMember_Boundary_LargePageSize()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 100 };
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện 1", TargetMemberId = memberId },
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện 2", TargetMemberId = memberId }
            };

            _mockEventService
                .Setup(s => s.GetEventsByMemberIdAsync(memberId, 0, 100))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetEventsByMember(memberId, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Equal(2, events.Count());

            _output.WriteLine("✅ PASSED - UTCID04 - GetEventsByMember - Boundary - Pagination với page size lớn");
        }

        #endregion

        #region FilterEvents Tests - POST /api/ftfamilyevent/filter

        [Fact(DisplayName = "UTCID01 - FilterEvents - Thành công - Lọc sự kiện theo tiêu chí")]
        public async Task FilterEvents_Success_ReturnsFilteredEvents()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new EventFilterRequest 
            { 
                FTId = ftId,
                StartDate = DateTimeOffset.Now.AddDays(-30),
                EndDate = DateTimeOffset.Now.AddDays(30),
                EventType = "Birthday"
            };
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện đã lọc", FTId = ftId }
            };

            _mockEventService
                .Setup(s => s.FilterEventsAsync(request))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.FilterEvents(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Events filtered successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - FilterEvents - Thành công - Lọc sự kiện theo tiêu chí");
        }

        [Fact(DisplayName = "UTCID02 - FilterEvents - Thành công - Trả về danh sách rỗng")]
        public async Task FilterEvents_Success_ReturnsEmptyList()
        {
            // Arrange
            var request = new EventFilterRequest 
            { 
                FTId = Guid.NewGuid(),
                EventType = "NonExistentType"
            };
            var emptyEvents = new List<FTFamilyEventDto>();

            _mockEventService
                .Setup(s => s.FilterEventsAsync(request))
                .ReturnsAsync(emptyEvents);

            // Act
            var result = await _controller.FilterEvents(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Empty(events);

            _output.WriteLine("✅ PASSED - UTCID02 - FilterEvents - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - FilterEvents - Thất bại - Lỗi server")]
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
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - FilterEvents - Thất bại - Lỗi server");
        }

        [Fact(DisplayName = "UTCID04 - FilterEvents - Boundary - Filter với nhiều tiêu chí")]
        public async Task FilterEvents_Boundary_MultipleCriteria()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var request = new EventFilterRequest 
            { 
                FTId = ftId,
                StartDate = DateTimeOffset.Now.AddDays(-30),
                EndDate = DateTimeOffset.Now.AddDays(30),
                EventType = "Birthday",
                FTMemberId = memberId,
                IsLunar = false
            };
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện đã lọc", FTId = ftId, TargetMemberId = memberId }
            };

            _mockEventService
                .Setup(s => s.FilterEventsAsync(request))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.FilterEvents(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Single(events);

            _output.WriteLine("✅ PASSED - UTCID04 - FilterEvents - Boundary - Filter với nhiều tiêu chí");
        }

        #endregion

        #region GetEventsGroupedByYear Tests - GET /api/ftfamilyevent/{year}

        [Fact(DisplayName = "UTCID01 - GetEventsGroupedByYear - Thành công - Trả về sự kiện theo năm")]
        public async Task GetEventsGroupedByYear_Success_ReturnsEvents()
        {
            // Arrange
            var year = 2024;
            var ftId = Guid.NewGuid();
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện năm 2024", StartTime = new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.Zero) }
            };

            _mockEventService
                .Setup(s => s.GetEventsGroupedByYearAsync(ftId, year))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetEventsGroupedByYear(year, ftId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Events in year successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetEventsGroupedByYear - Thành công - Trả về sự kiện theo năm");
        }

        [Fact(DisplayName = "UTCID02 - GetEventsGroupedByYear - Thành công - Trả về danh sách rỗng")]
        public async Task GetEventsGroupedByYear_Success_ReturnsEmptyList()
        {
            // Arrange
            var year = 2025;
            var ftId = Guid.NewGuid();
            var emptyEvents = new List<FTFamilyEventDto>();

            _mockEventService
                .Setup(s => s.GetEventsGroupedByYearAsync(ftId, year))
                .ReturnsAsync(emptyEvents);

            // Act
            var result = await _controller.GetEventsGroupedByYear(year, ftId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<List<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Empty(events);

            _output.WriteLine("✅ PASSED - UTCID02 - GetEventsGroupedByYear - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetEventsGroupedByYear - Thất bại - Lỗi server")]
        public async Task GetEventsGroupedByYear_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var year = 2024;
            var ftId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.GetEventsGroupedByYearAsync(ftId, year))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetEventsGroupedByYear(year, ftId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetEventsGroupedByYear - Thất bại - Lỗi server");
        }

        #endregion

        #region GetEventsGroupedByMonth Tests - GET /api/ftfamilyevent/{year}/{month}

        [Fact(DisplayName = "UTCID01 - GetEventsGroupedByMonth - Thành công - Trả về sự kiện theo tháng")]
        public async Task GetEventsGroupedByMonth_Success_ReturnsEvents()
        {
            // Arrange
            var year = 2024;
            var month = 10;
            var ftId = Guid.NewGuid();
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện tháng 10/2024", StartTime = new DateTimeOffset(2024, 10, 15, 10, 0, 0, TimeSpan.Zero) }
            };

            _mockEventService
                .Setup(s => s.GetEventsGroupedByMonthAsync(ftId, year, month))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetEventsGroupedByMonth(year, month, ftId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Events in month successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetEventsGroupedByMonth - Thành công - Trả về sự kiện theo tháng");
        }

        [Fact(DisplayName = "UTCID02 - GetEventsGroupedByMonth - Thành công - Trả về danh sách rỗng")]
        public async Task GetEventsGroupedByMonth_Success_ReturnsEmptyList()
        {
            // Arrange
            var year = 2025;
            var month = 1;
            var ftId = Guid.NewGuid();
            var emptyEvents = new List<FTFamilyEventDto>();

            _mockEventService
                .Setup(s => s.GetEventsGroupedByMonthAsync(ftId, year, month))
                .ReturnsAsync(emptyEvents);

            // Act
            var result = await _controller.GetEventsGroupedByMonth(year, month, ftId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<List<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Empty(events);

            _output.WriteLine("✅ PASSED - UTCID02 - GetEventsGroupedByMonth - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetEventsGroupedByMonth - Thất bại - Lỗi server")]
        public async Task GetEventsGroupedByMonth_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var year = 2024;
            var month = 10;
            var ftId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.GetEventsGroupedByMonthAsync(ftId, year, month))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetEventsGroupedByMonth(year, month, ftId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetEventsGroupedByMonth - Thất bại - Lỗi server");
        }

        #endregion

        #region GetEventsGroupedByWeek Tests - GET /api/ftfamilyevent/{year}/{month}/{week}

        [Fact(DisplayName = "UTCID01 - GetEventsGroupedByWeek - Thành công - Trả về sự kiện theo tuần")]
        public async Task GetEventsGroupedByWeek_Success_ReturnsEvents()
        {
            // Arrange
            var year = 2024;
            var month = 10;
            var week = 3;
            var ftId = Guid.NewGuid();
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Sự kiện tuần 3 tháng 10/2024", StartTime = new DateTimeOffset(2024, 10, 15, 10, 0, 0, TimeSpan.Zero) }
            };

            _mockEventService
                .Setup(s => s.GetEventsGroupedByWeekAsync(ftId, year, month, week))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetEventsGroupedByWeek(year, month, week, ftId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Events in week successfully", apiSuccess.Message);
            Assert.Equal(expectedEvents, apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetEventsGroupedByWeek - Thành công - Trả về sự kiện theo tuần");
        }

        [Fact(DisplayName = "UTCID02 - GetEventsGroupedByWeek - Thành công - Trả về danh sách rỗng")]
        public async Task GetEventsGroupedByWeek_Success_ReturnsEmptyList()
        {
            // Arrange
            var year = 2025;
            var month = 1;
            var week = 1;
            var ftId = Guid.NewGuid();
            var emptyEvents = new List<FTFamilyEventDto>();

            _mockEventService
                .Setup(s => s.GetEventsGroupedByWeekAsync(ftId, year, month, week))
                .ReturnsAsync(emptyEvents);

            // Act
            var result = await _controller.GetEventsGroupedByWeek(year, month, week, ftId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<List<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Empty(events);

            _output.WriteLine("✅ PASSED - UTCID02 - GetEventsGroupedByWeek - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetEventsGroupedByWeek - Thất bại - Lỗi server")]
        public async Task GetEventsGroupedByWeek_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var year = 2024;
            var month = 10;
            var week = 3;
            var ftId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.GetEventsGroupedByWeekAsync(ftId, year, month, week))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetEventsGroupedByWeek(year, month, week, ftId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetEventsGroupedByWeek - Thất bại - Lỗi server");
        }

        #endregion

        #region GetMyEvents Tests - GET /api/ftfamilyevent/my-events

        [Fact(DisplayName = "UTCID01 - GetMyEvents - Thành công - Trả về sự kiện của user")]
        public async Task GetMyEvents_Success_ReturnsEvents()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "My Event 1", FTId = ftId },
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "My Event 2", FTId = ftId }
            };

            // Mock User claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.GetMyEventsAsync(userId, ftId, 0, requestParams.PageSize))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetMyEvents(ftId, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("My events retrieved successfully", apiSuccess.Message);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Equal(2, events.Count());

            _output.WriteLine("✅ PASSED - UTCID01 - GetMyEvents - Thành công - Trả về sự kiện của user");
        }

        [Fact(DisplayName = "UTCID02 - GetMyEvents - Thành công - Trả về danh sách rỗng")]
        public async Task GetMyEvents_Success_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var emptyEvents = new List<FTFamilyEventDto>();

            // Mock User claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.GetMyEventsAsync(userId, ftId, 0, requestParams.PageSize))
                .ReturnsAsync(emptyEvents);

            // Act
            var result = await _controller.GetMyEvents(ftId, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Empty(events);

            _output.WriteLine("✅ PASSED - UTCID02 - GetMyEvents - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetMyEvents - Thất bại - Lỗi server")]
        public async Task GetMyEvents_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };

            // Mock User claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.GetMyEventsAsync(userId, ftId, 0, requestParams.PageSize))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetMyEvents(ftId, requestParams);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetMyEvents - Thất bại - Lỗi server");
        }

        [Fact(DisplayName = "UTCID04 - GetMyEvents - Boundary - Pagination page 2")]
        public async Task GetMyEvents_Boundary_Page2()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 2, PageSize = 5 };
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "My Event Page 2", FTId = ftId }
            };

            // Mock User claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.GetMyEventsAsync(userId, ftId, 5, requestParams.PageSize))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetMyEvents(ftId, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Single(events);

            _output.WriteLine("✅ PASSED - UTCID04 - GetMyEvents - Boundary - Pagination page 2");
        }

        #endregion

        #region GetMyUpcomingEvents Tests - GET /api/ftfamilyevent/my-upcoming-events

        [Fact(DisplayName = "UTCID01 - GetMyUpcomingEvents - Thành công - Trả về sự kiện sắp tới của user")]
        public async Task GetMyUpcomingEvents_Success_ReturnsEvents()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var days = 30;
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Upcoming Event 1", FTId = ftId, StartTime = DateTimeOffset.Now.AddDays(5) },
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "Upcoming Event 2", FTId = ftId, StartTime = DateTimeOffset.Now.AddDays(10) }
            };

            // Mock User claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.GetMyUpcomingEventsAsync(userId, ftId, days))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetMyUpcomingEvents(ftId, days);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("My upcoming events retrieved successfully", apiSuccess.Message);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Equal(2, events.Count());

            _output.WriteLine("✅ PASSED - UTCID01 - GetMyUpcomingEvents - Thành công - Trả về sự kiện sắp tới của user");
        }

        [Fact(DisplayName = "UTCID02 - GetMyUpcomingEvents - Thành công - Trả về danh sách rỗng")]
        public async Task GetMyUpcomingEvents_Success_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var days = 30;
            var emptyEvents = new List<FTFamilyEventDto>();

            // Mock User claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.GetMyUpcomingEventsAsync(userId, ftId, days))
                .ReturnsAsync(emptyEvents);

            // Act
            var result = await _controller.GetMyUpcomingEvents(ftId, days);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Empty(events);

            _output.WriteLine("✅ PASSED - UTCID02 - GetMyUpcomingEvents - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetMyUpcomingEvents - Thất bại - Lỗi server")]
        public async Task GetMyUpcomingEvents_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var days = 30;

            // Mock User claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.GetMyUpcomingEventsAsync(userId, ftId, days))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetMyUpcomingEvents(ftId, days);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetMyUpcomingEvents - Thất bại - Lỗi server");
        }

        [Fact(DisplayName = "UTCID04 - GetMyUpcomingEvents - Boundary - Days parameter là 7")]
        public async Task GetMyUpcomingEvents_Boundary_Days7()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var days = 7;
            var expectedEvents = new List<FTFamilyEventDto> 
            { 
                new FTFamilyEventDto { Id = Guid.NewGuid(), Name = "My Upcoming Event", FTId = ftId, StartTime = DateTimeOffset.Now.AddDays(3) }
            };

            // Mock User claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.GetMyUpcomingEventsAsync(userId, ftId, days))
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _controller.GetMyUpcomingEvents(ftId, days);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var events = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventDto>>(apiSuccess.Data);
            Assert.Single(events);

            _output.WriteLine("✅ PASSED - UTCID04 - GetMyUpcomingEvents - Boundary - Days parameter là 7");
        }

        #endregion

        #region AddMemberToEvent Tests - POST /api/ftfamilyevent/{eventId}/add-member/{memberId}

        [Fact(DisplayName = "UTCID01 - AddMemberToEvent - Thành công - Thêm thành viên vào sự kiện")]
        public async Task AddMemberToEvent_Success_ReturnsOk()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            // Mock User claims
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.AddMemberToEventAsync(eventId, memberId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AddMemberToEvent(eventId, memberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Member added to event successfully", apiSuccess.Message);
            Assert.True((bool)apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - AddMemberToEvent - Thành công - Thêm thành viên vào sự kiện");
        }

        [Fact(DisplayName = "UTCID02 - AddMemberToEvent - Thất bại - Thành viên đã có trong sự kiện")]
        public async Task AddMemberToEvent_AlreadyInEvent_ReturnsBadRequest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            // Mock User claims
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.AddMemberToEventAsync(eventId, memberId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.AddMemberToEvent(eventId, memberId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Member already in event or event not found", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - AddMemberToEvent - Thất bại - Thành viên đã có trong sự kiện");
        }

        [Fact(DisplayName = "UTCID03 - AddMemberToEvent - Thất bại - Lỗi server")]
        public async Task AddMemberToEvent_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            // Mock User claims
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.AddMemberToEventAsync(eventId, memberId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.AddMemberToEvent(eventId, memberId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - AddMemberToEvent - Thất bại - Lỗi server");
        }

        #endregion

        #region RemoveMemberFromEvent Tests - DELETE /api/ftfamilyevent/{eventId}/remove-member/{memberId}

        [Fact(DisplayName = "UTCID01 - RemoveMemberFromEvent - Thành công - Xóa thành viên khỏi sự kiện")]
        public async Task RemoveMemberFromEvent_Success_ReturnsOk()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            // Mock User claims
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.RemoveMemberFromEventAsync(eventId, memberId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveMemberFromEvent(eventId, memberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Member removed from event successfully", apiSuccess.Message);
            Assert.True((bool)apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - RemoveMemberFromEvent - Thành công - Xóa thành viên khỏi sự kiện");
        }

        [Fact(DisplayName = "UTCID02 - RemoveMemberFromEvent - Thất bại - Thành viên không có trong sự kiện")]
        public async Task RemoveMemberFromEvent_NotInEvent_ReturnsNotFound()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            // Mock User claims
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.RemoveMemberFromEventAsync(eventId, memberId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.RemoveMemberFromEvent(eventId, memberId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Member not found in event", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - RemoveMemberFromEvent - Thất bại - Thành viên không có trong sự kiện");
        }

        [Fact(DisplayName = "UTCID03 - RemoveMemberFromEvent - Thất bại - Lỗi server")]
        public async Task RemoveMemberFromEvent_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            // Mock User claims
            var userId = Guid.NewGuid();
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockEventService
                .Setup(s => s.RemoveMemberFromEventAsync(eventId, memberId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.RemoveMemberFromEvent(eventId, memberId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - RemoveMemberFromEvent - Thất bại - Lỗi server");
        }

        #endregion

        #region GetEventMembers Tests - GET /api/ftfamilyevent/{eventId}/members

        [Fact(DisplayName = "UTCID01 - GetEventMembers - Thành công - Trả về danh sách thành viên sự kiện")]
        public async Task GetEventMembers_Success_ReturnsMembers()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var expectedMembers = new List<FTFamilyEventMemberDto> 
            { 
                new FTFamilyEventMemberDto { Id = Guid.NewGuid(), FTMemberId = Guid.NewGuid(), MemberName = "Test Member 1" },
                new FTFamilyEventMemberDto { Id = Guid.NewGuid(), FTMemberId = Guid.NewGuid(), MemberName = "Test Member 2" }
            };

            _mockEventService
                .Setup(s => s.GetEventMembersAsync(eventId))
                .ReturnsAsync(expectedMembers);

            // Act
            var result = await _controller.GetEventMembers(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Event members retrieved successfully", apiSuccess.Message);
            var members = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventMemberDto>>(apiSuccess.Data);
            Assert.Equal(2, members.Count());

            _output.WriteLine("✅ PASSED - UTCID01 - GetEventMembers - Thành công - Trả về danh sách thành viên sự kiện");
        }

        [Fact(DisplayName = "UTCID02 - GetEventMembers - Thành công - Trả về danh sách rỗng")]
        public async Task GetEventMembers_Success_ReturnsEmptyList()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var emptyMembers = new List<FTFamilyEventMemberDto>();

            _mockEventService
                .Setup(s => s.GetEventMembersAsync(eventId))
                .ReturnsAsync(emptyMembers);

            // Act
            var result = await _controller.GetEventMembers(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var members = Assert.IsAssignableFrom<IEnumerable<FTFamilyEventMemberDto>>(apiSuccess.Data);
            Assert.Empty(members);

            _output.WriteLine("✅ PASSED - UTCID02 - GetEventMembers - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetEventMembers - Thất bại - Lỗi server")]
        public async Task GetEventMembers_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            _mockEventService
                .Setup(s => s.GetEventMembersAsync(eventId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetEventMembers(eventId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetEventMembers - Thất bại - Lỗi server");
        }

        #endregion
    }
}
