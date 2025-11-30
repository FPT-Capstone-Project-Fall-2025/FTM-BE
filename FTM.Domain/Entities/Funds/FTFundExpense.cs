using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTM.Domain.Entities.Funds
{
    /// <summary>
    /// Represents expenses/withdrawals from family fund
    /// Tracks spending transactions with approval workflow
    /// </summary>
    public class FTFundExpense : BaseEntity
    {
        /// <summary>
        /// Reference to the fund
        /// </summary>
        public Guid FTFundId { get; set; }

        /// <summary>
        /// Reference to campaign if expense is related to specific campaign
        /// </summary>
        public Guid? CampaignId { get; set; }

        /// <summary>
        /// Expense amount
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal ExpenseAmount { get; set; }

        /// <summary>
        /// Description/reason for the expense
        /// </summary>
        public string ExpenseDescription { get; set; } = string.Empty;

        /// <summary>
        /// Related event or activity (optional)
        /// </summary>
        public string? ExpenseEvent { get; set; }

        /// <summary>
        /// Recipient of the payment
        /// </summary>
        public string? Recipient { get; set; }

        /// <summary>
        /// Expense status (for approval workflow)
        /// </summary>
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

        /// <summary>
        /// FTMember who approved/rejected the expense
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
        /// Planned date for the expense (user input)
        /// </summary>
        public DateTimeOffset? PlannedDate { get; set; }

        /// <summary>
        /// Receipt/proof image URLs (comma-separated)
        /// Required for expense confirmation and approval
        /// </summary>
        public string? ReceiptImages { get; set; }

        /// <summary>
        /// Payment proof image URL uploaded by manager during approval
        /// Required evidence that payment was actually made
        /// </summary>
        public string? PaymentProofImage { get; set; }

        // Navigation properties
        public virtual FTFund Fund { get; set; } = null!;
        public virtual FTMember? Approver { get; set; }
        public virtual FTFundCampaign? Campaign { get; set; }
    }
}