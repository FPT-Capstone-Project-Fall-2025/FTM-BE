using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTM.Domain.Entities.Funds
{
    /// <summary>
    /// Independent fundraising campaigns (Chiến dịch gây quỹ độc lập)
    /// Each campaign is managed independently with its own fund balance
    /// Examples: Education Support Campaign (Chiến dịch khuyến học), etc.
    /// </summary>
    public class FTFundCampaign : BaseEntity
    {
        /// <summary>
        /// Reference to the family tree this campaign belongs to
        /// </summary>
        public Guid FTId { get; set; }

        /// <summary>
        /// Campaign name/title (e.g., "Chiến dịch khuyến học 2025")
        /// </summary>
        public string CampaignName { get; set; } = string.Empty;

        /// <summary>
        /// Campaign purpose/description
        /// </summary>
        public string CampaignDescription { get; set; } = string.Empty;

        /// <summary>
        /// Campaign manager (FTMember who manages this campaign's fund)
        /// This person will receive and manage all donations for this campaign
        /// </summary>
        public Guid CampaignManagerId { get; set; }

        /// <summary>
        /// Campaign start date
        /// </summary>
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// Campaign end date
        /// </summary>
        public DateTimeOffset EndDate { get; set; }

        /// <summary>
        /// Target amount to raise
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal FundGoal { get; set; }

        /// <summary>
        /// Current amount raised for this campaign
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentBalance { get; set; } = 0;

        /// <summary>
        /// Campaign status
        /// </summary>
        public CampaignStatus Status { get; set; } = CampaignStatus.Upcoming;

        /// <summary>
        /// Campaign visibility (public or private within family)
        /// </summary>
        public bool IsPublic { get; set; } = false;

        /// <summary>
        /// Campaign image/banner URL
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Additional notes or instructions
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Bank account number for receiving donations (from campaign manager)
        /// </summary>
        public string? BankAccountNumber { get; set; }

        /// <summary>
        /// Bank name (e.g., "Vietcombank", "Techcombank")
        /// </summary>
        public string? BankName { get; set; }

        /// <summary>
        /// Bank code for VietQR (e.g., "970436" for Vietcombank)
        /// </summary>
        public string? BankCode { get; set; }

        /// <summary>
        /// Account holder name (must match bank account)
        /// </summary>
        public string? AccountHolderName { get; set; }

        // Navigation properties
        public virtual FamilyTree.FamilyTree FamilyTree { get; set; } = null!;
        public virtual FTMember CampaignManager { get; set; } = null!;
        
        /// <summary>
        /// Donations specifically for this campaign
        /// </summary>
        public virtual ICollection<FTCampaignDonation> Donations { get; set; } = new List<FTCampaignDonation>();
        
        /// <summary>
        /// Expenses from this campaign's fund
        /// </summary>
        public virtual ICollection<FTCampaignExpense> Expenses { get; set; } = new List<FTCampaignExpense>();
    }
}
