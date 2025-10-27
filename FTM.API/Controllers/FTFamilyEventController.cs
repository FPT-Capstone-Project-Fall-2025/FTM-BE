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
        public async Task<IActionResult> CreateEvent([FromForm] CreateFTFamilyEventRequest request)
        {
            try
            {
                var result = await _eventService.CreateEventAsync(request);
                return Ok(new ApiSuccess("Event created successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.ToString()));
            }
        }

        /// <summary>
        /// Update an existing event
        /// </summary>
        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromForm] UpdateFTFamilyEventRequest request)
        {
            try
            {
                var result = await _eventService.UpdateEventAsync(id, request);
                return Ok(new ApiSuccess("Event updated successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Delete an event
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            try
            {
                var result = await _eventService.DeleteEventAsync(id);
                if (!result)
                    return NotFound(new ApiError("Event not found"));

                return Ok(new ApiSuccess("Event deleted successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get event by ID
        /// </summary>
        [HttpGet("event/{id:guid}")]
        public async Task<IActionResult> GetEventById(Guid id)
        {
            try
            {
                var result = await _eventService.GetEventByIdAsync(id);
                if (result == null)
                    return NotFound(new ApiError("Event not found"));

                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        #endregion

        #region Query Operations

        /// <summary>
        /// Get events by family tree ID with pagination
        /// </summary>
        [HttpGet("by-gp/{FTId}")]
        public async Task<IActionResult> GetEventsByGP(Guid FTId, [FromQuery] SearchWithPaginationRequest requestParams)
        {
            try
            {
                var skip = (requestParams.PageIndex - 1) * requestParams.PageSize;
                var result = await _eventService.GetEventsByFTIdAsync(FTId, skip, requestParams.PageSize);
                var totalItems = await _eventService.CountEventsByFTIdAsync(FTId);

                return Ok(new ApiSuccess("Events retrieved successfully", 
                    new Pagination<FTFamilyEventDto>(requestParams.PageIndex, requestParams.PageSize, totalItems, result.ToList())));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get upcoming events for a family tree
        /// </summary>
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingEvents([FromQuery] Guid FTId, [FromQuery] int days = 30)
        {
            try
            {
                var result = await _eventService.GetUpcomingEventsAsync(FTId, days);
                return Ok(new ApiSuccess("Upcoming events retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get events by date range
        /// </summary>
        [HttpGet("by-date")]
        public async Task<IActionResult> GetEventsByDateRange(
            [FromQuery] Guid FTId, 
            [FromQuery] DateTimeOffset startDate, 
            [FromQuery] DateTimeOffset endDate)
        {
            try
            {
                var result = await _eventService.GetEventsByDateRangeAsync(FTId, startDate, endDate);
                return Ok(new ApiSuccess("Events retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get events by member ID
        /// </summary>
        [HttpGet("by-member/{memberId}")]
        public async Task<IActionResult> GetEventsByMember(Guid memberId, [FromQuery] SearchWithPaginationRequest requestParams)
        {
            try
            {
                var skip = (requestParams.PageIndex - 1) * requestParams.PageSize;
                var result = await _eventService.GetEventsByMemberIdAsync(memberId, skip, requestParams.PageSize);

                return Ok(new ApiSuccess("Events retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Filter events with multiple criteria
        /// </summary>
        [HttpPost("filter")]
        public async Task<IActionResult> FilterEvents([FromBody] EventFilterRequest request)
        {
            try
            {
                var result = await _eventService.FilterEventsAsync(request);
                return Ok(new ApiSuccess("Events filtered successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get events in a specific year
        /// </summary>
        [HttpGet("{year:int}")]
        public async Task<IActionResult> GetEventsGroupedByYear(int year, [FromQuery] Guid ftId)
        {
            try
            {
                var result = await _eventService.GetEventsGroupedByYearAsync(ftId, year);
                return Ok(new ApiSuccess("Events in year successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get events in a specific month
        /// </summary>
        [HttpGet("{year:int}/{month:int}")]
        public async Task<IActionResult> GetEventsGroupedByMonth(int year, int month, [FromQuery] Guid ftId)
        {
            try
            {
                var result = await _eventService.GetEventsGroupedByMonthAsync(ftId, year, month);
                return Ok(new ApiSuccess("Events in month successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get events in a specific week
        /// </summary>
        [HttpGet("{year:int}/{month:int}/{week:int}")]
        public async Task<IActionResult> GetEventsGroupedByWeek(int year, int month, int week, [FromQuery] Guid ftId)
        {
            try
            {
                var result = await _eventService.GetEventsGroupedByWeekAsync(ftId, year, month, week);
                return Ok(new ApiSuccess("Events in week successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get events of current user (authenticated)
        /// </summary>
        [HttpGet("my-events")]
        [Authorize]
        public async Task<IActionResult> GetMyEvents([FromQuery] Guid ftId, [FromQuery] SearchWithPaginationRequest requestParams)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                var skip = (requestParams.PageIndex - 1) * requestParams.PageSize;
                var result = await _eventService.GetMyEventsAsync(userId, ftId, skip, requestParams.PageSize);

                return Ok(new ApiSuccess("My events retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get upcoming events of current user (authenticated)
        /// </summary>
        [HttpGet("my-upcoming-events")]
        [Authorize]
        public async Task<IActionResult> GetMyUpcomingEvents([FromQuery] Guid ftId, [FromQuery] int days = 30)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                var result = await _eventService.GetMyUpcomingEventsAsync(userId, ftId, days);

                return Ok(new ApiSuccess("My upcoming events retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        #endregion

        #region Member Management

        /// <summary>
        /// Add a member to an event
        /// </summary>
        [HttpPost("{eventId}/add-member/{memberId}")]
        [Authorize]
        public async Task<IActionResult> AddMemberToEvent(Guid eventId, Guid memberId)
        {
            try
            {
                var result = await _eventService.AddMemberToEventAsync(eventId, memberId);
                if (!result)
                    return BadRequest(new ApiError("Member already in event or event not found"));

                return Ok(new ApiSuccess("Member added to event successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Remove a member from an event
        /// </summary>
        [HttpDelete("{eventId}/remove-member/{memberId}")]
        [Authorize]
        public async Task<IActionResult> RemoveMemberFromEvent(Guid eventId, Guid memberId)
        {
            try
            {
                var result = await _eventService.RemoveMemberFromEventAsync(eventId, memberId);
                if (!result)
                    return NotFound(new ApiError("Member not found in event"));

                return Ok(new ApiSuccess("Member removed from event successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get all members in an event
        /// </summary>
        [HttpGet("{eventId}/members")]
        public async Task<IActionResult> GetEventMembers(Guid eventId)
        {
            try
            {
                var result = await _eventService.GetEventMembersAsync(eventId);
                return Ok(new ApiSuccess("Event members retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        #endregion
    }
}

