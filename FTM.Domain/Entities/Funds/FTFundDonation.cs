using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTM.Domain.Entities.Funds
{
    /// <summary>
    /// Represents donations/contributions to family fund
    /// Tracks who contributed how much and when
    /// </summary>
    public class FTFundDonation : BaseEntity
    {
        /// <summary>
        /// Reference to the fund
        /// </summary>
        public Guid FTFundId { get; set; }

        /// <summary>
        /// Reference to the member who contributed (optional - external donors)
        /// </summary>
        public Guid? FTMemberId { get; set; }

        /// <summary>
        /// Reference to campaign if this donation is for specific campaign
        /// </summary>
        public Guid? CampaignId { get; set; }

        /// <summary>
        /// Donation amount
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonationMoney { get; set; }

        /// <summary>
        /// Donor name (if FTMemberId is null, for external donors)
        /// </summary>
        public string? DonorName { get; set; }

        /// <summary>
        /// Payment method used for donation
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        /// <summary>
        /// Additional notes about payment method
        /// </summary>
        public string? PaymentNotes { get; set; }

        /// <summary>
        /// PayOS transaction ID for online payments
        /// </summary>
        public string? PaymentTransactionId { get; set; }

        /// <summary>
        /// PayOS order code for tracking online payments
        /// </summary>
        public long? PayOSOrderCode { get; set; }

        /// <summary>
        /// Donation status (for confirmation workflow)
        /// </summary>
        public DonationStatus Status { get; set; } = DonationStatus.Pending;

        /// <summary>
        /// FTMember who confirmed the donation (for cash donations)
        /// </summary>
        public Guid? ConfirmedBy { get; set; }

        /// <summary>
        /// Confirmation timestamp
        /// </summary>
        public DateTimeOffset? ConfirmedOn { get; set; }

        /// <summary>
        /// Confirmation notes or rejection reason
        /// </summary>
        public string? ConfirmationNotes { get; set; }

        /// <summary>
        /// Proof images (transfer receipt, cash photo, etc.) - comma-separated URLs
        /// Required when confirming cash donations or for transfer proof
        /// </summary>
        public string? ProofImages { get; set; }

        // Navigation properties
        public virtual FTFund Fund { get; set; } = null!;
        public virtual FTMember? Member { get; set; }
        public virtual FTMember? Confirmer { get; set; }
        public virtual FTFundCampaign? Campaign { get; set; }
    }
}