using FTM.Domain.Entities.FamilyTree;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTM.Domain.Entities.Funds
{
    /// <summary>
    /// Fund donations - Đóng góp vào quỹ gia phả
    /// </summary>
    public class FTDonation : BaseEntity
    {
        /// <summary>
        /// Reference to campaign
        /// </summary>
        public Guid CampaignId { get; set; }

        /// <summary>
        /// Reference to FTMember who made the donation
        /// </summary>
        public Guid FTMemberId { get; set; }

        /// <summary>
        /// Donation amount
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonatedMoney { get; set; }

        // Navigation properties
        public virtual FTFundCampaign Campaign { get; set; } = null!;
        public virtual FTMember FTMember { get; set; } = null!;
    }
}