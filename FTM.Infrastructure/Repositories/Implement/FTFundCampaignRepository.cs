using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FTM.Infrastructure.Repositories
{
    public class FTFundCampaignRepository : GenericRepository<FTFundCampaign>, IFTFundCampaignRepository
    {
        public FTFundCampaignRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver)
            : base(context, currentUserResolver)
        {
        }

        public async Task<IEnumerable<FTFundCampaign>> GetByFundIdAsync(Guid fundId, int skip = 0, int take = 20)
        {
            return await Context.FTFundCampaigns
                .Where(c => c.FTId == fundId && c.IsDeleted == false)
                .OrderByDescending(c => c.CreatedOn)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountByFundIdAsync(Guid fundId)
        {
            return await Context.FTFundCampaigns
                .CountAsync(c => c.FTId == fundId && c.IsDeleted == false);
        }

        public async Task<FTFundCampaign?> GetWithDetailsAsync(Guid campaignId)
        {
            return await Context.FTFundCampaigns
                .Include(c => c.FamilyTree)
                .Include(c => c.Donations.Where(d => d.IsDeleted != true))
                    .ThenInclude(d => d.Member)
                .FirstOrDefaultAsync(c => c.Id == campaignId && c.IsDeleted == false);
        }

        public async Task<IEnumerable<FTFundCampaign>> SearchCampaignsAsync(SearchCampaignRequest request)
        {
            var query = Context.FTFundCampaigns
                .Where(c => c.IsDeleted == false);

            // Filter by fund ID (family tree)
            if (request.FTFundId.HasValue)
            {
                query = query.Where(c => c.FTId == request.FTFundId.Value);
            }

            // Filter by keyword
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                query = query.Where(c => 
                    c.CampaignName.ToLower().Contains(keyword) ||
                    c.CampaignDescription.ToLower().Contains(keyword));
            }

            // Filter by status
            if (request.Status.HasValue)
            {
                query = query.Where(c => c.Status == request.Status.Value);
            }

            // Filter by start date range
            if (request.StartDateFrom.HasValue)
            {
                query = query.Where(c => c.StartDate >= request.StartDateFrom.Value);
            }
            if (request.StartDateTo.HasValue)
            {
                query = query.Where(c => c.StartDate <= request.StartDateTo.Value);
            }

            // Filter by target amount range
            if (request.MinTargetAmount.HasValue)
            {
                query = query.Where(c => c.FundGoal >= request.MinTargetAmount.Value);
            }
            if (request.MinTargetAmount.HasValue)
            {
                query = query.Where(c => c.FundGoal <= request.MinTargetAmount.Value);
            }

            return await query
                .OrderByDescending(c => c.CreatedOn)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
        }

        public async Task<int> CountSearchResultsAsync(SearchCampaignRequest request)
        {
            var query = Context.FTFundCampaigns
                .Where(c => c.IsDeleted == false);

            if (request.FTFundId.HasValue)
            {
                query = query.Where(c => c.FTId == request.FTFundId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                query = query.Where(c => 
                    c.CampaignName.ToLower().Contains(keyword) ||
                    c.CampaignDescription.ToLower().Contains(keyword));
            }

            if (request.Status.HasValue)
            {
                query = query.Where(c => c.Status == request.Status.Value);
            }

            if (request.StartDateFrom.HasValue)
            {
                query = query.Where(c => c.StartDate >= request.StartDateFrom.Value);
            }
            if (request.StartDateTo.HasValue)
            {
                query = query.Where(c => c.StartDate <= request.StartDateTo.Value);
            }

            if (request.MinTargetAmount.HasValue)
            {
                query = query.Where(c => c.FundGoal >= request.MinTargetAmount.Value);
            }
            if (request.MinTargetAmount.HasValue)
            {
                query = query.Where(c => c.FundGoal <= request.MinTargetAmount.Value);
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<FTFundCampaign>> GetActiveCampaignsAsync(Guid? fundId = null)
        {
            var query = Context.FTFundCampaigns
                .Where(c => c.IsDeleted == false);

            if (fundId.HasValue)
            {
                query = query.Where(c => c.FTId == fundId.Value);
            }

            return await query
                .OrderBy(c => c.EndDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateCurrentMoneyAsync(Guid campaignId, decimal amount)
        {
            var campaign = await Context.FTFundCampaigns.FindAsync(campaignId);
            if (campaign == null || campaign.IsDeleted == true)
                return false;

            campaign.CurrentBalance += amount;
            campaign.LastModifiedOn = DateTimeOffset.Now;

            Context.FTFundCampaigns.Update(campaign);
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStatusAsync(Guid campaignId, CampaignStatus status)
        {
            var campaign = await Context.FTFundCampaigns.FindAsync(campaignId);
            if (campaign == null || campaign.IsDeleted == true)
                return false;

            campaign.Status = status;
            campaign.LastModifiedOn = DateTimeOffset.Now;

            Context.FTFundCampaigns.Update(campaign);
            return await Context.SaveChangesAsync() > 0;
        }
    }
}





