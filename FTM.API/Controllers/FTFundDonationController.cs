using FTM.API.Helpers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/donations")]
    public class FTFundDonationController : ControllerBase
    {
        private readonly IFTFundDonationService _donationService;
        private readonly ILogger<FTFundDonationController> _logger;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IFTMemberService _memberService;

        public FTFundDonationController(
            IFTFundDonationService donationService,
            ILogger<FTFundDonationController> logger,
            IBlobStorageService blobStorageService,
            IFTMemberService memberService)
        {
            _donationService = donationService;
            _logger = logger;
            _blobStorageService = blobStorageService;
            _memberService = memberService;
        }

        /// <summary>
        /// Get all donations for a fund
        /// </summary>
        [HttpGet("fund/{fundId}")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<IActionResult> GetDonationsByFund(Guid fundId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _donationService.GetDonationsByFundAsync(fundId, page, pageSize);

                var donationDtos = result.Items.Select(d => new
                {
                    d.Id,
                    d.DonationMoney,
                    DonorName = d.Member?.Fullname ?? d.DonorName ?? "Anonymous",
                    d.PaymentMethod,
                    d.PaymentNotes,
                    d.Status,
                    CreatedDate = d.CreatedOn,
                    d.ConfirmedOn,
                    ConfirmerName = d.Confirmer?.Fullname,
                    d.ConfirmationNotes,
                    FundName = d.Fund?.FundName,
                    d.PayOSOrderCode
                });

                var response = new
                {
                    Donations = donationDtos,
                    TotalCount = result.TotalCount,
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalPages = (int)Math.Ceiling((double)result.TotalCount / result.PageSize)
                };

                return Ok(new ApiSuccess("Donations retrieved successfully", response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting donations for fund {FundId}", fundId);
                return StatusCode(500, new ApiError("Error retrieving donations", ex.Message));
            }
        }

        /// <summary>
        /// Get pending donations for confirmation
        /// </summary>
        [HttpGet("pending")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<IActionResult> GetPendingDonations([FromQuery] Guid? fundId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _donationService.GetPendingDonationsAsync(fundId, page, pageSize);

                var donationDtos = result.Items.Select(d => new
                {
                    d.Id,
                    d.DonationMoney,
                    DonorName = d.Member?.Fullname ?? d.DonorName ?? "Anonymous",
                    d.PaymentMethod,
                    d.PaymentNotes,
                    d.Status,
                    CreatedDate = d.CreatedOn,
                    FundName = d.Fund?.FundName,
                    d.PayOSOrderCode,
                    d.ProofImages
                });

                var response = new
                {
                    Donations = donationDtos,
                    TotalCount = result.TotalCount,
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalPages = (int)Math.Ceiling((double)result.TotalCount / result.PageSize)
                };

                return Ok(new ApiSuccess("Pending donations retrieved successfully", response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending donations");
                return StatusCode(500, new ApiError("Error retrieving pending donations", ex.Message));
            }
        }

        /// <summary>
        /// Get my pending donations (donations waiting for proof upload or confirmation)
        /// Used by FE to show user their own pending donations that need proof images
        /// </summary>
        [HttpGet("my-pending")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<IActionResult> GetMyPendingDonations([FromQuery] Guid? userId = null)
        {
            try
            {
                if (!userId.HasValue)
                    return BadRequest(new ApiError("User ID is required"));

                var donations = await _donationService.GetUserPendingDonationsAsync(userId.Value);

                var donationDtos = donations.Select(d => new
                {
                    d.Id,
                    d.DonationMoney,
                    d.PaymentMethod,
                    d.PaymentNotes,
                    d.Status,
                    CreatedDate = d.CreatedOn,
                    FundName = d.Fund?.FundName,
                    d.PayOSOrderCode,
                    d.CreatedByUserId,
                    HasProofImages = !string.IsNullOrEmpty(d.ProofImages),
                    ProofImageCount = string.IsNullOrEmpty(d.ProofImages) ? 0 : d.ProofImages.Split(',').Length
                });

                return Ok(new ApiSuccess("My pending donations retrieved successfully", donationDtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my pending donations");
                return StatusCode(500, new ApiError("Error retrieving my pending donations", ex.Message));
            }
        }

        /// <summary>
        /// Get donation by ID
        /// </summary>
        [HttpGet("{donationId}")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<IActionResult> GetDonationById(Guid donationId)
        {
            try
            {
                var donation = await _donationService.GetByIdAsync(donationId);

                if (donation == null)
                {
                    return NotFound(new ApiError("Donation not found"));
                }

                var donationDto = new
                {
                    donation.Id,
                    donation.DonationMoney,
                    DonorName = donation.Member?.Fullname ?? donation.DonorName ?? "Anonymous",
                    donation.PaymentMethod,
                    donation.PaymentNotes,
                    donation.Status,
                    CreatedDate = donation.CreatedOn,
                    donation.ConfirmedOn,
                    ConfirmerName = donation.Confirmer?.Fullname,
                    donation.ConfirmationNotes,
                    FundName = donation.Fund?.FundName,
                    donation.PayOSOrderCode,
                    donation.ProofImages
                };

                return Ok(new ApiSuccess("Donation retrieved successfully", donationDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting donation {DonationId}", donationId);
                return StatusCode(500, new ApiError("Error retrieving donation", ex.Message));
            }
        }

        /// <summary>
        /// Confirm a donation (for cash payments or manual confirmation)
        /// </summary>
        [HttpPost("{donationId}/confirm")]
        [FTAuthorize(MethodType.UPDATE, FeatureType.FUND)]
        public async Task<IActionResult> ConfirmDonation(Guid donationId, [FromBody] ConfirmDonationRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
                var ftId = Guid.Parse(Request.Headers["X-Ftid"].ToString());

                // Get FTMember from UserId and FTId
                var memberDto = await _memberService.GetByUserId(ftId, userId);
                if (memberDto == null)
                    return BadRequest(new ApiError("Member not found in this family tree"));

                var donation = await _donationService.ConfirmDonationAsync(donationId, memberDto.Id, request.Notes);

                _logger.LogInformation("Confirmed donation {DonationId} by member {MemberId}", donationId, memberDto.Id);

                return Ok(new ApiSuccess("Donation confirmed successfully", new
                {
                    DonationId = donation.Id,
                    donation.Status,
                    Amount = donation.DonationMoney,
                    ConfirmedAt = donation.ConfirmedOn
                }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming donation {DonationId}", donationId);
                return StatusCode(500, new ApiError("Error confirming donation", ex.Message));
            }
        }

        /// <summary>
        /// Reject a donation
        /// </summary>
        [HttpPost("{donationId}/reject")]
        [FTAuthorize(MethodType.UPDATE, FeatureType.FUND)]
        public async Task<IActionResult> RejectDonation(Guid donationId, [FromBody] RejectDonationRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
                var ftId = Guid.Parse(Request.Headers["X-Ftid"].ToString());

                // Get FTMember from UserId and FTId
                var memberDto = await _memberService.GetByUserId(ftId, userId);
                if (memberDto == null)
                    return BadRequest(new ApiError("Member not found in this family tree"));

                var donation = await _donationService.RejectDonationAsync(donationId, memberDto.Id, request.Reason);

                _logger.LogInformation("Rejected donation {DonationId} by member {MemberId}", donationId, memberDto.Id);

                return Ok(new ApiSuccess("Donation rejected successfully", new
                {
                    DonationId = donation.Id,
                    donation.Status,
                    Reason = donation.ConfirmationNotes
                }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting donation {DonationId}", donationId);
                return StatusCode(500, new ApiError("Error rejecting donation", ex.Message));
            }
        }

        /// <summary>
        /// Get donation statistics for a fund
        /// </summary>
        [HttpGet("fund/{fundId}/statistics")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<IActionResult> GetDonationStats(Guid fundId)
        {
            try
            {
                var stats = await _donationService.GetDonationStatisticsAsync(fundId);

                return Ok(new ApiSuccess("Donation statistics retrieved successfully", stats));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting donation statistics for fund {FundId}", fundId);
                return StatusCode(500, new ApiError("Error retrieving statistics", ex.Message));
            }
        }

        /// <summary>
        /// Update donation details (before confirmation)
        /// </summary>
        [HttpPut("{donationId}")]
        [FTAuthorize(MethodType.UPDATE, FeatureType.FUND)]
        public async Task<IActionResult> UpdateDonation(Guid donationId, [FromBody] UpdateDonationRequest request)
        {
            try
            {
                var donation = new FTFundDonation
                {
                    Id = donationId,
                    DonationMoney = request.Amount,
                    DonorName = request.DonorName,
                    PaymentNotes = request.PaymentNotes,
                    ProofImages = request.ProofImages
                };

                var updated = await _donationService.UpdateDonationAsync(donation);

                _logger.LogInformation("Updated donation {DonationId}", donationId);

                return Ok(new ApiSuccess("Donation updated successfully", new { DonationId = updated.Id }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating donation {DonationId}", donationId);
                return StatusCode(500, new ApiError("Error updating donation", ex.Message));
            }
        }

        /// <summary>
        /// Delete donation (soft delete, only for pending donations)
        /// </summary>
        [HttpDelete("{donationId}")]
        [FTAuthorize(MethodType.DELETE, FeatureType.FUND)]
        public async Task<IActionResult> DeleteDonation(Guid donationId)
        {
            try
            {
                await _donationService.DeleteDonationAsync(donationId);

                _logger.LogInformation("Deleted donation {DonationId}", donationId);

                return Ok(new ApiSuccess("Donation deleted successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting donation {DonationId}", donationId);
                return StatusCode(500, new ApiError("Error deleting donation", ex.Message));
            }
        }

        /// <summary>
        /// Upload proof images to blob storage for a specific fund donation
        /// Images are automatically linked to the donation upon upload
        /// </summary>
        [HttpPost("{donationId}/upload-proof")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<IActionResult> UploadProofImages(Guid donationId, [FromForm] List<IFormFile> images)
        {
            try
            {
                var donation = await _donationService.GetByIdAsync(donationId);

                if (donation == null)
                {
                    return NotFound(new ApiError("Donation not found"));
                }

                if (donation.Status != DonationStatus.Pending)
                {
                    return BadRequest(new ApiError("Can only upload proof for pending donations"));
                }

                if (images == null || images.Count == 0)
                {
                    return BadRequest(new ApiError("No images provided"));
                }

                var uploadedUrls = new List<string>();

                foreach (var image in images)
                {
                    var imageUrl = await _blobStorageService.UploadFileAsync(
                        image,
                        "donations",
                        $"{donationId}/proof/{Guid.NewGuid()}{Path.GetExtension(image.FileName)}");

                    uploadedUrls.Add(imageUrl);
                }

                // Update donation with proof images
                donation.ProofImages = string.Join(",", uploadedUrls);
                await _donationService.UpdateDonationAsync(donation);

                _logger.LogInformation("Uploaded {Count} proof images for donation {DonationId}", images.Count, donationId);

                return Ok(new ApiSuccess("Proof images uploaded successfully", new
                {
                    DonationId = donationId,
                    ImageUrls = uploadedUrls,
                    TotalImages = uploadedUrls.Count
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading proof images for donation {DonationId}", donationId);
                return StatusCode(500, new ApiError("Error uploading proof images", ex.Message));
            }
        }

        /// <summary>
        /// Confirm fund donation (proof images should be uploaded via upload-proof endpoint first)
        /// </summary>
        [HttpPost("{donationId}/confirm-with-proof")]
        [FTAuthorize(MethodType.UPDATE, FeatureType.FUND)]
        public async Task<IActionResult> ConfirmFundDonation(Guid donationId)
        {
            try
            {
                var confirmerId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());

                var donation = await _donationService.GetByIdAsync(donationId);

                if (donation == null)
                {
                    return NotFound(new ApiError("Donation not found"));
                }

                if (string.IsNullOrEmpty(donation.ProofImages))
                {
                    return BadRequest(new ApiError("Please upload proof images first using the upload-proof endpoint"));
                }

                var confirmed = await _donationService.ConfirmDonationAsync(donationId, confirmerId, "Confirmed with proof images");

                _logger.LogInformation("Confirmed donation {DonationId} with proof images", donationId);

                return Ok(new ApiSuccess("Donation confirmed successfully", new
                {
                    DonationId = confirmed.Id,
                    Status = confirmed.Status.ToString(),
                    StatusCode = (int)confirmed.Status,
                    Amount = confirmed.DonationMoney,
                    ConfirmedAt = confirmed.ConfirmedOn,
                    ProofImages = confirmed.ProofImages?.Split(','),
                    ConfirmedBy = confirmed.ConfirmedBy
                }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming donation {DonationId}", donationId);
                return BadRequest(new ApiError("Error confirming donation", ex.Message));
            }
        }
    }

    #region DTOs

    public class ConfirmDonationRequest
    {
        public string? Notes { get; set; }
    }

    public class RejectDonationRequest
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class UpdateDonationRequest
    {
        public decimal Amount { get; set; }
        public string? DonorName { get; set; }
        public string? PaymentNotes { get; set; }
        public string? ProofImages { get; set; }
    }

    #endregion
}
