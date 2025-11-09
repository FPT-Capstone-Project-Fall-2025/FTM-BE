using FTM.Domain.Entities.FamilyTree;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTM.Domain.Entities.Funds
{
    /// <summary>
    /// Main family fund (Quỹ chung dòng tộc)
    /// Each family tree has exactly ONE main fund for collective family activities
    /// This is separate from independent campaigns
    /// </summary>
    public class FTFund : BaseEntity
    {
        /// <summary>
        /// Reference to the family tree this fund belongs to
        /// One-to-One relationship: each family tree has exactly one main fund
        /// </summary>
        public Guid FTId { get; set; }

        /// <summary>
        /// Current balance of the main family fund
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentMoney { get; set; } = 0;

        /// <summary>
        /// Purpose and rules of the main fund
        /// </summary>
        public string? FundNote { get; set; }

        /// <summary>
        /// Fund name/title (e.g., "Quỹ Dòng Tộc Nguyễn")
        /// </summary>
        public string FundName { get; set; } = string.Empty;

        /// <summary>
        /// Fund manager(s) - can be multiple people managing the main fund
        /// JSON array of member IDs
        /// </summary>
        public string? FundManagers { get; set; }

        /// <summary>
        /// Is this fund currently active?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Bank account number for receiving donations
        /// </summary>
        public string? BankAccountNumber { get; set; }

        /// <summary>
        /// Bank code (e.g., "970436" for Vietcombank)
        /// </summary>
        public string? BankCode { get; set; }

        /// <summary>
        /// Bank name (e.g., "Vietcombank", "VietinBank")
        /// </summary>
        public string? BankName { get; set; }

        /// <summary>
        /// Account holder name (fund manager's name or family name)
        /// </summary>
        public string? AccountHolderName { get; set; }

        // Navigation properties
        public virtual FamilyTree.FamilyTree FamilyTree { get; set; } = null!;
        
        /// <summary>
        /// Expenses from the main family fund
        /// </summary>
        public virtual ICollection<FTFundExpense> Expenses { get; set; } = new List<FTFundExpense>();
        
        /// <summary>
        /// Donations to the main family fund
        /// </summary>
        public virtual ICollection<FTFundDonation> Donations { get; set; } = new List<FTFundDonation>();
    }
}
