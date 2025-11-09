using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Infrastructure.Repositories.Interface;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IFTFundCampaignRepository : IGenericRepository<FTFundCampaign>
    {
        /// <summary>
        /// Get campaigns by fund ID
        /// </summary>
        Task<IEnumerable<FTFundCampaign>> GetByFundIdAsync(Guid fundId, int skip = 0, int take = 20);

        /// <summary>
        /// Count campaigns by fund ID
        /// </summary>
        Task<int> CountByFundIdAsync(Guid fundId);

        /// <summary>
        /// Get campaign with details (includes fund, creator, contributions)
        /// </summary>
        Task<FTFundCampaign?> GetWithDetailsAsync(Guid campaignId);

        /// <summary>
        /// Search campaigns
        /// </summary>
        Task<IEnumerable<FTFundCampaign>> SearchCampaignsAsync(SearchCampaignRequest request);

        /// <summary>
        /// Count search results
        /// </summary>
        Task<int> CountSearchResultsAsync(SearchCampaignRequest request);

        /// <summary>
        /// Get active campaigns
        /// </summary>
        Task<IEnumerable<FTFundCampaign>> GetActiveCampaignsAsync(Guid? fundId = null);

        /// <summary>
        /// Update campaign amount raised
        /// </summary>
        Task<bool> UpdateCurrentMoneyAsync(Guid campaignId, decimal amount);

        /// <summary>
        /// Update campaign status
        /// </summary>
        Task<bool> UpdateStatusAsync(Guid campaignId, CampaignStatus status);
    }
}
