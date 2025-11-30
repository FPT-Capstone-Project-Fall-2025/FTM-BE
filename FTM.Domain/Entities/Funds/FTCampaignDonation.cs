using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTM.Domain.Entities.Funds
{
    /// <summary>
    /// Donations specifically for campaigns (independent from main family fund)
    /// </summary>
    public class FTCampaignDonation : BaseEntity
    {
        /// <summary>
        /// Reference to the campaign this donation is for
        /// </summary>
        public Guid CampaignId { get; set; }

        /// <summary>
        /// Member who made the donation (optional - can be anonymous)
        /// </summary>
        public Guid? FTMemberId { get; set; }

        /// <summary>
        /// Donor name (used when FTMemberId is null or for display)
        /// </summary>
        public string? DonorName { get; set; }

        /// <summary>
        /// Amount donated
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonationAmount { get; set; }

        /// <summary>
        /// Payment method used
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Additional notes from donor
        /// </summary>
        public string? DonorNotes { get; set; }

        /// <summary>
        /// Payment transaction ID (from PayOS or other payment systems)
        /// </summary>
        public string? PaymentTransactionId { get; set; }

        /// <summary>
        /// PayOS order code for tracking
        /// </summary>
        public long? PayOSOrderCode { get; set; }

        /// <summary>
        /// Donation status
        /// </summary>
        public DonationStatus Status { get; set; } = DonationStatus.Pending;

        /// <summary>
        /// Who confirmed this donation (campaign manager or admin)
        /// </summary>
        public Guid? ConfirmedBy { get; set; }

        /// <summary>
        /// When the donation was confirmed
        /// </summary>
        public DateTimeOffset? ConfirmedOn { get; set; }

        /// <summary>
        /// Confirmation notes
        /// </summary>
        public string? ConfirmationNotes { get; set; }

        /// <summary>
        /// Proof images (transfer receipt, cash photo, etc.) - comma-separated URLs
        /// Required when confirming cash donations or for transfer proof
        /// </summary>
        public string? ProofImages { get; set; }

        /// <summary>
        /// Is this donation anonymous?
        /// </summary>
        public bool IsAnonymous { get; set; } = false;

        // Navigation properties
        public virtual FTFundCampaign Campaign { get; set; } = null!;
        public virtual FTMember? Member { get; set; }
        public virtual FTMember? Confirmer { get; set; }
    }
}