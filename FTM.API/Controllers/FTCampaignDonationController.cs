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
        private readonly IFTCampaignService _campaignService;
        private readonly IBlobStorageService _blobStorageService;

        public FTCampaignDonationController(
            IFTCampaignDonationService donationService,
            IPayOSPaymentService payOSService,
            IFTCampaignService campaignService,
            IBlobStorageService blobStorageService)
        {
            _donationService = donationService;
            _payOSService = payOSService;
            _campaignService = campaignService;
            _blobStorageService = blobStorageService;
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
        /// Get my pending donations (donations waiting for proof upload or confirmation)
        /// Used by FE to show user their own pending donations that need proof images
        /// </summary>
        [HttpGet("my-pending")]
 
        public async Task<IActionResult> GetMyPendingDonations([FromQuery] Guid? userId = null)
        {
            try
            {
                if (!userId.HasValue)
                    return BadRequest(new ApiError("User ID is required"));

                var result = await _donationService.GetUserPendingDonationsAsync(userId.Value);
                return Ok(new ApiSuccess("Your pending donations retrieved successfully", result));
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

        /// <summary>
        /// Get pending donations for a specific campaign (waiting for manager confirmation)
        /// </summary>
        [HttpGet("campaign/{campaignId:guid}/pending")]
        public async Task<IActionResult> GetPendingDonationsByCampaign(Guid campaignId)
        {
            try
            {
                var result = await _donationService.GetPendingDonationsByCampaignAsync(campaignId);
                return Ok(new ApiSuccess("Pending donations retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get all pending donations across all campaigns (for admin/manager)
        /// Optional: filter by familyTreeId
        /// </summary>
        [HttpGet("pending")]
        public async Task<IActionResult> GetAllPendingDonations([FromQuery] Guid? familyTreeId = null)
        {
            try
            {
                var result = await _donationService.GetAllPendingDonationsAsync(familyTreeId);
                return Ok(new ApiSuccess("Pending donations retrieved successfully", result));
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
                    Status = DonationStatus.Pending,
                    ProofImages = request.ProofImages // Store proof images if provided
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

        #region Donation Confirmation

        /// <summary>
        /// Upload proof images to blob storage for a specific donation
        /// Images are automatically linked to the donation upon upload
        /// </summary>
        [HttpPost("{donationId:guid}/upload-proof")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProofImages(Guid donationId, [FromForm] List<IFormFile> images)
        {
            try
            {
                // Validate donation exists
                var donation = await _donationService.GetByIdAsync(donationId);
                if (donation == null)
                    return NotFound(new ApiError("Donation not found"));

                // Validate donation status
                if (donation.Status == DonationStatus.Completed)
                    return BadRequest(new ApiError("Cannot upload proof for already confirmed donation"));

                if (donation.Status == DonationStatus.Rejected)
                    return BadRequest(new ApiError("Cannot upload proof for rejected donation"));

                if (images == null || !images.Any())
                    return BadRequest(new ApiError("No images provided"));

                if (images.Count > 5)
                    return BadRequest(new ApiError("Maximum 5 images allowed"));

                var uploadedUrls = new List<string>();
                foreach (var image in images)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(extension))
                        return BadRequest(new ApiError($"Invalid file type: {extension}. Allowed: {string.Join(", ", allowedExtensions)}"));

                    // Validate file size (max 5MB)
                    if (image.Length > 5 * 1024 * 1024)
                        return BadRequest(new ApiError($"File {image.FileName} exceeds 5MB limit"));

                    // Upload to blob storage
                    var fileName = $"donation-proof_{donationId}_{Guid.NewGuid()}{extension}";
                    var blobUrl = await _blobStorageService.UploadFileAsync(image, "donation-proofs", fileName);
                    uploadedUrls.Add(blobUrl);
                }

                // Automatically update donation with proof images
                var existingProofs = string.IsNullOrWhiteSpace(donation.ProofImages) 
                    ? new List<string>() 
                    : donation.ProofImages.Split(',').ToList();
                
                existingProofs.AddRange(uploadedUrls);
                donation.ProofImages = string.Join(",", existingProofs);
                
                await _donationService.UpdateAsync(donation);

                return Ok(new ApiSuccess("Images uploaded and linked to donation successfully", new
                {
                    DonationId = donationId,
                    ImageUrls = uploadedUrls,
                    AllProofImages = existingProofs,
                    CommaSeparated = donation.ProofImages,
                    Count = uploadedUrls.Count,
                    TotalProofs = existingProofs.Count
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Error uploading images", ex.Message));
            }
        }

        /// <summary>
        /// Confirm cash/bank transfer donation (proof images should be uploaded via upload-proof endpoint first)
        /// </summary>
        [HttpPost("{donationId:guid}/confirm")]

        public async Task<IActionResult> ConfirmDonation(Guid donationId, [FromBody] ConfirmDonationDto request)
        {
            try
            {
                if (donationId != request.DonationId)
                    return BadRequest(new ApiError("Donation ID mismatch"));

                // Get donation
                var donation = await _donationService.GetByIdAsync(donationId);
                if (donation == null)
                    return NotFound(new ApiError("Donation not found"));

                // Validate current status
                if (donation.Status == DonationStatus.Completed)
                    return BadRequest(new ApiError("Donation already confirmed"));

                if (donation.Status == DonationStatus.Rejected)
                    return BadRequest(new ApiError("Cannot confirm rejected donation"));

                // Validate proof images
                if (string.IsNullOrWhiteSpace(donation.ProofImages))
                    return BadRequest(new ApiError("Proof images are required. Please upload proof images first using the upload-proof endpoint."));

                // Update donation properties
                donation.Status = DonationStatus.Completed;
                donation.ConfirmedBy = request.ConfirmedBy;
                donation.ConfirmedOn = DateTimeOffset.UtcNow;
                donation.ConfirmationNotes = request.Notes;

                // Save changes
                var updatedDonation = await _donationService.UpdateAsync(donation);

                // Update campaign balance
                var campaign = await _donationService.GetCampaignForDonationAsync(donation.CampaignId);
                if (campaign != null)
                {
                    campaign.CurrentBalance += donation.DonationAmount;
                    await _campaignService.UpdateAsync(campaign);
                }

                return Ok(new ApiSuccess("Donation confirmed successfully", new
                {
                    DonationId = updatedDonation.Id,
                    Status = updatedDonation.Status.ToString(),
                    Amount = updatedDonation.DonationAmount,
                    ProofImages = updatedDonation.ProofImages?.Split(','),
                    ConfirmedBy = updatedDonation.ConfirmedBy,
                    ConfirmedOn = updatedDonation.ConfirmedOn,
                    NewCampaignBalance = campaign?.CurrentBalance
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Error confirming donation", ex.Message));
            }
        }

        /// <summary>
        /// Reject a donation (for manager to reject pending donations with invalid proof or other reasons)
        /// </summary>
        [HttpPost("{donationId:guid}/reject")]
        public async Task<IActionResult> RejectDonation(Guid donationId, [FromBody] RejectCampaignDonationRequest request)
        {
            try
            {
                if (donationId != request.DonationId)
                    return BadRequest(new ApiError("Donation ID mismatch"));

                // Get donation
                var donation = await _donationService.GetByIdAsync(donationId);
                if (donation == null)
                    return NotFound(new ApiError("Donation not found"));

                // Validate current status
                if (donation.Status != DonationStatus.Pending)
                    return BadRequest(new ApiError("Can only reject pending donations"));

                // Update donation status to Rejected
                donation.Status = DonationStatus.Rejected;
                donation.ConfirmedBy = request.RejectedBy;
                donation.ConfirmedOn = DateTimeOffset.UtcNow;
                donation.ConfirmationNotes = request.Reason;

                // Save changes
                var updatedDonation = await _donationService.UpdateAsync(donation);

                return Ok(new ApiSuccess("Donation rejected successfully", new
                {
                    DonationId = updatedDonation.Id,
                    Status = updatedDonation.Status.ToString(),
                    RejectedBy = updatedDonation.ConfirmedBy,
                    RejectedOn = updatedDonation.ConfirmedOn,
                    Reason = updatedDonation.ConfirmationNotes,
                    Amount = updatedDonation.DonationAmount
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError("Error rejecting donation", ex.Message));
            }
        }

        #endregion
    }

    #region DTOs

    /// <summary>
    /// Request DTO for rejecting a campaign donation
    /// </summary>
    public class RejectCampaignDonationRequest
    {
        public Guid DonationId { get; set; }
        public Guid? RejectedBy { get; set; } // Made nullable to handle string input
        public string Reason { get; set; } = string.Empty;
    }

    #endregion
}
