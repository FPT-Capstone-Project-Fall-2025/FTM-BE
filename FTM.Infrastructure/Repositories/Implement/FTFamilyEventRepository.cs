using FTM.Domain.Entities.Events;
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
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Include(e => e.EventFTs)
                .Where(e => e.FTId == ftId && e.IsDeleted == false)
                .OrderBy(e => e.StartTime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountEventsByFTIdAsync(Guid ftId)
        {
            return await Context.FTFamilyEvents
                .Where(e => e.FTId == ftId && e.IsDeleted == false)
                .CountAsync();
        }

        public async Task<FTFamilyEvent> GetEventWithDetailsAsync(Guid eventId)
        {
            return await Context.FTFamilyEvents
                .Include(e => e.FT)
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Include(e => e.EventFTs)
                    .ThenInclude(eg => eg.FT)
                .FirstOrDefaultAsync(e => e.Id == eventId && e.IsDeleted == false);
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetUpcomingEventsAsync(Guid ftId, int days = 30)
        {
            var now = DateTimeOffset.UtcNow;
            var futureDate = now.AddDays(days);

            return await Context.FTFamilyEvents
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Where(e => e.FTId == ftId 
                    && e.IsDeleted == false 
                    && e.StartTime >= now 
                    && e.StartTime <= futureDate)
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetEventsByDateRangeAsync(Guid ftId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await Context.FTFamilyEvents
                .Include(e => e.EventMembers)
                    .ThenInclude(em => em.FTMember)
                .Where(e => e.FTId == ftId 
                    && e.IsDeleted == false 
                    && e.StartTime >= startDate 
                    && e.StartTime <= endDate)
                .OrderBy(e => e.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFamilyEvent>> GetEventsByMemberIdAsync(Guid memberId, int skip = 0, int take = 20)
        {
            return await Context.FTFamilyEventMembers
                .Include(em => em.FTFamilyEvent)
                    .ThenInclude(e => e.EventMembers)
                        .ThenInclude(m => m.FTMember)
                .Where(em => em.FTMemberId == memberId && em.IsDeleted == false && em.FTFamilyEvent.IsDeleted == false)
                .Select(em => em.FTFamilyEvent)
                .OrderBy(e => e.StartTime)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<bool> IsMemberInEventAsync(Guid eventId, Guid memberId)
        {
            return await Context.FTFamilyEventMembers
                .AnyAsync(em => em.FTFamilyEventId == eventId 
                    && em.FTMemberId == memberId 
                    && em.IsDeleted == false);
        }
    }
}
