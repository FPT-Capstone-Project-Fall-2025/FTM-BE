using FTM.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FTM.Domain.DTOs.Funds
{
    /// <summary>
    /// DTO for fund donation creation
    /// </summary>
    public class FundDonateRequest
    {
        public Guid? MemberId { get; set; }

        [StringLength(200)]
        public string? DonorName { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(500)]
        public string? PaymentNotes { get; set; }

        /// <summary>
        /// Comma-separated blob URLs of proof images (for Cash donations)
        /// </summary>
        [StringLength(2000)]
        public string? ProofImages { get; set; }
    }

    /// <summary>
    /// DTO for fund donation response
    /// </summary>
    public class FTFundDonationResponseDto
    {
        public Guid Id { get; set; }
        public Guid FundId { get; set; }
        public string? FundName { get; set; }
        public Guid? DonorId { get; set; }
        public string DonorName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Message { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DonationStatus Status { get; set; }
        public string? PayOSOrderCode { get; set; }
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Comma-separated URLs of proof images
        /// </summary>
        public string? ProofImages { get; set; }
        
        public Guid? ConfirmedBy { get; set; }
        public string? ConfirmerName { get; set; }
        public string? ConfirmationNotes { get; set; }
    }

    /// <summary>
    /// DTO for confirming fund donation
    /// Proof images should be uploaded via the upload-proof endpoint before confirmation
    /// </summary>
    public class ConfirmFundDonationDto
    {
        [Required]
        public Guid DonationId { get; set; }

        [Required]
        public Guid ConfirmedBy { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for fund expense creation
    /// </summary>
    public class CreateFundExpenseDto
    {
        [Required]
        public Guid FundId { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public ExpenseCategory Category { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public Guid RequestedBy { get; set; }

        /// <summary>
        /// Comma-separated blob URLs of receipt images
        /// </summary>
        [StringLength(2000)]
        public string? ReceiptImages { get; set; }
    }

    /// <summary>
    /// DTO for approving fund expense
    /// </summary>
    public class ApproveFundExpenseDto
    {
        [Required]
        public Guid ExpenseId { get; set; }

        [Required]
        public Guid ApprovedBy { get; set; }

        /// <summary>
        /// Required: Receipt images must be provided for approval
        /// </summary>
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string ReceiptImages { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ApprovalNotes { get; set; }
    }

    /// <summary>
    /// DTO for fund expense response
    /// </summary>
    public class FTFundExpenseResponseDto
    {
        public Guid Id { get; set; }
        public Guid FundId { get; set; }
        public string? FundName { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public ExpenseCategory Category { get; set; }
        public string? Notes { get; set; }
        public ExpenseStatus Status { get; set; }
        
        public Guid RequestedBy { get; set; }
        public string? RequestedByName { get; set; }
        public DateTime RequestedDate { get; set; }
        
        public Guid? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovalNotes { get; set; }
        
        /// <summary>
        /// Comma-separated URLs of receipt images
        /// </summary>
        public string? ReceiptImages { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
}
