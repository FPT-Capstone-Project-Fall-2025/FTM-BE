using FTM.Domain.Entities.Funds;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FTM.Infrastructure.Repositories.Implement
{
    public class FTFundDonationRepository : GenericRepository<FTFundDonation>, IFTFundDonationRepository
    {
        private readonly FTMDbContext _context;

        public FTFundDonationRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) : base(context, currentUserResolver)
        {
            _context = context;
        }

        public async Task<IEnumerable<FTFundDonation>> GetDonationsByFundIdAsync(Guid fundId)
        {
            return await _context.FTFundDonations
                .Include(d => d.Member)
                .Include(d => d.Campaign)
                .Where(d => d.FTFundId == fundId && d.IsDeleted != true)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFundDonation>> GetDonationsByCampaignIdAsync(Guid campaignId)
        {
            return await _context.FTFundDonations
                .Include(d => d.Member)
                .Where(d => d.CampaignId == campaignId && d.IsDeleted != true)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFundDonation>> GetRecentContributorsAsync(Guid fundId, int limit)
        {
            return await _context.FTFundDonations
                .Include(d => d.Member)
                .Where(d => d.FTFundId == fundId && d.IsDeleted != true)
                .OrderByDescending(d => d.CreatedOn)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFundDonation>> GetDonationsByMemberIdAsync(Guid memberId)
        {
            return await _context.FTFundDonations
                .Include(d => d.Fund)
                .Include(d => d.Campaign)
                .Where(d => d.FTMemberId == memberId && d.IsDeleted != true)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalDonationsAsync(Guid fundId)
        {
            return await _context.FTFundDonations
                .Where(d => d.FTFundId == fundId && d.IsDeleted != true)
                .SumAsync(d => d.DonationMoney);
        }

        public async Task<decimal> GetCampaignDonationsAsync(Guid campaignId)
        {
            return await _context.FTFundDonations
                .Where(d => d.CampaignId == campaignId && d.IsDeleted != true)
                .SumAsync(d => d.DonationMoney);
        }
    }
}
