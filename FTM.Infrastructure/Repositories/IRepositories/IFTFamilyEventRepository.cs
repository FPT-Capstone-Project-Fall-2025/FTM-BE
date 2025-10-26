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
    }
}
