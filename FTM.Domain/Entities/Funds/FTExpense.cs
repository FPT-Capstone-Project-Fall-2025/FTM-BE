using FTM.Domain.Entities.FamilyTree;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTM.Domain.Entities.Funds
{
    /// <summary>
    /// Fund expenses - Chi tiêu từ quỹ gia phả
    /// </summary>
    public class FTExpense : BaseEntity
    {
        /// <summary>
        /// Reference to the fund
        /// </summary>
        public Guid FTFundId { get; set; }

        /// <summary>
        /// Reference to FTMember who requested the expense
        /// </summary>
        public Guid FTMemberId { get; set; }

        /// <summary>
        /// Expense amount
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal MoneyExpense { get; set; }

        /// <summary>
        /// Expense description/reason
        /// </summary>
        public string ExpenseDescription { get; set; } = string.Empty;

        /// <summary>
        /// Related campaign (if expense is for a specific campaign)
        /// </summary>
        public Guid? CampaignId { get; set; }

        // Navigation properties
        public virtual FTFund Fund { get; set; } = null!;
        public virtual FTMember FTMember { get; set; } = null!;
        public virtual FTFundCampaign? Campaign { get; set; }
    }
}