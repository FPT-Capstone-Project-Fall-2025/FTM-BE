using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTM.Domain.Entities.Funds
{
    /// <summary>
    /// Transaction history for family fund (income/expense tracking)
    /// </summary>
    public class FTFundHistory : BaseEntity
    {
        /// <summary>
        /// Reference to the fund
        /// </summary>
        public Guid FTFundId { get; set; }

        /// <summary>
        /// Type of transaction (Contribute, Expense, Withdrawal)
        /// </summary>
        public MoneyType MoneyType { get; set; }

        /// <summary>
        /// Transaction amount
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal MoneyAmount { get; set; }

        /// <summary>
        /// Description/reason for the transaction
        /// </summary>
        public string FundDescription { get; set; } = string.Empty;

        /// <summary>
        /// Related event or activity (optional)
        /// </summary>
        public string? FundEvent { get; set; }

        /// <summary>
        /// Recipient for withdrawal/expense transactions
        /// </summary>
        public string? Recipient { get; set; }

        /// <summary>
        /// Transaction status (for withdrawals requiring approval)
        /// </summary>
        public TransactionStatus Status { get; set; } = TransactionStatus.Approved;

        /// <summary>
        /// AdminGP who approved/rejected the transaction
        /// </summary>
        public Guid? ApprovedBy { get; set; }

        /// <summary>
        /// Approval/rejection timestamp
        /// </summary>
        public DateTimeOffset? ApprovedOn { get; set; }

        /// <summary>
        /// Feedback or reason for rejection
        /// </summary>
        public string? ApprovalFeedback { get; set; }

        /// <summary>
        /// PayOS transaction ID (for online contributions)
        /// </summary>
        public string? PaymentTransactionId { get; set; }

        /// <summary>
        /// Reference to campaign (if this contribution is for a specific campaign)
        /// </summary>
        public Guid? CampaignId { get; set; }

        // Navigation properties
        public virtual FTFund Fund { get; set; } = null!;
        public virtual FTMember? Approver { get; set; }
        public virtual FTFundCampaign? Campaign { get; set; }
    }
}
