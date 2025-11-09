using FTM.Domain.Enums;

namespace FTM.Domain.DTOs.Funds
{
    public class SearchCampaignRequest
    {
        public string? Keywords { get; set; }
        public string? Keyword => Keywords;
        public CampaignStatus? Status { get; set; }
        public DateTimeOffset? StartDateFrom { get; set; }
        public DateTimeOffset? StartDateTo { get; set; }
        public DateTimeOffset? EndDateFrom { get; set; }
        public DateTimeOffset? EndDateTo { get; set; }
        public decimal? MinTargetAmount { get; set; }
        public decimal? MaxTargetAmount { get; set; }
        public string? OrganizerName { get; set; }
        public Guid? FundId { get; set; }
        public Guid? FTFundId => FundId;
        public int Page { get; set; } = 1;
        public int PageNumber => Page;
        public int PageSize { get; set; } = 10;
    }
}
