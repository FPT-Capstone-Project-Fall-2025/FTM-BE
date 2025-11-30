using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTM.Application.IServices
{
    public interface IFTFamilyEventService
    {
        // CRUD operations
        Task<FTFamilyEventDto> CreateEventAsync(CreateFTFamilyEventRequest request);
        Task<FTFamilyEventDto> UpdateEventAsync(Guid id, UpdateFTFamilyEventRequest request);
        Task<bool> DeleteEventAsync(Guid eventId);
        Task<FTFamilyEventDto?> GetEventByIdAsync(Guid eventId);
        
        // Query operations
        Task<IEnumerable<FTFamilyEventDto>> GetEventsByFTIdAsync(Guid ftId, int skip = 0, int take = 20);
        Task<int> CountEventsByFTIdAsync(Guid ftId);
        Task<IEnumerable<FTFamilyEventDto>> GetUpcomingEventsAsync(Guid ftId, int days = 30);
        Task<IEnumerable<FTFamilyEventDto>> GetEventsByDateRangeAsync(Guid ftId, DateTimeOffset startDate, DateTimeOffset endDate);
        Task<IEnumerable<FTFamilyEventDto>> GetEventsByMemberIdAsync(Guid memberId, int skip = 0, int take = 20);
        Task<IEnumerable<FTFamilyEventDto>> FilterEventsAsync(EventFilterRequest request);
        
        // Grouped events
        // Separate grouped APIs
        Task<List<FTFamilyEventDto>> GetEventsGroupedByYearAsync(Guid FTId, int year);
        Task<List<FTFamilyEventDto>> GetEventsGroupedByMonthAsync(Guid FTId, int year, int month);
        Task<List<FTFamilyEventDto>> GetEventsGroupedByWeekAsync(Guid FTId, int year, int month, int week);
        
        // User events
        Task<IEnumerable<FTFamilyEventDto>> GetMyEventsAsync(Guid userId, Guid ftId, int skip = 0, int take = 20);
        Task<IEnumerable<FTFamilyEventDto>> GetMyUpcomingEventsAsync(Guid userId, Guid ftId, int days = 30);
        
        // Member management
        Task<bool> AddMemberToEventAsync(Guid eventId, Guid memberId);
        Task<bool> RemoveMemberFromEventAsync(Guid eventId, Guid memberId);
        Task<IEnumerable<FTFamilyEventMemberDto>> GetEventMembersAsync(Guid eventId);
    }
}
