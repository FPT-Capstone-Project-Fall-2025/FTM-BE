using System;
using FTM.Domain.Enums;

namespace FTM.Domain.DTOs.Funds
{
    public class CreateFundExpenseRequest
    {
        public Guid FTFundId { get; set; }
        public Guid? CampaignId { get; set; } // null nếu chi từ quỹ chung
        public decimal ExpenseAmount { get; set; }
        public string ExpenseDescription { get; set; } = string.Empty;
        public string? ExpenseEvent { get; set; }
        public string? Recipient { get; set; }
        public DateTimeOffset? PlannedDate { get; set; }
    }

    public class ApproveFundExpenseRequest
    {
        public Guid ExpenseId { get; set; }
        public TransactionStatus Status { get; set; } // Approved hoặc Rejected
        public string? ApprovalFeedback { get; set; }
    }

    public class FundExpenseResponseDto
    {
        public Guid Id { get; set; }
        public Guid FTFundId { get; set; }
        public string FundName { get; set; } = string.Empty;
        public Guid? CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public decimal ExpenseAmount { get; set; }
        public string ExpenseDescription { get; set; } = string.Empty;
        public string? ExpenseEvent { get; set; }
        public string? Recipient { get; set; }
        public TransactionStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public Guid? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTimeOffset? ApprovedOn { get; set; }
        public string? ApprovalFeedback { get; set; }
        public DateTimeOffset? PlannedDate { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class FundOverviewDto
    {
        public Guid FundId { get; set; }
        public Guid FamilyTreeId { get; set; }
        public string FamilyTreeName { get; set; } = string.Empty;
        public string FundName { get; set; } = string.Empty;
        public string? FundNote { get; set; }
        
        // Statistics
        public int TotalCampaigns { get; set; }
        public int ActiveCampaigns { get; set; }
        public int TotalDonations { get; set; }
        public int PendingConfirmations { get; set; }
        public int TotalExpenses { get; set; }
        public int PendingApprovals { get; set; }
        public decimal TotalDonationAmount { get; set; }
        public decimal TotalExpenseAmount { get; set; }
        
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
    }
}