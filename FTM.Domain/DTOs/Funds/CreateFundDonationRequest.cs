using System;
using FTM.Domain.Enums;

namespace FTM.Domain.DTOs.Funds
{
    public class CreateFundDonationRequest
    {
        public Guid FTFundId { get; set; }
        public Guid? FTMemberId { get; set; } // null nếu là donor ngoài gia đình
        public Guid? CampaignId { get; set; } // null nếu đóng góp cho quỹ chung
        public decimal DonationMoney { get; set; }
        public string? DonorName { get; set; } // Tên người đóng góp (nếu không phải member)
        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentNotes { get; set; }
        public string? PaymentTransactionId { get; set; } // Cho PayOS integration
    }

    public class ConfirmDonationRequest
    {
        public Guid DonationId { get; set; }
        public DonationStatus Status { get; set; } // Confirmed hoặc Rejected
        public string? ConfirmationNotes { get; set; }
    }

    public class FundDonationResponseDto
    {
        public Guid Id { get; set; }
        public Guid FTFundId { get; set; }
        public string FundName { get; set; } = string.Empty;
        public Guid? FTMemberId { get; set; }
        public string DonorName { get; set; } = string.Empty;
        public Guid? CampaignId { get; set; }
        public string? CampaignName { get; set; }
        public decimal DonationMoney { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string PaymentMethodName { get; set; } = string.Empty;
        public string? PaymentNotes { get; set; }
        public string? PaymentTransactionId { get; set; }
        public DonationStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public Guid? ConfirmedBy { get; set; }
        public string? ConfirmedByName { get; set; }
        public DateTimeOffset? ConfirmedOn { get; set; }
        public string? ConfirmationNotes { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}