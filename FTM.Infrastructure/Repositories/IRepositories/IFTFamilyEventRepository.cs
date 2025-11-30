using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.Events;
using FTM.Infrastructure.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IFTFamilyEventRepository : IGenericRepository<FTFamilyEvent>
    {
        Task<IEnumerable<FTFamilyEvent>> GetEventsByFTIdAsync(Guid ftId, int skip = 0, int take = 20);
        Task<int> CountEventsByFTIdAsync(Guid ftId);
        Task<FTFamilyEvent> GetEventWithDetailsAsync(Guid eventId);
        Task<IEnumerable<FTFamilyEvent>> GetUpcomingEventsAsync(Guid ftId, int days = 30);
        Task<IEnumerable<FTFamilyEvent>> GetEventsByDateRangeAsync(Guid ftId, DateTimeOffset startDate, DateTimeOffset endDate);
        Task<IEnumerable<FTFamilyEvent>> GetEventsByMemberIdAsync(Guid memberId, int skip = 0, int take = 20);
        Task<bool> IsMemberInEventAsync(Guid eventId, Guid memberId);
        Task AddEventWithRelationsAsync(FTFamilyEvent eventEntity, IEnumerable<FTFamilyEventFT> eventFTs, IEnumerable<FTFamilyEventMember> eventMembers);
        Task UpdateEventMembersAsync(Guid eventId, IEnumerable<FTFamilyEventMember> newMembers);
        Task<IEnumerable<FTFamilyEvent>> GetEventsByFTIdWithIncludesAsync(Guid ftId, int skip = 0, int take = 20);
        Task<IEnumerable<FTFamilyEvent>> GetUpcomingEventsWithIncludesAsync(Guid ftId, int days = 30);
        Task<IEnumerable<FTFamilyEvent>> GetEventsByDateRangeWithIncludesAsync(Guid ftId, DateTimeOffset startDate, DateTimeOffset endDate);
        Task<IEnumerable<FTFamilyEvent>> GetEventsByMemberWithIncludesAsync(Guid memberId, int skip = 0, int take = 20);
        Task<IEnumerable<FTFamilyEvent>> GetFilteredEventsWithIncludesAsync(FTM.Domain.DTOs.FamilyTree.EventFilterRequest request);
        Task<IEnumerable<FTFamilyEvent>> GetMyEventsAsync(Guid userId, Guid ftId, int skip = 0, int take = 20);
        Task<IEnumerable<FTFamilyEvent>> GetMyUpcomingEventsAsync(Guid userId, Guid ftId, int days = 30);
        Task<IEnumerable<FTFamilyEventMemberDto>> GetEventMembersAsync(Guid eventId);
        Task<bool> AddMemberToEventAsync(Guid eventId, Guid memberId, Guid currentUserId);
        Task<bool> RemoveMemberFromEventAsync(Guid eventId, Guid memberId);
        Task<bool> HardDeleteEventAsync(Guid eventId);
        Task<bool> FamilyTreeExistsAsync(Guid ftId);
        Task<bool> MemberExistsAsync(Guid memberId);
        Task<bool> MemberBelongsToFTAsync(Guid memberId, Guid ftId);
        Task<bool> UserIsMemberOfFTAsync(Guid userId, Guid ftId);
        Task<Guid?> GetMemberIdByUserIdAndFTIdAsync(Guid userId, Guid ftId);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<FTFamilyEvent>> GetRecurringEventInstancesAsync(Guid originalEventId);
    }
}
