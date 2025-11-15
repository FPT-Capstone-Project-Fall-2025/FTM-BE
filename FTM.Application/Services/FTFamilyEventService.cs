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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBlobStorageService _blobStorageService;

        public FTFamilyEventService(
            IFTFamilyEventRepository eventRepository,
            IFTAuthorizationRepository authorizationRepository,
            IHttpContextAccessor httpContextAccessor,
            IBlobStorageService blobStorageService)
        {
            _eventRepository = eventRepository;
            _authorizationRepository = authorizationRepository;
            _httpContextAccessor = httpContextAccessor;
            _blobStorageService = blobStorageService;
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
            var familyTreeExists = await _eventRepository.FamilyTreeExistsAsync(request.FTId);
            if (!familyTreeExists)
                throw new Exception($"Family tree with ID {request.FTId} not found");

            // Validate user permissions
            var userIsMember = await _eventRepository.UserIsMemberOfFTAsync(currentUserId, request.FTId);
            if (!userIsMember && currentUserRole != "GPOwner")
                throw new Exception("User is not a member of this family tree");

            // Check permission to create events
            if (userIsMember)
            {
                var memberId = await _eventRepository.GetMemberIdByUserIdAndFTIdAsync(currentUserId, request.FTId);
                if (memberId.HasValue)
                {
                    //var hasPermission = await _authorizationRepository.IsAuthorizationExisting(
                    //    request.FTId, memberId.Value, FeatureType.EVENT, MethodType.ADD);
                    
                    //if (!hasPermission && currentUserRole != "GPOwner")
                    //    throw new Exception("User does not have permission to create events in this family tree");
                }
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
                var memberExists = await _eventRepository.MemberExistsAsync(request.TargetMemberId.Value);
                if (!memberExists)
                    throw new Exception($"Target member with ID {request.TargetMemberId} not found");
                
                var memberBelongsToFT = await _eventRepository.MemberBelongsToFTAsync(request.TargetMemberId.Value, request.FTId);
                if (!memberBelongsToFT)
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
                    var memberExists = await _eventRepository.MemberExistsAsync(memberId);
                    if (!memberExists)
                        throw new Exception($"Member with ID {memberId} not found");
                    
                    var memberBelongsToFT = await _eventRepository.MemberBelongsToFTAsync(memberId, request.FTId);
                    if (!memberBelongsToFT)
                        throw new Exception($"Member with ID {memberId} does not belong to this family tree");
                }
            }

            // Handle image upload if ImageFile is provided
            string imageUrl = string.Empty;
            if (request.ImageFile != null)
            {
                imageUrl = await _blobStorageService.UploadEventImageAsync(request.ImageFile, "events");
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
                ImageUrl = imageUrl,
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

            // Prepare relations
            var eventFTs = new List<FTFamilyEventFT>
            {
                new FTFamilyEventFT
                {
                    Id = Guid.NewGuid(),
                    FTFamilyEventId = eventEntity.Id,
                    FTId = request.FTId,
                    CreatedOn = DateTimeOffset.UtcNow,
                    CreatedByUserId = currentUserId,
                    IsDeleted = false
                }
            };

            var eventMembers = new List<FTFamilyEventMember>();

            // Add target member if personal event
            if (request.TargetMemberId.HasValue)
            {
                eventMembers.Add(new FTFamilyEventMember
                {
                    Id = Guid.NewGuid(),
                    FTFamilyEventId = eventEntity.Id,
                    FTMemberId = request.TargetMemberId.Value,
                    CreatedOn = DateTimeOffset.UtcNow,
                    CreatedByUserId = currentUserId,
                    IsDeleted = false
                });
            }

            // Add members if provided
            if (request.MemberIds != null && request.MemberIds.Any())
            {
                foreach (var memberId in request.MemberIds)
                {
                    // Skip if already added as target member
                    if (request.TargetMemberId.HasValue && memberId == request.TargetMemberId.Value)
                        continue;

                    eventMembers.Add(new FTFamilyEventMember
                    {
                        Id = Guid.NewGuid(),
                        FTFamilyEventId = eventEntity.Id,
                        FTMemberId = memberId,
                        CreatedOn = DateTimeOffset.UtcNow,
                        CreatedByUserId = currentUserId,
                        IsDeleted = false
                    });
                }
            }

            // Save main event with relations
            await _eventRepository.AddEventWithRelationsAsync(eventEntity, eventFTs, eventMembers);

            // Generate and save recurring events if applicable
            if (eventEntity.RecurrenceType != RecurrenceType.None && eventEntity.RecurrenceEndTime.HasValue)
            {
                var recurringEvents = GenerateRecurringEventsForSave(eventEntity);
                foreach (var recEvent in recurringEvents)
                {
                    var recEventFTs = new List<FTFamilyEventFT>
                    {
                        new FTFamilyEventFT
                        {
                            Id = Guid.NewGuid(),
                            FTFamilyEventId = recEvent.Id,
                            FTId = request.FTId,
                            CreatedOn = DateTimeOffset.UtcNow,
                            CreatedByUserId = currentUserId,
                            IsDeleted = false
                        }
                    };

                    var recEventMembers = new List<FTFamilyEventMember>();

                    // Add target member if personal event
                    if (request.TargetMemberId.HasValue)
                    {
                        recEventMembers.Add(new FTFamilyEventMember
                        {
                            Id = Guid.NewGuid(),
                            FTFamilyEventId = recEvent.Id,
                            FTMemberId = request.TargetMemberId.Value,
                            CreatedOn = DateTimeOffset.UtcNow,
                            CreatedByUserId = currentUserId,
                            IsDeleted = false
                        });
                    }

                    // Add members if provided
                    if (request.MemberIds != null && request.MemberIds.Any())
                    {
                        foreach (var memberId in request.MemberIds)
                        {
                            // Skip if already added as target member
                            if (request.TargetMemberId.HasValue && memberId == request.TargetMemberId.Value)
                                continue;

                            recEventMembers.Add(new FTFamilyEventMember
                            {
                                Id = Guid.NewGuid(),
                                FTFamilyEventId = recEvent.Id,
                                FTMemberId = memberId,
                                CreatedOn = DateTimeOffset.UtcNow,
                                CreatedByUserId = currentUserId,
                                IsDeleted = false
                            });
                        }
                    }

                    await _eventRepository.AddEventWithRelationsAsync(recEvent, recEventFTs, recEventMembers);
                }
            }

            return await GetEventByIdAsync(eventEntity.Id);
        }

        public async Task<FTFamilyEventDto> UpdateEventAsync(Guid id, UpdateFTFamilyEventRequest request)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
                throw new Exception("Event not found");

            Guid seriesOriginalEventId;
            FTFamilyEvent seriesOriginalEvent;

            // Determine if this is an original recurring event or an instance
            if (eventEntity.RecurrenceType != RecurrenceType.None && eventEntity.RecurrenceEndTime.HasValue)
            {
                // This is the original event
                seriesOriginalEventId = id;
                seriesOriginalEvent = eventEntity;
            }
            else if (eventEntity.ReferenceEventId.HasValue)
            {
                // This is an instance, find the original event
                seriesOriginalEventId = eventEntity.ReferenceEventId.Value;
                seriesOriginalEvent = await _eventRepository.GetByIdAsync(seriesOriginalEventId);
                if (seriesOriginalEvent == null || seriesOriginalEvent.RecurrenceType == RecurrenceType.None)
                {
                    // Original event not found or not recurring, treat as single event
                    seriesOriginalEventId = id;
                    seriesOriginalEvent = eventEntity;
                }
            }
            else
            {
                // Single non-recurring event
                seriesOriginalEventId = id;
                seriesOriginalEvent = eventEntity;
            }

            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            // Validate user permissions
            var userIsMember = await _eventRepository.UserIsMemberOfFTAsync(currentUserId, eventEntity.FTId);
            if (!userIsMember && currentUserRole != "GPOwner")
                throw new Exception("User is not a member of this family tree");

            // Check permission to update events
            if (userIsMember)
            {
                var memberId = await _eventRepository.GetMemberIdByUserIdAndFTIdAsync(currentUserId, eventEntity.FTId);
                if (memberId.HasValue)
                {
                    var hasPermission = await _authorizationRepository.IsAuthorizationExisting(
                        eventEntity.FTId, memberId.Value, FeatureType.EVENT, MethodType.UPDATE);
                    
                    if (!hasPermission && currentUserRole != "GPOwner")
                        throw new Exception("User does not have permission to update events in this family tree");
                }
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
                var memberExists = await _eventRepository.MemberExistsAsync(request.TargetMemberId.Value);
                if (!memberExists)
                    throw new Exception($"Target member with ID {request.TargetMemberId} not found");

                var memberBelongsToFT = await _eventRepository.MemberBelongsToFTAsync(request.TargetMemberId.Value, eventEntity.FTId);
                if (!memberBelongsToFT)
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
                    var memberExists = await _eventRepository.MemberExistsAsync(memberId);
                    if (!memberExists)
                        throw new Exception($"Member with ID {memberId} not found");

                    var memberBelongsToFT = await _eventRepository.MemberBelongsToFTAsync(memberId, eventEntity.FTId);
                    if (!memberBelongsToFT)
                        throw new Exception($"Member with ID {memberId} does not belong to this family tree");
                }
            }

            // Handle image upload if ImageFile is provided
            if (request.ImageFile != null)
            {
                // Upload new image
                var newImageUrl = await _blobStorageService.UploadEventImageAsync(request.ImageFile, "events");

                // Store old image URL for potential cleanup
                var oldImageUrl = eventEntity.ImageUrl;

                // Update with new image URL
                eventEntity.ImageUrl = newImageUrl;

                // TODO: Optionally delete old image from blob storage if it exists
                // This would require extracting the blob path from the old URL
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
                var newMembers = request.MemberIds.Select(memberId => new FTFamilyEventMember
                {
                    Id = Guid.NewGuid(),
                    FTFamilyEventId = id,
                    FTMemberId = memberId,
                    CreatedOn = DateTimeOffset.UtcNow,
                    CreatedByUserId = currentUserId,
                    IsDeleted = false
                }).ToList();

                await _eventRepository.UpdateEventMembersAsync(id, newMembers);
            }

            // If this is a recurring event series, update all events in the series
            if (seriesOriginalEvent.RecurrenceType != RecurrenceType.None && seriesOriginalEvent.RecurrenceEndTime.HasValue)
            {
                // Get all events in the series: original + all instances
                var allEventsInSeries = new List<FTFamilyEvent> { seriesOriginalEvent };
                var recurringInstances = await _eventRepository.GetRecurringEventInstancesAsync(seriesOriginalEventId);
                allEventsInSeries.AddRange(recurringInstances);

                // Update all events in the series with metadata changes (keep original times for instances)
                foreach (var seriesEvent in allEventsInSeries.Where(e => e.Id != id)) // Exclude current event as it's already updated
                {
                    // Update metadata fields for instances (keep original times)
                    if (request.Name != null)
                        seriesEvent.Name = request.Name;
                    if (request.EventType.HasValue)
                        seriesEvent.EventType = (FTM.Domain.Enums.EventType)request.EventType.Value;
                    if (request.Location != null)
                        seriesEvent.Location = request.Location;
                    if (request.Description != null)
                        seriesEvent.Description = request.Description;
                    if (request.Address != null)
                        seriesEvent.Address = request.Address;
                    if (request.LocationName != null)
                        seriesEvent.LocationName = request.LocationName;
                    if (request.IsAllDay.HasValue)
                        seriesEvent.IsAllDay = request.IsAllDay.Value;
                    if (request.IsLunar.HasValue)
                        seriesEvent.IsLunar = request.IsLunar.Value;
                    if (request.TargetMemberId.HasValue)
                        seriesEvent.TargetMemberId = request.TargetMemberId.Value;
                    if (request.IsPublic.HasValue)
                        seriesEvent.IsPublic = request.IsPublic.Value;

                    seriesEvent.LastModifiedOn = DateTimeOffset.UtcNow;
                    seriesEvent.LastModifiedBy = currentUserId.ToString();

                    _eventRepository.Update(seriesEvent);

                    // Update members for all events in series if provided
                    if (request.MemberIds != null)
                    {
                        var seriesMembers = request.MemberIds.Select(memberId => new FTFamilyEventMember
                        {
                            Id = Guid.NewGuid(),
                            FTFamilyEventId = seriesEvent.Id,
                            FTMemberId = memberId,
                            CreatedOn = DateTimeOffset.UtcNow,
                            CreatedByUserId = currentUserId,
                            IsDeleted = false
                        }).ToList();

                        await _eventRepository.UpdateEventMembersAsync(seriesEvent.Id, seriesMembers);
                    }
                }
            }

            await _eventRepository.SaveChangesAsync();

            return await GetEventByIdAsync(id);
        }        public async Task<bool> DeleteEventAsync(Guid eventId)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                return false;

            Guid eventToDeleteId = eventId;

            // If this is an instance, find the original event and delete the entire series
            if (eventEntity.ReferenceEventId.HasValue)
            {
                eventToDeleteId = eventEntity.ReferenceEventId.Value;
                // Get the original event
                var originalEvent = await _eventRepository.GetByIdAsync(eventToDeleteId);
                if (originalEvent == null)
                {
                    // If original event not found, just delete this instance
                    return await _eventRepository.HardDeleteEventAsync(eventId);
                }
                eventEntity = originalEvent;
            }

            // Get all recurring instances if this is the original event
            var recurringInstances = new List<FTFamilyEvent>();
            if (!eventEntity.ReferenceEventId.HasValue) // This is the original event
            {
                recurringInstances = (await _eventRepository.GetRecurringEventInstancesAsync(eventEntity.Id)).ToList();
            }

            // Hard delete the main event
            await _eventRepository.HardDeleteEventAsync(eventEntity.Id);

            // Hard delete all recurring instances
            foreach (var instance in recurringInstances)
            {
                await _eventRepository.HardDeleteEventAsync(instance.Id);
            }

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
            var events = await _eventRepository.GetEventsByFTIdWithIncludesAsync(FTId, skip, take);
            return events.Select(e => MapToDto(e)).ToList();
        }

        public async Task<int> CountEventsByFTIdAsync(Guid FTId)
        {
            return await _eventRepository.CountEventsByFTIdAsync(FTId);
        }

        public async Task<IEnumerable<FTFamilyEventDto>> GetUpcomingEventsAsync(Guid FTId, int days = 30)
        {
            var events = await _eventRepository.GetUpcomingEventsWithIncludesAsync(FTId, days);
            return events.Select(e => MapToDto(e)).ToList();
        }

        public async Task<IEnumerable<FTFamilyEventDto>> GetEventsByDateRangeAsync(Guid FTId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            var events = await _eventRepository.GetEventsByDateRangeWithIncludesAsync(FTId, startDate, endDate);
            return events.Select(e => MapToDto(e)).ToList();
        }

        public async Task<IEnumerable<FTFamilyEventDto>> GetEventsByMemberIdAsync(Guid memberId, int skip = 0, int take = 20)
        {
            var events = await _eventRepository.GetEventsByMemberIdAsync(memberId, skip, take);
            return events.Select(e => MapToDto(e)).ToList();
        }

        public async Task<IEnumerable<FTFamilyEventDto>> FilterEventsAsync(EventFilterRequest request)
        {
            var events = await _eventRepository.GetFilteredEventsWithIncludesAsync(request);
            return events.Select(e => MapToDto(e)).ToList();
        }

        public async Task<bool> AddMemberToEventAsync(Guid eventId, Guid memberId)
        {
            var currentUserId = GetCurrentUserId();
            return await _eventRepository.AddMemberToEventAsync(eventId, memberId, currentUserId);
        }

        public async Task<bool> RemoveMemberFromEventAsync(Guid eventId, Guid memberId)
        {
            return await _eventRepository.RemoveMemberFromEventAsync(eventId, memberId);
        }

        public async Task<IEnumerable<FTFamilyEventDto>> GetMyEventsAsync(Guid userId, Guid ftId, int skip = 0, int take = 20)
        {
            var events = await _eventRepository.GetMyEventsAsync(userId, ftId, skip, take);
            return events.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<FTFamilyEventDto>> GetMyUpcomingEventsAsync(Guid userId, Guid ftId, int days = 30)
        {
            var events = await _eventRepository.GetMyUpcomingEventsAsync(userId, ftId, days);
            return events.Select(MapToDto).ToList();
        }

        public async Task<IEnumerable<FTFamilyEventMemberDto>> GetEventMembersAsync(Guid eventId)
        {
            return await _eventRepository.GetEventMembersAsync(eventId);
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

