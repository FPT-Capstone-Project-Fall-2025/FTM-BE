using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTM.Domain.Entities.Funds
{
    /// <summary>
    /// Expenses from campaign funds (independent from main family fund)
    /// </summary>
    public class FTCampaignExpense : BaseEntity
    {
        /// <summary>
        /// Reference to the campaign this expense belongs to
        /// </summary>
        public Guid CampaignId { get; set; }

        /// <summary>
        /// Member who authorized this expense (usually campaign manager)
        /// </summary>
        public Guid AuthorizedBy { get; set; }

        /// <summary>
        /// Expense title/name
        /// </summary>
        public string ExpenseTitle { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the expense
        /// </summary>
        public string ExpenseDescription { get; set; } = string.Empty;

        /// <summary>
        /// Expense amount
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal ExpenseAmount { get; set; }

        /// <summary>
        /// Expense category
        /// </summary>
        public ExpenseCategory Category { get; set; }

        /// <summary>
        /// Date when the expense occurred
        /// </summary>
        public DateTimeOffset ExpenseDate { get; set; }

        /// <summary>
        /// Recipient of the expense (person/organization who received money)
        /// </summary>
        public string? Recipient { get; set; }

        /// <summary>
        /// Payment method used for this expense
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// Receipt/proof image URLs (JSON array)
        /// </summary>
        public string? ReceiptImages { get; set; }

        /// <summary>
        /// Additional notes about the expense
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Approval status
        /// </summary>
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

        /// <summary>
        /// Who approved this expense (if different from authorized by)
        /// </summary>
        public Guid? ApprovedBy { get; set; }

        /// <summary>
        /// When the expense was approved
        /// </summary>
        public DateTimeOffset? ApprovedOn { get; set; }

        /// <summary>
        /// Approval notes
        /// </summary>
        public string? ApprovalNotes { get; set; }

        // Navigation properties
        public virtual FTFundCampaign Campaign { get; set; } = null!;
        public virtual FTMember Authorizer { get; set; } = null!;
        public virtual FTMember? Approver { get; set; }
    }
}