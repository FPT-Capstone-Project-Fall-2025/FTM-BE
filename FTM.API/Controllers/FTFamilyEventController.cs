using FTM.API.Helpers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FTFamilyEventController : ControllerBase
    {
        private readonly IFTFamilyEventService _eventService;

        public FTFamilyEventController(IFTFamilyEventService eventService)
        {
            _eventService = eventService;
        }

        #region CRUD Operations

        /// <summary>
        /// Create a new family event
        /// </summary>
        [HttpPost]
        //[Authorize]
        [FTAuthorize(MethodType.ADD, FeatureType.EVENT)]
        public async Task<IActionResult> CreateEvent([FromForm] CreateFTFamilyEventRequest request)
        {
            try
            {
                var result = await _eventService.CreateEventAsync(request);
                return Ok(new ApiSuccess("Tạo sự kiện thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi tạo sự kiện"));
            }
        }

        /// <summary>
        /// Update an existing event
        /// </summary>
        [HttpPut("{id}")]
        //[Authorize]
        [FTAuthorize(MethodType.UPDATE, FeatureType.EVENT)]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromForm] UpdateFTFamilyEventRequest request)
        {
            try
            {
                var result = await _eventService.UpdateEventAsync(id, request);
                return Ok(new ApiSuccess("Cập nhật sự kiện thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi cập nhật sự kiện"));
            }
        }

        /// <summary>
        /// Delete an event
        /// </summary>
        [HttpDelete("{id}")]
        [FTAuthorize(MethodType.DELETE, FeatureType.EVENT)]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            try
            {
                var result = await _eventService.DeleteEventAsync(id);
                if (!result)
                    return NotFound(new ApiError("Không tìm thấy sự kiện"));

                return Ok(new ApiSuccess("Xóa sự kiện thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi xóa sự kiện"));
            }
        }

        /// <summary>
        /// Get event by ID
        /// </summary>
        [HttpGet("event/{id:guid}")]
        [FTAuthorize(MethodType.VIEW, FeatureType.EVENT)]
        public async Task<IActionResult> GetEventById(Guid id)
        {
            try
            {
                var result = await _eventService.GetEventByIdAsync(id);
                if (result == null)
                    return NotFound(new ApiError("Không tìm thấy sự kiện"));

                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi lấy thông tin sự kiện"));
            }
        }

        #endregion

        #region Query Operations

        /// <summary>
        /// Get events by family tree ID with pagination
        /// </summary>
        [HttpGet("by-gp/{FTId}")]
        [FTAuthorize(MethodType.VIEW, FeatureType.EVENT)]
        public async Task<IActionResult> GetEventsByGP(Guid FTId, [FromQuery] SearchWithPaginationRequest requestParams)
        {
            try
            {
                var skip = (requestParams.PageIndex - 1) * requestParams.PageSize;
                var result = await _eventService.GetEventsByFTIdAsync(FTId, skip, requestParams.PageSize);
                var totalItems = await _eventService.CountEventsByFTIdAsync(FTId);

                return Ok(new ApiSuccess("Lấy danh sách sự kiện thành công", 
                    new Pagination<FTFamilyEventDto>(requestParams.PageIndex, requestParams.PageSize, totalItems, result.ToList())));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi lấy danh sách sự kiện"));
            }
        }

        /// <summary>
        /// Get upcoming events for a family tree
        /// </summary>
        [HttpGet("upcoming")]
        [FTAuthorize(MethodType.VIEW, FeatureType.EVENT)]
        public async Task<IActionResult> GetUpcomingEvents([FromQuery] Guid FTId, [FromQuery] int days = 30)
        {
            try
            {
                var result = await _eventService.GetUpcomingEventsAsync(FTId, days);
                return Ok(new ApiSuccess("Lấy danh sách sự kiện sắp tới thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi lấy sự kiện sắp tới"));
            }
        }

        /// <summary>
        /// Get events by date range
        /// </summary>
        [HttpGet("by-date")]
        [FTAuthorize(MethodType.VIEW, FeatureType.EVENT)]
        public async Task<IActionResult> GetEventsByDateRange(
            [FromQuery] Guid FTId, 
            [FromQuery] DateTimeOffset startDate, 
            [FromQuery] DateTimeOffset endDate)
        {
            try
            {
                var result = await _eventService.GetEventsByDateRangeAsync(FTId, startDate, endDate);
                return Ok(new ApiSuccess("Lấy danh sách sự kiện thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi lấy sự kiện theo khoảng thời gian"));
            }
        }

        /// <summary>
        /// Get events by member ID
        /// </summary>
        [HttpGet("by-member/{memberId}")]
        [FTAuthorize(MethodType.VIEW, FeatureType.EVENT)]
        public async Task<IActionResult> GetEventsByMember(Guid memberId, [FromQuery] SearchWithPaginationRequest requestParams)
        {
            try
            {
                var skip = (requestParams.PageIndex - 1) * requestParams.PageSize;
                var result = await _eventService.GetEventsByMemberIdAsync(memberId, skip, requestParams.PageSize);

                return Ok(new ApiSuccess("Lấy danh sách sự kiện thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi lấy sự kiện theo thành viên"));
            }
        }

        /// <summary>
        /// Filter events with multiple criteria
        /// </summary>
        [HttpPost("filter")]
        [FTAuthorize(MethodType.VIEW, FeatureType.EVENT)]
        public async Task<IActionResult> FilterEvents([FromBody] EventFilterRequest request)
        {
            try
            {
                var result = await _eventService.FilterEventsAsync(request);
                return Ok(new ApiSuccess("Lọc sự kiện thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi lọc sự kiện"));
            }
        }

        /// <summary>
        /// Get events in a specific year
        /// </summary>
        [HttpGet("{year:int}")]
        [FTAuthorize(MethodType.VIEW, FeatureType.EVENT)]
        public async Task<IActionResult> GetEventsGroupedByYear(int year, [FromQuery] Guid ftId)
        {
            try
            {
                var result = await _eventService.GetEventsGroupedByYearAsync(ftId, year);
                return Ok(new ApiSuccess("Lấy sự kiện theo năm thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi lấy sự kiện theo năm"));
            }
        }

        /// <summary>
        /// Get events in a specific month
        /// </summary>
        [HttpGet("{year:int}/{month:int}")]
        [FTAuthorize(MethodType.VIEW, FeatureType.EVENT)]
        public async Task<IActionResult> GetEventsGroupedByMonth(int year, int month, [FromQuery] Guid ftId)
        {
            try
            {
                var result = await _eventService.GetEventsGroupedByMonthAsync(ftId, year, month);
                return Ok(new ApiSuccess("Lấy sự kiện theo tháng thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi lấy sự kiện theo tháng"));
            }
        }

        /// <summary>
        /// Get events in a specific week
        /// </summary>
        [HttpGet("{year:int}/{month:int}/{week:int}")]
        [FTAuthorize(MethodType.VIEW, FeatureType.EVENT)]
        public async Task<IActionResult> GetEventsGroupedByWeek(int year, int month, int week, [FromQuery] Guid ftId)
        {
            try
            {
                var result = await _eventService.GetEventsGroupedByWeekAsync(ftId, year, month, week);
                return Ok(new ApiSuccess("Lấy sự kiện theo tuần thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi lấy sự kiện theo tuần"));
            }
        }

        /// <summary>
        /// Get events of current user (authenticated)
        /// </summary>
        [HttpGet("my-events")]
        [FTAuthorize(MethodType.VIEW, FeatureType.EVENT)]
        public async Task<IActionResult> GetMyEvents([FromQuery] Guid ftId, [FromQuery] SearchWithPaginationRequest requestParams)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                var skip = (requestParams.PageIndex - 1) * requestParams.PageSize;
                var result = await _eventService.GetMyEventsAsync(userId, ftId, skip, requestParams.PageSize);

                return Ok(new ApiSuccess("Lấy sự kiện của tôi thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi lấy sự kiện của bạn"));
            }
        }

        /// <summary>
        /// Get upcoming events of current user (authenticated)
        /// </summary>
        [HttpGet("my-upcoming-events")]
        [FTAuthorize(MethodType.VIEW, FeatureType.EVENT)]
        public async Task<IActionResult> GetMyUpcomingEvents([FromQuery] Guid ftId, [FromQuery] int days = 30)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                var result = await _eventService.GetMyUpcomingEventsAsync(userId, ftId, days);

                return Ok(new ApiSuccess("Lấy sự kiện sắp tới của tôi thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi lấy sự kiện sắp tới của bạn"));
            }
        }

        #endregion

        #region Member Management

        /// <summary>
        /// Add a member to an event
        /// </summary>
        [HttpPost("{eventId}/add-member/{memberId}")]
        [FTAuthorize(MethodType.ADD, FeatureType.EVENT)]
        public async Task<IActionResult> AddMemberToEvent(Guid eventId, Guid memberId)
        {
            try
            {
                var result = await _eventService.AddMemberToEventAsync(eventId, memberId);
                if (!result)
                    return BadRequest(new ApiError("Thành viên đã có trong sự kiện hoặc không tìm thấy sự kiện"));

                return Ok(new ApiSuccess("Thêm thành viên vào sự kiện thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi thêm thành viên vào sự kiện"));
            }
        }

        /// <summary>
        /// Remove a member from an event
        /// </summary>
        [HttpDelete("{eventId}/remove-member/{memberId}")]
        [FTAuthorize(MethodType.DELETE, FeatureType.EVENT)]
        public async Task<IActionResult> RemoveMemberFromEvent(Guid eventId, Guid memberId)
        {
            try
            {
                var result = await _eventService.RemoveMemberFromEventAsync(eventId, memberId);
                if (!result)
                    return NotFound(new ApiError("Không tìm thấy thành viên trong sự kiện"));

                return Ok(new ApiSuccess("Xóa thành viên khỏi sự kiện thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi xóa thành viên khỏi sự kiện"));
            }
        }

        /// <summary>
        /// Get all members in an event
        /// </summary>
        [HttpGet("{eventId}/members")]
        [FTAuthorize(MethodType.VIEW, FeatureType.EVENT)]
        public async Task<IActionResult> GetEventMembers(Guid eventId)
        {
            try
            {
                var result = await _eventService.GetEventMembersAsync(eventId);
                return Ok(new ApiSuccess("Lấy danh sách thành viên sự kiện thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Đã xảy ra lỗi khi lấy danh sách thành viên"));
            }
        }

        #endregion
    }
}

