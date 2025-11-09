using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FTCampaignDonationController : ControllerBase
    {
        private readonly IFTCampaignDonationService _donationService;
        private readonly IPayOSPaymentService _payOSService;

        public FTCampaignDonationController(
            IFTCampaignDonationService donationService,
            IPayOSPaymentService payOSService)
        {
            _donationService = donationService;
            _payOSService = payOSService;
        }

        #region Query Operations

        /// <summary>
        /// Get donation by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDonationById(Guid id)
        {
            try
            {
                var donation = await _donationService.GetByIdAsync(id);
                if (donation == null)
                    return NotFound(new ApiError("Donation not found"));

                return Ok(new ApiSuccess(donation));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get all donations for a campaign (paginated)
        /// </summary>
        [HttpGet("campaign/{campaignId:guid}")]
        public async Task<IActionResult> GetCampaignDonations(
            Guid campaignId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _donationService.GetCampaignDonationsAsync(campaignId, page, pageSize);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get user's donation history (paginated)
        /// </summary>
        [HttpGet("user/{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetUserDonations(
            Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _donationService.GetUserDonationsAsync(userId, page, pageSize);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get top donors for a campaign
        /// </summary>
        [HttpGet("campaign/{campaignId:guid}/top-donors")]
        public async Task<IActionResult> GetTopDonors(
            Guid campaignId,
            [FromQuery] int limit = 10)
        {
            try
            {
                var result = await _donationService.GetTopDonorsAsync(campaignId, limit);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get donation statistics for a campaign
        /// </summary>
        [HttpGet("campaign/{campaignId:guid}/statistics")]
        public async Task<IActionResult> GetDonationStatistics(Guid campaignId)
        {
            try
            {
                var result = await _donationService.GetDonationStatisticsAsync(campaignId);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        #endregion

        #region Donation Creation

        /// <summary>
        /// Create donation (unified endpoint - supports both Cash and BankTransfer)
        /// For Cash: requires manual confirmation
        /// For BankTransfer: generates VietQR code
        /// </summary>
        [HttpPost("campaign/{campaignId:guid}/donate")]
        public async Task<IActionResult> DonateToCampaign(Guid campaignId, [FromBody] CampaignDonateRequest request)
        {
            try
            {
                // Get campaign info first
                var campaign = await _donationService.GetCampaignForDonationAsync(campaignId);
                if (campaign == null)
                    return NotFound(new ApiError("Campaign not found"));

                // Create base donation
                var donation = new FTCampaignDonation
                {
                    CampaignId = campaignId,
                    FTMemberId = request.MemberId,
                    DonorName = request.DonorName,
                    DonationAmount = request.Amount,
                    PaymentMethod = request.PaymentMethod,
                    DonorNotes = request.PaymentNotes,
                    IsAnonymous = request.IsAnonymous ?? false,
                    Status = DonationStatus.Pending
                };

                // For online payments, generate VietQR
                string? qrCodeUrl = null;
                if (request.PaymentMethod == PaymentMethod.BankTransfer)
                {
                    // Validate bank account info
                    if (string.IsNullOrEmpty(campaign.BankAccountNumber) || 
                        string.IsNullOrEmpty(campaign.BankName) ||
                        string.IsNullOrEmpty(campaign.AccountHolderName))
                    {
                        return BadRequest(new ApiError("Campaign has not set up bank account information. Please contact campaign manager."));
                    }

                    // Generate order code and QR
                    donation.PayOSOrderCode = GenerateOrderCode();
                    
                    var paymentInfo = await _payOSService.CreateCampaignDonationPaymentAsync(
                        donation,
                        campaign.CampaignName,
                        campaign.BankCode ?? "970436", // Default to Vietcombank
                        campaign.BankAccountNumber,
                        campaign.AccountHolderName,
                        campaign.BankName);

                    qrCodeUrl = paymentInfo.QRCodeUrl;
                }

                // Save donation to database
                var createdDonation = await _donationService.AddAsync(donation);

                // Build response
                var result = new
                {
                    DonationId = createdDonation.Id,
                    OrderCode = donation.PayOSOrderCode?.ToString(),
                    QrCodeUrl = qrCodeUrl,
                    RequiresManualConfirmation = request.PaymentMethod == PaymentMethod.Cash || qrCodeUrl == null,
                    Message = request.PaymentMethod == PaymentMethod.Cash 
                        ? "Cash donation recorded. Waiting for manager confirmation." 
                        : "Please scan QR code to complete payment. Donation will be confirmed after successful transfer."
                };

                return Ok(new ApiSuccess("Donation created successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Error creating donation", ex.Message));
            }
        }

        /// <summary>
        /// Create online donation (returns QR code for bank transfer)
        /// </summary>
        [HttpPost("online")]
        public async Task<IActionResult> CreateOnlineDonation([FromBody] CreateOnlineDonationDto request)
        {
            try
            {
                // Create donation record
                var donation = new FTCampaignDonation
                {
                    CampaignId = request.CampaignId,
                    FTMemberId = request.MemberId,
                    DonorName = request.DonorName,
                    DonationAmount = request.Amount,
                    PaymentMethod = PaymentMethod.BankTransfer,
                    DonorNotes = request.Message,
                    IsAnonymous = request.IsAnonymous,
                    Status = DonationStatus.Pending,
                    PayOSOrderCode = GenerateOrderCode()
                };

                var createdDonation = await _donationService.AddAsync(donation);

                // Get campaign to check bank account info
                var campaign = await _donationService.GetCampaignForDonationAsync(request.CampaignId);
                if (campaign == null)
                    return BadRequest(new ApiError("Campaign not found"));

                // Validate bank account info
                if (string.IsNullOrEmpty(campaign.BankAccountNumber) || 
                    string.IsNullOrEmpty(campaign.BankName) ||
                    string.IsNullOrEmpty(campaign.AccountHolderName))
                {
                    return BadRequest(new ApiError("Campaign has not set up bank account information. Please contact campaign manager to add banking details before accepting online donations."));
                }

                // Generate QR code for payment
                var paymentInfo = await _payOSService.CreateCampaignDonationPaymentAsync(
                    createdDonation,
                    campaign.CampaignName,
                    campaign.BankCode ?? "970436", // Default to Vietcombank if not set
                    campaign.BankAccountNumber,
                    campaign.AccountHolderName,
                    campaign.BankName);

                var response = new OnlineDonationResponseDto
                {
                    DonationId = createdDonation.Id,
                    OrderCode = createdDonation.PayOSOrderCode.ToString()!,
                    PaymentInfo = paymentInfo
                };

                return Ok(new ApiSuccess("Please scan QR code to complete payment. After transferring, donation will be confirmed by campaign manager.", response));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Create cash donation (requires manual confirmation)
        /// </summary>
        [HttpPost("cash")]
        [Authorize]
        public async Task<IActionResult> CreateCashDonation([FromBody] CreateCashDonationDto request)
        {
            try
            {
                var donation = new FTCampaignDonation
                {
                    CampaignId = request.CampaignId,
                    FTMemberId = request.MemberId,
                    DonorName = request.DonorName,
                    DonationAmount = request.Amount,
                    PaymentMethod = PaymentMethod.Cash,
                    DonorNotes = request.Notes,
                    IsAnonymous = request.IsAnonymous,
                    Status = DonationStatus.Pending
                };

                var result = await _donationService.AddAsync(donation);
                return Ok(new ApiSuccess("Cash donation recorded, pending confirmation", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        #endregion

        #region PayOS Callback

        /// <summary>
        /// PayOS payment callback webhook
        /// </summary>
        [HttpPost("payos-callback")]
        public async Task<IActionResult> PayOSCallback([FromBody] PaymentCallbackDto callback)
        {
            try
            {
                // Verify webhook signature (if implemented in PayOS service)
                // await _payOSService.VerifyWebhookSignature(callback);

                if (callback.Status == "PAID" || callback.Status == "SUCCESS")
                {
                    await _donationService.ProcessCompletedDonationAsync(callback.OrderCode);
                    return Ok(new ApiSuccess("Payment processed successfully"));
                }
                else
                {
                    // Update donation status to failed
                    var donation = await _donationService.GetByOrderCodeAsync(callback.OrderCode);
                    if (donation != null)
                    {
                        donation.Status = DonationStatus.Pending; // Keep as pending or set to failed
                        await _donationService.UpdateAsync(donation);
                    }
                    return Ok(new ApiSuccess("Payment failed"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Check payment status by order code
        /// </summary>
        [HttpGet("payment-status/{orderCode}")]
        public async Task<IActionResult> GetPaymentStatus(string orderCode)
        {
            try
            {
                var donation = await _donationService.GetByOrderCodeAsync(orderCode);
                if (donation == null)
                    return NotFound(new ApiError("Donation not found"));

                return Ok(new ApiSuccess(new
                {
                    OrderCode = orderCode,
                    Status = donation.Status.ToString(),
                    Amount = donation.DonationAmount,
                    CreatedAt = donation.CreatedOn
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        #endregion

        #region Helper Methods

        private long GenerateOrderCode()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        #endregion
    }

    #region DTOs (if not already defined)

    public class CampaignDonateRequest
    {
        public Guid? MemberId { get; set; }
        public string DonorName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentNotes { get; set; }
        public bool? IsAnonymous { get; set; }
    }

    public class CreateOnlineDonationDto
    {
        public Guid CampaignId { get; set; }
        public Guid? MemberId { get; set; }
        public string DonorName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Message { get; set; }
        public bool IsAnonymous { get; set; }
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
    }

    public class CreateCashDonationDto
    {
        public Guid CampaignId { get; set; }
        public Guid? MemberId { get; set; }
        public string DonorName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
        public bool IsAnonymous { get; set; }
    }

    public class PaymentCallbackDto
    {
        public string OrderCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? TransactionId { get; set; }
        public DateTime? TransactionDateTime { get; set; }
    }

    #endregion
}
