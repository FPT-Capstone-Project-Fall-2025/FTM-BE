using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTM.Domain.Entities.Funds
{
    /// <summary>
    /// PayOS payment transaction tracking
    /// </summary>
    public class FTPayOSTransaction : BaseEntity
    {
        /// <summary>
        /// PayOS order code
        /// </summary>
        public string OrderCode { get; set; } = string.Empty;

        /// <summary>
        /// PayOS transaction ID
        /// </summary>
        public string PayOSTransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Related donation ID
        /// </summary>
        public Guid? DonationId { get; set; }

        /// <summary>
        /// Transaction amount
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Transaction description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// PayOS transaction status
        /// </summary>
        public string PayOSStatus { get; set; } = string.Empty;

        /// <summary>
        /// Webhook data from PayOS (JSON)
        /// </summary>
        public string? WebhookData { get; set; }

        /// <summary>
        /// Payment completion timestamp
        /// </summary>
        public DateTimeOffset? CompletedAt { get; set; }

        // Navigation properties
        public virtual FTFundDonation? Donation { get; set; }
    }
}