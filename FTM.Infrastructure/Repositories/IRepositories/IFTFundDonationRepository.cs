using FTM.Domain.Entities.Funds;
using FTM.Infrastructure.Repositories.Interface;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IFTFundDonationRepository : IGenericRepository<FTFundDonation>
    {
        /// <summary>
        /// Get all donations for a specific fund
        /// </summary>
        Task<IEnumerable<FTFundDonation>> GetDonationsByFundIdAsync(Guid fundId);

        /// <summary>
        /// Get all donations for a specific campaign
        /// </summary>
        Task<IEnumerable<FTFundDonation>> GetDonationsByCampaignIdAsync(Guid campaignId);

        /// <summary>
        /// Get recent contributors for a fund (with limit)
        /// </summary>
        Task<IEnumerable<FTFundDonation>> GetRecentContributorsAsync(Guid fundId, int limit);

        /// <summary>
        /// Get donations by member
        /// </summary>
        Task<IEnumerable<FTFundDonation>> GetDonationsByMemberIdAsync(Guid memberId);

        /// <summary>
        /// Get total donation amount for a fund
        /// </summary>
        Task<decimal> GetTotalDonationsAsync(Guid fundId);

        /// <summary>
        /// Get total donation amount for a campaign
        /// </summary>
        Task<decimal> GetCampaignDonationsAsync(Guid campaignId);
    }
}