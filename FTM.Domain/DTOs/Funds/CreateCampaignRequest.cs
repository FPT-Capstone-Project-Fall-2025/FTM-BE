using System;
using FTM.Domain.Enums;

namespace FTM.Domain.DTOs.Funds
{
    public class CreateCampaignRequest
    {
        public Guid FamilyTreeId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public string CampaignDescription { get; set; } = string.Empty;
        public string OrganizerName { get; set; } = string.Empty;
        public string? OrganizerContact { get; set; }
        public Guid? CampaignManagerId { get; set; } // Người đại diện quản lý chiến dịch
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public decimal FundGoal { get; set; }
        public string? MediaAttachments { get; set; } // JSON string của array URLs
        
        // Bank account information for online donations
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankCode { get; set; }
        public string? AccountHolderName { get; set; }
    }

    /// <summary>
    /// DTO for updating campaign information
    /// All fields are optional - only provided fields will be updated
    /// </summary>
    public class UpdateCampaignRequest
    {
        public string? CampaignName { get; set; }
        public string? CampaignDescription { get; set; }
        public Guid? CampaignManagerId { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public decimal? FundGoal { get; set; }
        public CampaignStatus? Status { get; set; }
        
        // Bank account information (optional update)
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankCode { get; set; }
        public string? AccountHolderName { get; set; }
        public string? Notes { get; set; }
    }

    public class CampaignResponseDto
    {
        public Guid Id { get; set; }
        public Guid FTFundId { get; set; }
        public string FundName { get; set; } = string.Empty;
        public string CampaignName { get; set; } = string.Empty;
        public string CampaignDescription { get; set; } = string.Empty;
        public string OrganizerName { get; set; } = string.Empty;
        public string? OrganizerContact { get; set; }
        public Guid? CampaignManagerId { get; set; }
        public string? CampaignManagerName { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public decimal FundGoal { get; set; }
        public decimal CurrentMoney { get; set; }
        public decimal ProgressPercentage { get; set; }
        public CampaignStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? MediaAttachments { get; set; }
        public int TotalDonations { get; set; }
        public int TotalExpenses { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
