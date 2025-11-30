using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.Events;
using FTM.Domain.Enums;
using FTM.Domain.Specification;
using FTM.Domain.Specification.FamilyTrees;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories
{
    public class FTFamilyEventRepository : GenericRepository<FTFamilyEvent>, IFTFamilyEventRepository
    {
        public FTFamilyEventRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver)
            : base(context, currentUserResolver)
        {
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetEventsByFTIdAsync(Guid ftId, int skip = 0, int take = 20)
        {
            return await Context.FTFamilyEvents
                .Where(e => e.FTId == ftId && e.IsDeleted == false)
                .OrderBy(e => e.StartTime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountEventsByFTIdAsync(Guid ftId)
        {
            return await Context.FTFamilyEvents
                .CountAsync(e => e.FTId == ftId && e.IsDeleted == false);
        }

        public async Task<FTFamilyEvent> GetEventWithDetailsAsync(Guid eventId)
        {
            return await Context.FTFamilyEvents
                .Include(e => e.FT)
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Include(e => e.EventFTs)
                    .ThenInclude(ef => ef.FT)
                .FirstOrDefaultAsync(e => e.Id == eventId && e.IsDeleted == false);
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetUpcomingEventsAsync(Guid ftId, int days = 30)
        {
            var now = DateTimeOffset.UtcNow;
            var endDate = now.AddDays(days);
            return await Context.FTFamilyEvents
                .Where(e => e.FTId == ftId && e.IsDeleted == false
                    && e.StartTime >= now && e.StartTime <= endDate)
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetEventsByDateRangeAsync(Guid ftId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await Context.FTFamilyEvents
                .Where(e => e.FTId == ftId && e.IsDeleted == false
                    && e.StartTime >= startDate && e.StartTime <= endDate)
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetEventsByMemberIdAsync(Guid memberId, int skip = 0, int take = 20)
        {
            return await Context.FTFamilyEventMembers
                .Include(em => em.FTFamilyEvent)
                .Where(em => em.FTMemberId == memberId && em.IsDeleted == false)
                .Select(em => em.FTFamilyEvent)
                .Where(e => e.IsDeleted == false)
                .OrderBy(e => e.StartTime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetFilteredEventsAsync(EventFilterRequest request)
        {
            var query = Context.FTFamilyEvents
                .Where(e => e.FTId == request.FTId && e.IsDeleted == false);

            if (request.StartDate.HasValue)
            {
                query = query.Where(e => e.StartTime >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(e => e.StartTime <= request.EndDate.Value);
            }

            if (request.FTMemberId.HasValue)
            {
                query = query.Where(e => e.EventMembers.Any(em => em.FTMemberId == request.FTMemberId.Value && em.IsDeleted == false));
            }

            if (!string.IsNullOrEmpty(request.EventType))
            {
                var eventType = Enum.Parse<FTM.Domain.Enums.EventType>(request.EventType);
                query = query.Where(e => e.EventType == eventType);
            }

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(e => e.Name.Contains(request.SearchTerm) || e.Description.Contains(request.SearchTerm));
            }

            if (request.IsLunar.HasValue)
            {
                query = query.Where(e => e.IsLunar == request.IsLunar.Value);
            }

            return await query
                .OrderBy(e => e.StartTime)
                .Skip(request.Skip ?? 0)
                .Take(request.Take ?? 20)
                .ToListAsync();
        }

        public async Task<bool> IsMemberInEventAsync(Guid eventId, Guid memberId)
        {
            return await Context.FTFamilyEventMembers
                .AnyAsync(em => em.FTFamilyEventId == eventId
                    && em.FTMemberId == memberId
                    && em.IsDeleted == false);
        }

        public async Task AddEventWithRelationsAsync(FTFamilyEvent eventEntity, IEnumerable<FTFamilyEventFT> eventFTs, IEnumerable<FTFamilyEventMember> eventMembers)
        {
            await Context.FTFamilyEvents.AddAsync(eventEntity);
            await Context.FTFamilyEventFTs.AddRangeAsync(eventFTs);
            await Context.FTFamilyEventMembers.AddRangeAsync(eventMembers);
            await Context.SaveChangesAsync();
        }

        public async Task UpdateEventMembersAsync(Guid eventId, IEnumerable<FTFamilyEventMember> newMembers)
        {
            // Remove existing members
            var existingMembers = await Context.FTFamilyEventMembers
                .Where(em => em.FTFamilyEventId == eventId && em.IsDeleted == false)
                .ToListAsync();

            foreach (var member in existingMembers)
            {
                member.IsDeleted = true;
                member.LastModifiedOn = DateTimeOffset.UtcNow;
            }

            // Add new members
            await Context.FTFamilyEventMembers.AddRangeAsync(newMembers);
            await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetEventsByFTIdWithIncludesAsync(Guid ftId, int skip = 0, int take = 20)
        {
            return await Context.FTFamilyEvents
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Where(e => e.FTId == ftId && e.IsDeleted == false)
                .OrderBy(e => e.StartTime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetUpcomingEventsWithIncludesAsync(Guid ftId, int days = 30)
        {
            var now = DateTimeOffset.UtcNow;
            var endDate = now.AddDays(days);
            return await Context.FTFamilyEvents
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Where(e => e.FTId == ftId && e.IsDeleted == false
                    && e.StartTime >= now && e.StartTime <= endDate)
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetEventsByDateRangeWithIncludesAsync(Guid ftId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await Context.FTFamilyEvents
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Where(e => e.FTId == ftId && e.IsDeleted == false
                    && e.StartTime >= startDate && e.StartTime <= endDate)
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetEventsByMemberWithIncludesAsync(Guid memberId, int skip = 0, int take = 20)
        {
            return await Context.FTFamilyEventMembers
                .Include(em => em.FTFamilyEvent)
                    .ThenInclude(e => e.EventMembers)
                        .ThenInclude(em => em.FTMember)
                .Where(em => em.FTMemberId == memberId && em.IsDeleted == false)
                .Select(em => em.FTFamilyEvent)
                .Where(e => e.IsDeleted == false)
                .OrderBy(e => e.StartTime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetFilteredEventsWithIncludesAsync(EventFilterRequest request)
        {
            var query = Context.FTFamilyEvents
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

            if (request.FTMemberId.HasValue)
            {
                query = query.Where(e => e.EventMembers.Any(em => em.FTMemberId == request.FTMemberId.Value && em.IsDeleted == false));
            }

            if (!string.IsNullOrEmpty(request.EventType))
            {
                var eventType = Enum.Parse<FTM.Domain.Enums.EventType>(request.EventType);
                query = query.Where(e => e.EventType == eventType);
            }

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(e => e.Name.Contains(request.SearchTerm) || e.Description.Contains(request.SearchTerm));
            }

            if (request.IsLunar.HasValue)
            {
                query = query.Where(e => e.IsLunar == request.IsLunar.Value);
            }

            return await query
                .OrderBy(e => e.StartTime)
                .Skip(request.Skip ?? 0)
                .Take(request.Take ?? 20)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetMyEventsAsync(Guid userId, Guid ftId, int skip = 0, int take = 20)
        {
            // Tìm FTMember của user trong family tree này
            var member = await Context.FTMembers
                .FirstOrDefaultAsync(m => m.UserId == userId && m.FTId == ftId && m.IsDeleted == false);

            if (member == null)
                return new List<FTFamilyEvent>();

            // Lấy tất cả các sự kiện mà member này được tag (không giới hạn thời gian)
            var events = await Context.FTFamilyEventMembers
                .Include(em => em.FTFamilyEvent)
                    .ThenInclude(e => e.EventMembers)
                        .ThenInclude(em => em.FTMember)
                .Where(em => em.FTMemberId == member.Id && em.IsDeleted == false)
                .Select(em => em.FTFamilyEvent)
                .Where(e => e.IsDeleted == false && e.FTId == ftId)
                .OrderByDescending(e => e.StartTime) // Sắp xếp theo thời gian mới nhất trước
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return events;
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetMyUpcomingEventsAsync(Guid userId, Guid ftId, int days = 30)
        {
            // Tìm FTMember của user trong family tree này
            var member = await Context.FTMembers
                .FirstOrDefaultAsync(m => m.UserId == userId && m.FTId == ftId && m.IsDeleted == false);

            if (member == null)
                return new List<FTFamilyEvent>();

            var now = DateTimeOffset.UtcNow;
            var endDate = now.AddDays(days);

            // Lấy các sự kiện sắp tới mà member này được tag
            // Bao gồm: sự kiện bắt đầu trong tương lai HOẶC đang diễn ra (còn EndTime trong tương lai)
            var events = await Context.FTFamilyEventMembers
                .Include(em => em.FTFamilyEvent)
                    .ThenInclude(e => e.EventMembers)
                        .ThenInclude(em => em.FTMember)
                .Where(em => em.FTMemberId == member.Id && em.IsDeleted == false)
                .Select(em => em.FTFamilyEvent)
                .Where(e => e.IsDeleted == false && e.FTId == ftId)
                .Where(e => e.StartTime >= now || (e.EndTime.HasValue && e.EndTime.Value >= now)) // Sự kiện bắt đầu trong tương lai HOẶC đang diễn ra
                .Where(e => e.StartTime <= endDate) // Giới hạn trong khoảng thời gian yêu cầu
                .OrderBy(e => e.StartTime)
                .ToListAsync();

            return events;
        }

        public async Task<IEnumerable<FTFamilyEventMemberDto>> GetEventMembersAsync(Guid eventId)
        {
            var members = await Context.FTFamilyEventMembers
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

        public async Task<bool> AddMemberToEventAsync(Guid eventId, Guid memberId, Guid currentUserId)
        {
            // Check if member already in event
            if (await IsMemberInEventAsync(eventId, memberId))
                return false;

            var eventMember = new FTFamilyEventMember
            {
                Id = Guid.NewGuid(),
                FTFamilyEventId = eventId,
                FTMemberId = memberId,
                CreatedOn = DateTimeOffset.UtcNow,
                CreatedByUserId = currentUserId,
                IsDeleted = false
            };

            await Context.FTFamilyEventMembers.AddAsync(eventMember);
            await Context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveMemberFromEventAsync(Guid eventId, Guid memberId)
        {
            var eventMember = await Context.FTFamilyEventMembers
                .FirstOrDefaultAsync(em => em.FTFamilyEventId == eventId
                    && em.FTMemberId == memberId
                    && em.IsDeleted == false);

            if (eventMember == null)
                return false;

            eventMember.IsDeleted = true;
            eventMember.LastModifiedOn = DateTimeOffset.UtcNow;

            await Context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> HardDeleteEventAsync(Guid eventId)
        {
            var eventEntity = await Context.FTFamilyEvents
                .Include(e => e.EventMembers)
                .Include(e => e.EventFTs)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventEntity == null)
                return false;

            // Hard delete all related members
            Context.FTFamilyEventMembers.RemoveRange(eventEntity.EventMembers);

            // Hard delete all related FTs
            Context.FTFamilyEventFTs.RemoveRange(eventEntity.EventFTs);

            // Hard delete the main event
            Context.FTFamilyEvents.Remove(eventEntity);

            await Context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> FamilyTreeExistsAsync(Guid ftId)
        {
            return await Context.FamilyTrees.AnyAsync(ft => ft.Id == ftId && ft.IsDeleted == false);
        }

        public async Task<bool> MemberExistsAsync(Guid memberId)
        {
            return await Context.FTMembers.AnyAsync(m => m.Id == memberId && m.IsDeleted == false);
        }

        public async Task<bool> MemberBelongsToFTAsync(Guid memberId, Guid ftId)
        {
            return await Context.FTMembers.AnyAsync(m => m.Id == memberId && m.FTId == ftId && m.IsDeleted == false);
        }

        public async Task<bool> UserIsMemberOfFTAsync(Guid userId, Guid ftId)
        {
            return await Context.FTMembers.AnyAsync(m => m.UserId == userId && m.FTId == ftId && m.IsDeleted == false);
        }

        public async Task<Guid?> GetMemberIdByUserIdAndFTIdAsync(Guid userId, Guid ftId)
        {
            var member = await Context.FTMembers
                .Where(m => m.UserId == userId && m.FTId == ftId && m.IsDeleted == false)
                .Select(m => m.Id)
                .FirstOrDefaultAsync();
            
            return member == Guid.Empty ? null : member;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetRecurringEventInstancesAsync(Guid originalEventId)
        {
            // First try to find by ReferenceEventId
            var instances = await Context.FTFamilyEvents
                .Where(e => e.ReferenceEventId == originalEventId && e.IsDeleted == false)
                .OrderBy(e => e.StartTime)
                .ToListAsync();

            if (instances.Any())
            {
                return instances;
            }

            // If no instances found by ReferenceEventId, try to find by metadata
            // Get the original event to get its metadata
            var originalEvent = await Context.FTFamilyEvents
                .FirstOrDefaultAsync(e => e.Id == originalEventId && e.IsDeleted == false);

            if (originalEvent != null && originalEvent.RecurrenceType != RecurrenceType.None && originalEvent.RecurrenceEndTime.HasValue)
            {
                // Find events with same FTId, Name, EventType, and within recurrence period
                var metadataInstances = await Context.FTFamilyEvents
                    .Where(e => e.FTId == originalEvent.FTId &&
                               e.Name == originalEvent.Name &&
                               e.EventType == originalEvent.EventType &&
                               e.ReferenceEventId != null && // Has ReferenceEventId (is instance)
                               e.StartTime >= originalEvent.StartTime &&
                               e.StartTime <= originalEvent.RecurrenceEndTime.Value &&
                               e.IsDeleted == false &&
                               e.Id != originalEventId) // Exclude original
                    .OrderBy(e => e.StartTime)
                    .ToListAsync();

                return metadataInstances;
            }

            return new List<FTFamilyEvent>();
        }
    }
}
