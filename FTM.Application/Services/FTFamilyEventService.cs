using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.Events;
using FTM.Domain.Enums;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FTM.Application.Services
{
    public class FTFamilyEventService : IFTFamilyEventService
    {
        private readonly IFTFamilyEventRepository _eventRepository;
        private readonly IFTAuthorizationRepository _authorizationRepository;
        private readonly FTMDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FTFamilyEventService(
            IFTFamilyEventRepository eventRepository,
            IFTAuthorizationRepository authorizationRepository,
            FTMDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _eventRepository = eventRepository;
            _authorizationRepository = authorizationRepository;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private Guid GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
        }

        private string GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        }

        public async Task<FTFamilyEventDto> CreateEventAsync(CreateFTFamilyEventRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();
            
            // Validate FTId exists
            var familyTree = await _context.FamilyTrees.FindAsync(request.FTId);
            if (familyTree == null)
                throw new Exception($"Family tree with ID {request.FTId} not found");

            // Validate user permissions
            var userMember = await _context.FTMembers
                .FirstOrDefaultAsync(m => m.UserId == currentUserId && m.FTId == request.FTId && m.IsDeleted == false);
            if (userMember == null && currentUserRole != "GPOwner")
                throw new Exception("User is not a member of this family tree");

            if (userMember != null)
            {
                var hasPermission = await _authorizationRepository.IsAuthorizationExisting(
                    request.FTId, userMember.Id, FeatureType.EVENT, MethodType.ADD);
                if (!hasPermission && currentUserRole != "GPOwner")
                    throw new Exception("User does not have permission to create events in this family tree");
            }

            // Validate RecurrenceType
            if (request.RecurrenceType < 0 || request.RecurrenceType > 3)
                throw new Exception("Invalid recurrence type. Must be between 0 and 3");

            // If recurring, RecurrenceEndTime is required
            if ((RecurrenceType)request.RecurrenceType != RecurrenceType.None && !request.RecurrenceEndTime.HasValue)
                throw new Exception("RecurrenceEndTime is required for recurring events");

            // Validate date times
            if (request.EndTime.HasValue && request.StartTime >= request.EndTime.Value)
                throw new Exception("Start time must be before end time");

            // Validate TargetMemberId exists and belongs to FT if provided
            if (request.TargetMemberId.HasValue)
            {
                var targetMember = await _context.FTMembers.FindAsync(request.TargetMemberId.Value);
                if (targetMember == null)
                    throw new Exception($"Target member with ID {request.TargetMemberId} not found");
                if (targetMember.FTId != request.FTId)
                    throw new Exception("Target member does not belong to this family tree");
                
                // For personal events, IsPublic must be true
                if (!request.IsPublic)
                    throw new Exception("IsPublic must be true for personal events");
            }

            // Validate MemberIds exist and belong to FT
            if (request.MemberIds != null && request.MemberIds.Any())
            {
                foreach (var memberId in request.MemberIds)
                {
                    var member = await _context.FTMembers.FindAsync(memberId);
                    if (member == null)
                        throw new Exception($"Member with ID {memberId} not found");
                    if (member.FTId != request.FTId)
                        throw new Exception($"Member with ID {memberId} does not belong to this family tree");
                }
            }

            var eventEntity = new FTFamilyEvent
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                EventType = (FTM.Domain.Enums.EventType)request.EventType,
                // Always store solar dates in DB
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Location = request.Location,
                RecurrenceType = (RecurrenceType)request.RecurrenceType,
                FTId = request.FTId,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                ReferenceEventId = request.ReferenceEventId,
                Address = request.Address,
                LocationName = request.LocationName,
                IsAllDay = request.IsAllDay,
                RecurrenceEndTime = request.RecurrenceEndTime,
                IsLunar = request.IsLunar,
                TargetMemberId = request.TargetMemberId,
                IsPublic = request.IsPublic,
                CreatedOn = DateTimeOffset.UtcNow,
                CreatedByUserId = currentUserId,
                IsDeleted = false
            };

            await _eventRepository.AddAsync(eventEntity);

            // Add FT link
            var eventFT = new FTFamilyEventFT
            {
                Id = Guid.NewGuid(),
                FTFamilyEventId = eventEntity.Id,
                FTId = request.FTId,
                CreatedOn = DateTimeOffset.UtcNow,
                CreatedByUserId = currentUserId,
                IsDeleted = false
            };
            await _context.FTFamilyEventFTs.AddAsync(eventFT);

            // Add target member if personal event
            if (request.TargetMemberId.HasValue)
            {
                var targetEventMember = new FTFamilyEventMember
                {
                    Id = Guid.NewGuid(),
                    FTFamilyEventId = eventEntity.Id,
                    FTMemberId = request.TargetMemberId.Value,
                    CreatedOn = DateTimeOffset.UtcNow,
                    CreatedByUserId = currentUserId,
                    IsDeleted = false
                };
                await _context.FTFamilyEventMembers.AddAsync(targetEventMember);
            }

            // Add members if provided
            if (request.MemberIds != null && request.MemberIds.Any())
            {
                foreach (var memberId in request.MemberIds)
                {
                    // Skip if already added as target member
                    if (request.TargetMemberId.HasValue && memberId == request.TargetMemberId.Value)
                        continue;

                    var eventMember = new FTFamilyEventMember
                    {
                        Id = Guid.NewGuid(),
                        FTFamilyEventId = eventEntity.Id,
                        FTMemberId = memberId,
                        CreatedOn = DateTimeOffset.UtcNow,
                        CreatedByUserId = currentUserId,
                        IsDeleted = false
                    };
                    await _context.FTFamilyEventMembers.AddAsync(eventMember);
                }
            }

            await _context.SaveChangesAsync();

            // Generate and save recurring events if applicable
            if (eventEntity.RecurrenceType != RecurrenceType.None && eventEntity.RecurrenceEndTime.HasValue)
            {
                var recurringEvents = GenerateRecurringEventsForSave(eventEntity);
                foreach (var recEvent in recurringEvents)
                {
                    await _eventRepository.AddAsync(recEvent);

                    // Add FT link
                    var recEventFT = new FTFamilyEventFT
                    {
                        Id = Guid.NewGuid(),
                        FTFamilyEventId = recEvent.Id,
                        FTId = request.FTId,
                        CreatedOn = DateTimeOffset.UtcNow,
                        CreatedByUserId = currentUserId,
                        IsDeleted = false
                    };
                    await _context.FTFamilyEventFTs.AddAsync(recEventFT);

                    // Add target member if personal event
                    if (request.TargetMemberId.HasValue)
                    {
                        var recTargetMember = new FTFamilyEventMember
                        {
                            Id = Guid.NewGuid(),
                            FTFamilyEventId = recEvent.Id,
                            FTMemberId = request.TargetMemberId.Value,
                            CreatedOn = DateTimeOffset.UtcNow,
                            CreatedByUserId = currentUserId,
                            IsDeleted = false
                        };
                        await _context.FTFamilyEventMembers.AddAsync(recTargetMember);
                    }

                    // Add members if provided
                    if (request.MemberIds != null && request.MemberIds.Any())
                    {
                        foreach (var memberId in request.MemberIds)
                        {
                            // Skip if already added as target member
                            if (request.TargetMemberId.HasValue && memberId == request.TargetMemberId.Value)
                                continue;

                            var recEventMember = new FTFamilyEventMember
                            {
                                Id = Guid.NewGuid(),
                                FTFamilyEventId = recEvent.Id,
                                FTMemberId = memberId,
                                CreatedOn = DateTimeOffset.UtcNow,
                                CreatedByUserId = currentUserId,
                                IsDeleted = false
                            };
                            await _context.FTFamilyEventMembers.AddAsync(recEventMember);
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }

            return await GetEventByIdAsync(eventEntity.Id);
        }

        public async Task<FTFamilyEventDto> UpdateEventAsync(Guid id, UpdateFTFamilyEventRequest request)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
                throw new Exception("Event not found");

            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            // Validate user permissions
            var userMember = await _context.FTMembers
                .FirstOrDefaultAsync(m => m.UserId == currentUserId && m.FTId == eventEntity.FTId && m.IsDeleted == false);
            if (userMember == null && currentUserRole != "GPOwner")
                throw new Exception("User is not a member of this family tree");

            if (userMember != null)
            {
                var hasPermission = await _authorizationRepository.IsAuthorizationExisting(
                    eventEntity.FTId, userMember.Id, FeatureType.EVENT, MethodType.UPDATE);
                if (!hasPermission && currentUserRole != "GPOwner")
                    throw new Exception("User does not have permission to update events in this family tree");
            }

            // Validate RecurrenceType if provided
            if (request.RecurrenceType.HasValue && (request.RecurrenceType.Value < 0 || request.RecurrenceType.Value > 3))
                throw new Exception("Invalid recurrence type. Must be between 0 and 3");

            // If recurring, RecurrenceEndTime is required
            if (request.RecurrenceType.HasValue && (RecurrenceType)request.RecurrenceType.Value != RecurrenceType.None && !request.RecurrenceEndTime.HasValue)
                throw new Exception("RecurrenceEndTime is required for recurring events");

            // Validate date times if both provided
            if (request.StartTime.HasValue && request.EndTime.HasValue && request.StartTime.Value >= request.EndTime.Value)
                throw new Exception("Start time must be before end time");

            // Validate TargetMemberId exists and belongs to FT if provided
            if (request.TargetMemberId.HasValue)
            {
                var targetMember = await _context.FTMembers.FindAsync(request.TargetMemberId.Value);
                if (targetMember == null)
                    throw new Exception($"Target member with ID {request.TargetMemberId} not found");
                if (targetMember.FTId != eventEntity.FTId)
                    throw new Exception("Target member does not belong to this family tree");

                // For personal events, IsPublic must be true if provided
                if (request.IsPublic.HasValue && !request.IsPublic.Value)
                    throw new Exception("IsPublic must be true for personal events");
            }

            // Validate MemberIds exist and belong to FT
            if (request.MemberIds != null && request.MemberIds.Any())
            {
                foreach (var memberId in request.MemberIds)
                {
                    var member = await _context.FTMembers.FindAsync(memberId);
                    if (member == null)
                        throw new Exception($"Member with ID {memberId} not found");
                    if (member.FTId != eventEntity.FTId)
                        throw new Exception($"Member with ID {memberId} does not belong to this family tree");
                }
            }

            // Update only provided fields
            if (request.Name != null)
                eventEntity.Name = request.Name;
            if (request.EventType.HasValue)
                eventEntity.EventType = (FTM.Domain.Enums.EventType)request.EventType.Value;
            if (request.StartTime.HasValue)
            {
                eventEntity.StartTime = request.StartTime.Value;
            }
            if (request.EndTime.HasValue)
            {
                eventEntity.EndTime = request.EndTime.Value;
            }
            if (request.Location != null)
                eventEntity.Location = request.Location;
            if (request.RecurrenceType.HasValue)
                eventEntity.RecurrenceType = (RecurrenceType)request.RecurrenceType.Value;
            if (request.Description != null)
                eventEntity.Description = request.Description;
            if (request.ImageUrl != null)
                eventEntity.ImageUrl = request.ImageUrl;
            if (request.Address != null)
                eventEntity.Address = request.Address;
            if (request.LocationName != null)
                eventEntity.LocationName = request.LocationName;
            if (request.IsAllDay.HasValue)
                eventEntity.IsAllDay = request.IsAllDay.Value;
            if (request.RecurrenceEndTime.HasValue)
                eventEntity.RecurrenceEndTime = request.RecurrenceEndTime.Value;
            if (request.IsLunar.HasValue)
                eventEntity.IsLunar = request.IsLunar.Value;
            if (request.TargetMemberId.HasValue)
                eventEntity.TargetMemberId = request.TargetMemberId.Value;
            if (request.IsPublic.HasValue)
                eventEntity.IsPublic = request.IsPublic.Value;
            eventEntity.LastModifiedOn = DateTimeOffset.UtcNow;
            eventEntity.LastModifiedBy = currentUserId.ToString();

            _eventRepository.Update(eventEntity);

            // Update members if provided
            if (request.MemberIds != null)
            {
                // Remove existing members
                var existingMembers = await _context.FTFamilyEventMembers
                    .Where(em => em.FTFamilyEventId == id && em.IsDeleted == false)
                    .ToListAsync();

                foreach (var member in existingMembers)
                {
                    member.IsDeleted = true;
                    member.LastModifiedOn = DateTimeOffset.UtcNow;
                }

                // Add new members
                foreach (var memberId in request.MemberIds)
                {
                    var eventMember = new FTFamilyEventMember
                    {
                        Id = Guid.NewGuid(),
                        FTFamilyEventId = id,
                        FTMemberId = memberId,
                        CreatedOn = DateTimeOffset.UtcNow,
                        CreatedByUserId = currentUserId,
                        IsDeleted = false
                    };
                    await _context.FTFamilyEventMembers.AddAsync(eventMember);
                }
            }

            await _context.SaveChangesAsync();

            return await GetEventByIdAsync(id);
        }

        public async Task<bool> DeleteEventAsync(Guid eventId)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                return false;

            eventEntity.IsDeleted = true;
            eventEntity.LastModifiedOn = DateTimeOffset.UtcNow;

            _eventRepository.Update(eventEntity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<FTFamilyEventDto?> GetEventByIdAsync(Guid eventId)
        {
            var eventEntity = await _eventRepository.GetEventWithDetailsAsync(eventId);
            if (eventEntity == null)
                return null;

            return MapToDto(eventEntity);
        }

        public async Task<IEnumerable<FTFamilyEventDto>> GetEventsByFTIdAsync(Guid FTId, int skip = 0, int take = 20)
        {
            var events = await _context.FTFamilyEvents
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Where(e => e.FTId == FTId && e.IsDeleted == false)
                .OrderBy(e => e.StartTime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return events.Select(e => MapToDto(e)).ToList();
        }

        public async Task<int> CountEventsByFTIdAsync(Guid FTId)
        {
            return await _context.FTFamilyEvents
                .Where(e => e.FTId == FTId && e.IsDeleted == false)
                .CountAsync();
        }

        public async Task<IEnumerable<FTFamilyEventDto>> GetUpcomingEventsAsync(Guid FTId, int days = 30)
        {
            var now = DateTimeOffset.UtcNow;
            var endDate = now.AddDays(days);
            var events = await _context.FTFamilyEvents
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Where(e => e.FTId == FTId && e.IsDeleted == false
                    && e.StartTime >= now && e.StartTime <= endDate)
                .OrderBy(e => e.StartTime)
                .ToListAsync();

            return events.Select(e => MapToDto(e)).ToList();
        }

        public async Task<IEnumerable<FTFamilyEventDto>> GetEventsByDateRangeAsync(Guid FTId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var events = await _context.FTFamilyEvents
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Where(e => e.FTId == FTId && e.IsDeleted == false
                    && e.StartTime >= startDate && e.StartTime <= endDate)
                .OrderBy(e => e.StartTime)
                .ToListAsync();

            return events.Select(e => MapToDto(e)).ToList();
        }

        public async Task<IEnumerable<FTFamilyEventDto>> GetEventsByMemberIdAsync(Guid memberId, int skip = 0, int take = 20)
        {
            var events = await _eventRepository.GetEventsByMemberIdAsync(memberId, skip, take);
            return events.Select(e => MapToDto(e)).ToList();
        }

        public async Task<IEnumerable<FTFamilyEventDto>> FilterEventsAsync(EventFilterRequest request)
        {
            var query = _context.FTFamilyEvents
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Where(e => e.FTId == request.FTId && e.IsDeleted == false);

            if (request.StartDate.HasValue)
            {
                query = query.Where(e => e.StartTime >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(e => e.StartTime <= request.EndDate.Value);
            }

            if (request.EventType.HasValue)
            {
                query = query.Where(e => e.EventType == request.EventType.Value);
            }

            if (request.IsLunar.HasValue)
            {
                query = query.Where(e => e.IsLunar == request.IsLunar.Value);
            }

            var events = await query
                .OrderBy(e => e.StartTime)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return events.Select(e => MapToDto(e)).ToList();
        }

        public async Task<bool> AddMemberToEventAsync(Guid eventId, Guid memberId)
        {
            // Check if member already in event
            if (await _eventRepository.IsMemberInEventAsync(eventId, memberId))
                return false;

            var currentUserId = GetCurrentUserId();
            var eventMember = new FTFamilyEventMember
            {
                Id = Guid.NewGuid(),
                FTFamilyEventId = eventId,
                FTMemberId = memberId,
                CreatedOn = DateTimeOffset.UtcNow,
                CreatedByUserId = currentUserId,
                IsDeleted = false
            };

            await _context.FTFamilyEventMembers.AddAsync(eventMember);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveMemberFromEventAsync(Guid eventId, Guid memberId)
        {
            var eventMember = await _context.FTFamilyEventMembers
                .FirstOrDefaultAsync(em => em.FTFamilyEventId == eventId 
                    && em.FTMemberId == memberId 
                    && em.IsDeleted == false);

            if (eventMember == null)
                return false;

            eventMember.IsDeleted = true;
            eventMember.LastModifiedOn = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<FTFamilyEventDto>> GetMyEventsAsync(Guid userId, Guid ftId, int skip = 0, int take = 20)
        {
            // Tìm FTMember của user trong family tree này
            var member = await _context.FTMembers
                .Where(m => m.UserId == userId && m.FTId == ftId && m.IsDeleted == false)
                .FirstOrDefaultAsync();

            if (member == null)
                return new List<FTFamilyEventDto>();

            // Lấy các sự kiện mà member này được tag
            var events = await _context.FTFamilyEventMembers
                .Include(em => em.FTFamilyEvent)
                    .ThenInclude(e => e.EventMembers)
                        .ThenInclude(em => em.FTMember)
                .Where(em => em.FTMemberId == member.Id && em.IsDeleted == false)
                .Select(em => em.FTFamilyEvent)
                .Where(e => e.IsDeleted == false && e.FTId == ftId)
                .OrderBy(e => e.StartTime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return events.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<FTFamilyEventDto>> GetMyUpcomingEventsAsync(Guid userId, Guid ftId, int days = 30)
        {
            // Tìm FTMember của user trong family tree này
            var member = await _context.FTMembers
                .Where(m => m.UserId == userId && m.FTId == ftId && m.IsDeleted == false)
                .FirstOrDefaultAsync();

            if (member == null)
                return new List<FTFamilyEventDto>();

            var now = DateTimeOffset.UtcNow;
            var endDate = now.AddDays(days);

            // Lấy các sự kiện sắp tới mà member này được tag
            var events = await _context.FTFamilyEventMembers
                .Include(em => em.FTFamilyEvent)
                    .ThenInclude(e => e.EventMembers)
                        .ThenInclude(em => em.FTMember)
                .Where(em => em.FTMemberId == member.Id && em.IsDeleted == false)
                .Select(em => em.FTFamilyEvent)
                .Where(e => e.IsDeleted == false 
                    && e.FTId == ftId
                    && e.StartTime >= now 
                    && e.StartTime <= endDate)
                .OrderBy(e => e.StartTime)
                .ToListAsync();

            return events.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<FTFamilyEventMemberDto>> GetEventMembersAsync(Guid eventId)
        {
            var members = await _context.FTFamilyEventMembers
                .Include(em => em.FTMember)
                .Where(em => em.FTFamilyEventId == eventId && em.IsDeleted == false)
                .ToListAsync();

            return members.Select(em => new FTFamilyEventMemberDto
            {
                Id = em.Id,
                FTMemberId = em.FTMemberId,
                MemberName = em.FTMember?.Fullname ?? "Unknown",
                MemberPicture = em.FTMember?.Picture,
                UserId = em.UserId
            }).ToList();
        }

        private DateTimeOffset GetNextLunarDate(DateTimeOffset currentDate, RecurrenceType type)
        {
            // Deprecated: replaced by explicit lunar conversion helpers.
            // Keep method for backward compat but forward to conversion using lunar helpers.
            return currentDate;
        }

        // Convert a solar DateTimeOffset into a stored "lunar representation" DateTimeOffset.
        // We store the lunar year/month/day in the DateTime fields, and preserve the time-of-day and offset.
        private DateTimeOffset ConvertSolarToLunarRepresentation(DateTimeOffset solar)
        {
            var cal = new ChineseLunisolarCalendar();
            var localSolar = solar.ToLocalTime(); // Convert to local time for accurate lunar calculation
            int lunarYear = cal.GetYear(localSolar.DateTime);
            int lunarMonth = cal.GetMonth(localSolar.DateTime);
            int lunarDay = cal.GetDayOfMonth(localSolar.DateTime);

            // Adjust for Vietnamese lunar calendar: subtract the number of leap months before this month
            int leapCount = 0;
            for (int m = 1; m < lunarMonth; m++)
            {
                if (cal.IsLeapMonth(lunarYear, m))
                {
                    leapCount++;
                }
            }
            lunarMonth -= leapCount;

            var dt = new DateTime(lunarYear, lunarMonth, lunarDay, localSolar.Hour, localSolar.Minute, localSolar.Second, localSolar.Millisecond);
            return new DateTimeOffset(dt, localSolar.Offset);
        }

        // Convert a stored lunar-representation DateTimeOffset (year/month/day are lunar components)
        // into a solar DateTimeOffset for a given target lunar year. If targetLunarYear is null,
        // convert using the year stored in the representation.
        private DateTimeOffset ConvertLunarRepresentationToSolar(DateTimeOffset lunarRepresentation, int? targetLunarYear = null)
        {
            var cal = new ChineseLunisolarCalendar();
            int storedLunarYear = lunarRepresentation.DateTime.Year;
            int lunarMonth = lunarRepresentation.DateTime.Month;
            int lunarDay = lunarRepresentation.DateTime.Day;
            int yearToUse = targetLunarYear ?? storedLunarYear;

            // Adjust back: add the number of leap months before this adjusted month
            int leapCount = 0;
            for (int m = 1; m <= 12; m++)
            {
                if (cal.IsLeapMonth(yearToUse, m))
                {
                    if (m < lunarMonth + leapCount)
                    {
                        leapCount++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            lunarMonth += leapCount;

            // Ensure lunarDay does not exceed the days in the lunar month
            int maxDays = cal.GetDaysInMonth(yearToUse, lunarMonth);
            if (lunarDay > maxDays)
            {
                lunarDay = maxDays;
            }

            // ToDateTime expects a valid lunar date; exceptions propagate if invalid.
            DateTime solar = cal.ToDateTime(yearToUse, lunarMonth, lunarDay, lunarRepresentation.Hour, lunarRepresentation.Minute, lunarRepresentation.Second, lunarRepresentation.Millisecond);
            return new DateTimeOffset(solar, lunarRepresentation.Offset);
        }

        // Convert a DateTimeOffset with lunar components (year/month/day) to solar DateTimeOffset
        private DateTimeOffset ConvertLunarDateToSolar(DateTimeOffset lunarInput)
        {
            var cal = new ChineseLunisolarCalendar();
            int lunarYear = lunarInput.Year;
            int lunarMonth = lunarInput.Month;
            int lunarDay = lunarInput.Day;
            DateTime solar = cal.ToDateTime(lunarYear, lunarMonth, lunarDay, lunarInput.Hour, lunarInput.Minute, lunarInput.Second, lunarInput.Millisecond);
            return new DateTimeOffset(solar, lunarInput.Offset);
        }

        // Compare two lunar dates represented as DateTime (year/month/day are lunar components)
        // returns <0 if a < b, 0 if equal, >0 if a > b
        private int CompareLunar(DateTimeOffset a, DateTimeOffset b)
        {
            if (a.DateTime.Year != b.DateTime.Year)
                return a.DateTime.Year.CompareTo(b.DateTime.Year);
            if (a.DateTime.Month != b.DateTime.Month)
                return a.DateTime.Month.CompareTo(b.DateTime.Month);
            return a.DateTime.Day.CompareTo(b.DateTime.Day);
        }

        private IEnumerable<FTFamilyEvent> GenerateRecurringEvents(IEnumerable<FTFamilyEvent> events, DateTimeOffset? startRange = null, DateTimeOffset? endRange = null)
        {
            var result = new List<FTFamilyEvent>();

            foreach (var eventEntity in events)
            {
                // Add the original event only if within the range (if range is specified)
                if (!startRange.HasValue || !endRange.HasValue || (eventEntity.StartTime >= startRange.Value && eventEntity.StartTime <= endRange.Value))
                {
                    result.Add(eventEntity);
                }

                // Only generate recurring if range is specified
                if (!startRange.HasValue || !endRange.HasValue)
                    continue;

                if (eventEntity.RecurrenceType == RecurrenceType.None || !eventEntity.RecurrenceEndTime.HasValue)
                    continue;

                var currentStart = eventEntity.StartTime;
                var currentEnd = eventEntity.EndTime;
                var recurrenceEnd = eventEntity.RecurrenceEndTime.Value;

                // Limit to range if provided
                var effectiveStart = startRange ?? DateTimeOffset.MinValue;
                var effectiveEnd = endRange ?? DateTimeOffset.MaxValue;

                while (true)
                {
                    // Calculate next occurrence
                    if (eventEntity.IsLunar)
                    {
                        // Convert current solar to lunar rep, advance lunar, convert back to solar
                        var lunarRepStart = ConvertSolarToLunarRepresentation(currentStart);
                        var lunarRepEnd = currentEnd.HasValue ? ConvertSolarToLunarRepresentation(currentEnd.Value) : lunarRepStart;

                        DateTimeOffset advancedLunarStart;
                        DateTimeOffset advancedLunarEnd;

                        if (eventEntity.RecurrenceType == RecurrenceType.Monthly)
                        {
                            advancedLunarStart = new DateTimeOffset(lunarRepStart.DateTime.AddMonths(1), lunarRepStart.Offset);
                            advancedLunarEnd = new DateTimeOffset(lunarRepEnd.DateTime.AddMonths(1), lunarRepEnd.Offset);
                        }
                        else if (eventEntity.RecurrenceType == RecurrenceType.Yearly)
                        {
                            advancedLunarStart = new DateTimeOffset(lunarRepStart.DateTime.AddYears(1), lunarRepStart.Offset);
                            advancedLunarEnd = new DateTimeOffset(lunarRepEnd.DateTime.AddYears(1), lunarRepEnd.Offset);
                        }
                        else // Daily
                        {
                            advancedLunarStart = new DateTimeOffset(lunarRepStart.DateTime.AddDays(1), lunarRepStart.Offset);
                            advancedLunarEnd = new DateTimeOffset(lunarRepEnd.DateTime.AddDays(1), lunarRepEnd.Offset);
                        }

                        // Convert back to solar
                        currentStart = ConvertLunarRepresentationToSolar(advancedLunarStart);
                        currentEnd = ConvertLunarRepresentationToSolar(advancedLunarEnd);
                    }
                    else
                    {
                        switch (eventEntity.RecurrenceType)
                        {
                            case RecurrenceType.Daily:
                                currentStart = currentStart.AddDays(1);
                                currentEnd = currentEnd?.AddDays(1);
                                break;
                            case RecurrenceType.Monthly:
                                currentStart = currentStart.AddMonths(1);
                                currentEnd = currentEnd?.AddMonths(1);
                                break;
                            case RecurrenceType.Yearly:
                                currentStart = currentStart.AddYears(1);
                                currentEnd = currentEnd?.AddYears(1);
                                break;
                            default:
                                break;
                        }
                    }

                    // Stop if past recurrence end or range end
                    if (currentStart > recurrenceEnd || currentStart > effectiveEnd)
                        break;

                    // Skip if before range start
                    if (currentStart < effectiveStart)
                        continue;

                    // Create a new event instance
                    // Stored times are already in correct format
                    DateTimeOffset returnedStart = currentStart;
                    DateTimeOffset? returnedEnd = currentEnd;

                    var recurringEvent = new FTFamilyEvent
                    {
                        Id = Guid.NewGuid(), // New ID for recurring instance
                        Name = eventEntity.Name,
                        EventType = eventEntity.EventType,
                        StartTime = returnedStart,
                        EndTime = returnedEnd,
                        Location = eventEntity.Location,
                        RecurrenceType = RecurrenceType.None, // Instances don't recur themselves
                        FTId = eventEntity.FTId,
                        Description = eventEntity.Description,
                        ImageUrl = eventEntity.ImageUrl,
                        ReferenceEventId = eventEntity.Id, // Reference to original
                        Address = eventEntity.Address,
                        LocationName = eventEntity.LocationName,
                        IsAllDay = eventEntity.IsAllDay,
                        RecurrenceEndTime = null, // No recurrence for instances
                        IsLunar = eventEntity.IsLunar,
                        TargetMemberId = eventEntity.TargetMemberId,
                        IsPublic = eventEntity.IsPublic,
                        CreatedOn = eventEntity.CreatedOn,
                        CreatedByUserId = eventEntity.CreatedByUserId,
                        IsDeleted = false,
                        // Copy navigation properties
                        FT = eventEntity.FT,
                        TargetMember = eventEntity.TargetMember,
                        EventMembers = eventEntity.EventMembers != null ? eventEntity.EventMembers.Select(em => new FTFamilyEventMember
                        {
                            Id = Guid.NewGuid(),
                            FTFamilyEventId = Guid.Empty, // Will be set later if needed
                            FTMemberId = em.FTMemberId,
                            FTMember = em.FTMember,
                            CreatedOn = em.CreatedOn,
                            CreatedByUserId = em.CreatedByUserId,
                            IsDeleted = false
                        }).ToList() : new List<FTFamilyEventMember>(),
                        EventFTs = eventEntity.EventFTs != null ? eventEntity.EventFTs.Select(eg => new FTFamilyEventFT
                        {
                            Id = Guid.NewGuid(),
                            FTFamilyEventId = Guid.Empty,
                            FTId = eg.FTId,
                            FT = eg.FT,
                            CreatedOn = eg.CreatedOn,
                            CreatedByUserId = eg.CreatedByUserId,
                            IsDeleted = false
                        }).ToList() : new List<FTFamilyEventFT>()
                    };

                    result.Add(recurringEvent);
                }
            }

            return result;
        }

        private List<FTFamilyEvent> GenerateRecurringEventsForSave(FTFamilyEvent originalEvent)
        {
            var result = new List<FTFamilyEvent>();

            if (originalEvent.RecurrenceType == RecurrenceType.None || !originalEvent.RecurrenceEndTime.HasValue)
                return result;

            var currentStart = originalEvent.StartTime;
            var currentEnd = originalEvent.EndTime;
            var recurrenceEnd = originalEvent.RecurrenceEndTime.Value;

            while (true)
            {
                // Calculate next occurrence
                if (originalEvent.IsLunar)
                {
                    // Convert current solar to lunar rep, advance lunar, convert back to solar
                    var lunarRepStart = ConvertSolarToLunarRepresentation(currentStart);
                    var lunarRepEnd = currentEnd.HasValue ? ConvertSolarToLunarRepresentation(currentEnd.Value) : lunarRepStart;

                    DateTimeOffset advancedLunarStart;
                    DateTimeOffset advancedLunarEnd;

                    if (originalEvent.RecurrenceType == RecurrenceType.Monthly)
                    {
                        advancedLunarStart = new DateTimeOffset(lunarRepStart.DateTime.AddMonths(1), lunarRepStart.Offset);
                        advancedLunarEnd = new DateTimeOffset(lunarRepEnd.DateTime.AddMonths(1), lunarRepEnd.Offset);
                    }
                    else if (originalEvent.RecurrenceType == RecurrenceType.Yearly)
                    {
                        advancedLunarStart = new DateTimeOffset(lunarRepStart.DateTime.AddYears(1), lunarRepStart.Offset);
                        advancedLunarEnd = new DateTimeOffset(lunarRepEnd.DateTime.AddYears(1), lunarRepEnd.Offset);
                    }
                    else // Daily
                    {
                        advancedLunarStart = new DateTimeOffset(lunarRepStart.DateTime.AddDays(1), lunarRepStart.Offset);
                        advancedLunarEnd = new DateTimeOffset(lunarRepEnd.DateTime.AddDays(1), lunarRepEnd.Offset);
                    }

                    // Convert back to solar
                    currentStart = ConvertLunarRepresentationToSolar(advancedLunarStart);
                    currentEnd = ConvertLunarRepresentationToSolar(advancedLunarEnd);
                }
                else
                {
                    switch (originalEvent.RecurrenceType)
                    {
                        case RecurrenceType.Daily:
                            currentStart = currentStart.AddDays(1);
                            currentEnd = currentEnd?.AddDays(1);
                            break;
                        case RecurrenceType.Monthly:
                            currentStart = currentStart.AddMonths(1);
                            currentEnd = currentEnd?.AddMonths(1);
                            break;
                        case RecurrenceType.Yearly:
                            currentStart = currentStart.AddYears(1);
                            currentEnd = currentEnd?.AddYears(1);
                            break;
                        default:
                            break;
                    }
                }

                // Stop if past recurrence end
                if (currentStart > recurrenceEnd)
                    break;

                // Create a new event instance
                var recurringEvent = new FTFamilyEvent
                {
                    Id = Guid.NewGuid(),
                    Name = originalEvent.Name,
                    EventType = originalEvent.EventType,
                    StartTime = currentStart,
                    EndTime = currentEnd,
                    Location = originalEvent.Location,
                    RecurrenceType = RecurrenceType.None, // Instances don't recur themselves
                    FTId = originalEvent.FTId,
                    Description = originalEvent.Description,
                    ImageUrl = originalEvent.ImageUrl,
                    ReferenceEventId = originalEvent.Id, // Reference to original
                    Address = originalEvent.Address,
                    LocationName = originalEvent.LocationName,
                    IsAllDay = originalEvent.IsAllDay,
                    RecurrenceEndTime = null, // No recurrence for instances
                    IsLunar = originalEvent.IsLunar,
                    TargetMemberId = originalEvent.TargetMemberId,
                    IsPublic = originalEvent.IsPublic,
                    CreatedOn = originalEvent.CreatedOn,
                    CreatedByUserId = originalEvent.CreatedByUserId,
                    IsDeleted = false
                };

                result.Add(recurringEvent);
            }

            return result;
        }

        private FTFamilyEventDto MapToDto(FTFamilyEvent eventEntity)
        {
            // Stored times are already in the correct format: solar for solar events, lunar rep for lunar events
            DateTimeOffset displayStart = eventEntity.StartTime;
            DateTimeOffset? displayEnd = eventEntity.EndTime;
            DateTimeOffset? displayRecurrenceEnd = eventEntity.RecurrenceEndTime;

            return new FTFamilyEventDto
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                EventType = eventEntity.EventType,
                StartTime = displayStart,
                EndTime = displayEnd,
                Location = eventEntity.Location,
                RecurrenceType = eventEntity.RecurrenceType,
                FTId = eventEntity.FTId,
                Description = eventEntity.Description,
                ImageUrl = eventEntity.ImageUrl,
                ReferenceEventId = eventEntity.ReferenceEventId,
                Address = eventEntity.Address,
                LocationName = eventEntity.LocationName,
                IsAllDay = eventEntity.IsAllDay,
                RecurrenceEndTime = displayRecurrenceEnd,
                IsLunar = eventEntity.IsLunar,
                TargetMemberId = eventEntity.TargetMemberId,
                TargetMemberName = eventEntity.TargetMember?.Fullname,
                IsPublic = eventEntity.IsPublic,
                CreatedOn = eventEntity.CreatedOn,
                LastModifiedOn = eventEntity.LastModifiedOn,
                EventMembers = eventEntity.EventMembers?
                    .Where(em => em.IsDeleted == false)
                    .Select(em => new FTFamilyEventMemberDto
                    {
                        Id = em.Id,
                        FTMemberId = em.FTMemberId,
                        MemberName = em.FTMember?.Fullname ?? "Unknown",
                        MemberPicture = em.FTMember?.Picture,
                        UserId = em.UserId
                    }).ToList() ?? new List<FTFamilyEventMemberDto>()
            };
        }

        public async Task<Dictionary<string, List<FTFamilyEventDto>>> GetEventsGroupedByPeriodAsync(Guid FTId, Period period, DateTimeOffset? startDate = null, DateTimeOffset? endDate = null)
        {
            var query = _context.FTFamilyEvents
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Where(e => e.FTId == FTId && e.IsDeleted == false);

            if (startDate.HasValue)
                query = query.Where(e => e.StartTime >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(e => e.StartTime <= endDate.Value);

            var events = await query.ToListAsync();

            var grouped = new Dictionary<string, List<FTFamilyEventDto>>();

            foreach (var evt in events)
            {
                string key;
                var date = evt.StartTime;

                switch (period)
                {
                    case Period.Year:
                        key = date.Year.ToString();
                        break;
                    case Period.Month:
                        key = $"{date.Year}-{date.Month:D2}";
                        break;
                    case Period.Week:
                        var cal = CultureInfo.CurrentCulture.Calendar;
                        var week = cal.GetWeekOfYear(date.DateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                        key = $"{date.Year}-{date.Month:D2}-W{week:D2}";
                        break;
                    case Period.Day:
                        key = date.ToString("yyyy-MM-dd");
                        break;
                    default:
                        key = date.Year.ToString();
                        break;
                }

                if (!grouped.ContainsKey(key))
                    grouped[key] = new List<FTFamilyEventDto>();

                grouped[key].Add(MapToDto(evt));
            }

            return grouped;
        }

        public async Task<List<FTFamilyEventDto>> GetEventsGroupedByYearAsync(Guid FTId, int year)
        {
            var startDate = new DateTimeOffset(new DateTime(year, 1, 1));
            var endDate = new DateTimeOffset(new DateTime(year, 12, 31, 23, 59, 59));
            var events = await _eventRepository.GetEventsByDateRangeAsync(FTId, startDate, endDate);
            return events.Select(e => MapToDto(e)).ToList();
        }

        public async Task<List<FTFamilyEventDto>> GetEventsGroupedByMonthAsync(Guid FTId, int year, int month)
        {
            var startDate = new DateTimeOffset(new DateTime(year, month, 1));
            var endDate = new DateTimeOffset(new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59));
            var events = await _eventRepository.GetEventsByDateRangeAsync(FTId, startDate, endDate);
            return events.Select(e => MapToDto(e)).ToList();
        }

        public async Task<List<FTFamilyEventDto>> GetEventsGroupedByWeekAsync(Guid FTId, int year, int month, int week)
        {
            // Calculate start and end of the week
            var firstDayOfMonth = new DateTime(year, month, 1);
            var cal = CultureInfo.CurrentCulture.Calendar;
            var firstWeek = cal.GetWeekOfYear(firstDayOfMonth, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var daysToAdd = (week - firstWeek) * 7;
            var startOfWeek = firstDayOfMonth.AddDays(daysToAdd - (int)firstDayOfMonth.DayOfWeek + (int)DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);

            var startDate = new DateTimeOffset(startOfWeek);
            var endDate = new DateTimeOffset(endOfWeek);

            var events = await _eventRepository.GetEventsByDateRangeAsync(FTId, startDate, endDate);
            return events.Select(e => MapToDto(e)).ToList();
        }
    }
}

