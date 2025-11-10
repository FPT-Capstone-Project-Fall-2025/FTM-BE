using FTM.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FTM.Domain.DTOs.Funds
{
    /// <summary>
    /// DTO for creating a new campaign
    /// </summary>
    public class CreateFTCampaignDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        public Guid FamilyTreeId { get; set; }

        [Required]
        public Guid CampaignManagerId { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Target amount must be greater than 0")]
        public decimal TargetAmount { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(500)]
        public string? Purpose { get; set; }

        [StringLength(500)]
        public string? BeneficiaryInfo { get; set; }
    }

    /// <summary>
    /// DTO for updating campaign details
    /// </summary>
    public class UpdateFTCampaignDto
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Target amount must be greater than 0")]
        public decimal? TargetAmount { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(500)]
        public string? Purpose { get; set; }

        [StringLength(500)]
        public string? BeneficiaryInfo { get; set; }

        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO for campaign response
    /// </summary>
    public class FTCampaignResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid FamilyTreeId { get; set; }
        public string? FamilyTreeName { get; set; }
        public Guid CampaignManagerId { get; set; }
        public string? CampaignManagerName { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal ProgressPercentage => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Purpose { get; set; }
        public string? BeneficiaryInfo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalDonations { get; set; }
        public int TotalDonors { get; set; }
    }

    /// <summary>
    /// DTO for creating a campaign donation
    /// </summary>
    public class CreateCampaignDonationDto
    {
        [Required]
        public Guid CampaignId { get; set; }

        [Required]
        [Range(1000, double.MaxValue, ErrorMessage = "Minimum donation amount is 1,000 VND")]
        public decimal Amount { get; set; }

        [StringLength(500)]
        public string? Message { get; set; }

        public bool IsAnonymous { get; set; } = false;

        // Donor information for payment
        [Required]
        [StringLength(100)]
        public string DonorName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string DonorEmail { get; set; }

        [StringLength(20)]
        public string? DonorPhone { get; set; }

        [StringLength(200)]
        public string? DonorAddress { get; set; }

        // Payment URLs
        [Required]
        [Url]
        public string ReturnUrl { get; set; }

        [Required]
        [Url]
        public string CancelUrl { get; set; }
    }

    /// <summary>
    /// DTO for campaign donation response
    /// </summary>
    public class FTCampaignDonationResponseDto
    {
        public Guid Id { get; set; }
        public Guid CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public Guid? DonorId { get; set; }
        public string? DonorName { get; set; }
        public decimal Amount { get; set; }
        public string? Message { get; set; }
        public bool IsAnonymous { get; set; }
        public DonationStatus Status { get; set; }
        public string PayOSOrderCode { get; set; }
        public string? TransactionId { get; set; }
        public string? ProofImages { get; set; } // Proof of payment (transfer receipt, cash photo)
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Response for online donation with payment information
    /// </summary>
    public class OnlineDonationResponseDto
    {
        public Guid DonationId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public PaymentInfoDto PaymentInfo { get; set; } = new();
    }

    /// <summary>
    /// Payment information for QR code
    /// </summary>
    public class PaymentInfoDto
    {
        public string BankCode { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountHolderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string QRCodeUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// Campaign with manager bank account info
    /// </summary>
    public class CampaignWithBankAccountDto
    {
        public Guid CampaignId { get; set; }
        public string CampaignName { get; set; } = string.Empty;
        public Guid CampaignManagerId { get; set; }
        public ManagerBankAccountDto? ManagerBankAccount { get; set; }
    }

    /// <summary>
    /// Manager's bank account information
    /// </summary>
    public class ManagerBankAccountDto
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string? BankCode { get; set; }
        public string AccountHolderName { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for creating a campaign expense
    /// </summary>
    public class CreateCampaignExpenseDto
    {
        [Required]
        public Guid CampaignId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public ExpenseCategory Category { get; set; }

        public DateTime? ExpenseDate { get; set; }

        [StringLength(500)]
        public string? ReceiptUrl { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for updating campaign expense
    /// </summary>
    public class UpdateCampaignExpenseDto
    {
        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal? Amount { get; set; }

        public ExpenseCategory? Category { get; set; }

        public DateTime? ExpenseDate { get; set; }

        [StringLength(500)]
        public string? ReceiptUrl { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for campaign expense response
    /// </summary>
    public class FTCampaignExpenseResponseDto
    {
        public Guid Id { get; set; }
        public Guid CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public ExpenseCategory Category { get; set; }
        public string CategoryName => Category.ToString();
        public DateTime? ExpenseDate { get; set; }
        public string? ReceiptUrl { get; set; }
        public string? Notes { get; set; }
        public ApprovalStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public Guid RequestedById { get; set; }
        public string? RequestedByName { get; set; }
        public Guid? ApprovedById { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovalNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for approving an expense
    /// </summary>
    public class ApproveExpenseDto
    {
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for rejecting an expense
    /// </summary>
    public class RejectExpenseDto
    {
        [Required]
        [StringLength(500)]
        public string Reason { get; set; }
    }

    /// <summary>
    /// DTO for campaign financial summary
    /// </summary>
    public class CampaignFinancialSummaryDto
    {
        public Guid CampaignId { get; set; }
        public string CampaignName { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal TotalDonations { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal ApprovedExpenses { get; set; }
        public decimal PendingExpenses { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal ProgressPercentage => TargetAmount > 0 ? (TotalDonations / TargetAmount) * 100 : 0;
        public int TotalDonors { get; set; }
        public int TotalExpenseRequests { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public DateTime? LastExpenseDate { get; set; }
    }

    /// <summary>
    /// DTO for expense statistics
    /// </summary>
    public class ExpenseStatisticsDto
    {
        public Guid CampaignId { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal ApprovedExpenses { get; set; }
        public decimal PendingExpenses { get; set; }
        public decimal RejectedExpenses { get; set; }
        public int TotalExpenseRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int PendingRequests { get; set; }
        public int RejectedRequests { get; set; }
        public Dictionary<ExpenseCategory, decimal> ExpensesByCategory { get; set; } = new();
    }

    /// <summary>
    /// DTO for top donors
    /// </summary>
    public class TopDonorDto
    {
        public Guid? DonorId { get; set; }
        public string DonorName { get; set; }
        public decimal TotalDonated { get; set; }
        public int DonationCount { get; set; }
        public DateTime LastDonationDate { get; set; }
        public bool IsAnonymous { get; set; }
    }

    /// <summary>
    /// DTO for payment response
    /// </summary>
    public class PaymentResponseDto
    {
        public Guid DonationId { get; set; }
        public string PaymentUrl { get; set; }
        public string OrderCode { get; set; }
    }

    /// <summary>
    /// DTO for payment status
    /// </summary>
    public class PaymentStatusDto
    {
        public string OrderCode { get; set; }
        public string Status { get; set; }
        public DateTime? TransactionDateTime { get; set; }
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// DTO for confirming cash/bank transfer donation
    /// Proof images should be uploaded via the upload-proof endpoint before confirmation
    /// </summary>
    public class ConfirmDonationDto
    {
        [Required]
        public Guid DonationId { get; set; }

        [Required]
        public Guid ConfirmedBy { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for creating donation (unified - supports Cash and BankTransfer)
    /// </summary>
    public class CampaignDonateRequest
    {
        public Guid? MemberId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string DonorName { get; set; } = string.Empty;
        
        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
        
        [StringLength(500)]
        public string? PaymentNotes { get; set; }
        
        public bool? IsAnonymous { get; set; }
        
        /// <summary>
        /// Comma-separated blob URLs of proof images (for Cash donations)
        /// </summary>
        [StringLength(2000)]
        public string? ProofImages { get; set; }
    }

    /// <summary>
    /// DTO for online donations (bank transfer with QR code)
    /// </summary>
    public class CreateOnlineDonationDto
    {
        [Required]
        public Guid CampaignId { get; set; }
        
        public Guid? MemberId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string DonorName { get; set; } = string.Empty;
        
        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [StringLength(500)]
        public string? Message { get; set; }
        
        public bool IsAnonymous { get; set; }
        
        [Required]
        [Url]
        public string ReturnUrl { get; set; } = string.Empty;
        
        [Required]
        [Url]
        public string CancelUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for cash donations (requires proof images)
    /// </summary>
    public class CreateCashDonationDto
    {
        [Required]
        public Guid CampaignId { get; set; }
        
        public Guid? MemberId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string DonorName { get; set; } = string.Empty;
        
        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public bool IsAnonymous { get; set; }
        
        /// <summary>
        /// Comma-separated blob URLs of proof images (cash photos, receipts)
        /// </summary>
        [StringLength(2000)]
        public string? ProofImages { get; set; }
    }

    /// <summary>
    /// DTO for payment callback from payment gateway
    /// </summary>
    public class PaymentCallbackDto
    {
        [Required]
        public string OrderCode { get; set; } = string.Empty;
        
        [Required]
        public string Status { get; set; } = string.Empty;
        
        [Required]
        public decimal Amount { get; set; }
        
        public string? TransactionId { get; set; }
        
        public DateTime? TransactionDateTime { get; set; }
    }
}
