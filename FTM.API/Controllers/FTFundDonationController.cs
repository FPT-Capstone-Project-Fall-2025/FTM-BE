using Microsoft.AspNetCore.Mvc;
using FTM.Domain.Entities.Funds;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using FTM.API.Reponses;
using FTM.Domain.Enums;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/donations")]
    public class FTFundDonationController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FTFundDonationController> _logger;

        public FTFundDonationController(
            IUnitOfWork unitOfWork,
            ILogger<FTFundDonationController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all donations for a fund
        /// </summary>
        [HttpGet("fund/{fundId}")]
        public async Task<IActionResult> GetDonationsByFund(Guid fundId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var donations = await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                    .Where(d => d.FTFundId == fundId && d.IsDeleted == false)
                    .Include(d => d.Member)
                    .Include(d => d.Fund)
                    .Include(d => d.Confirmer)
                    .OrderByDescending(d => d.CreatedOn)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var totalCount = await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                    .CountAsync(d => d.FTFundId == fundId && d.IsDeleted == false);

                var donationDtos = donations.Select(d => new
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

                var result = new
                {
                    Donations = donationDtos,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(new ApiSuccess("Donations retrieved successfully", result));
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
        public async Task<IActionResult> GetPendingDonations([FromQuery] Guid? treeId = null)
        {
            try
            {
                IQueryable<FTFundDonation> query = _unitOfWork.Repository<FTFundDonation>().GetQuery()
                    .Where(d => d.Status == DonationStatus.Pending && d.IsDeleted == false)
                    .Include(d => d.Member)
                    .Include(d => d.Fund);

                // Apply tree filter if provided
                if (treeId.HasValue)
                {
                    query = query.Where(d => d.Fund.FTId == treeId.Value);
                }

                var donations = await query
                    .OrderBy(d => d.CreatedOn)
                    .ToListAsync();

                var donationDtos = donations.Select(d => new
                {
                    d.Id,
                    d.DonationMoney,
                    DonorName = d.Member?.Fullname ?? d.DonorName ?? "Anonymous",
                    d.PaymentMethod,
                    d.PaymentNotes,
                    CreatedDate = d.CreatedOn,
                    FundName = d.Fund?.FundName,
                    FundId = d.FTFundId,
                    TreeId = d.Fund?.FTId,
                    d.PayOSOrderCode
                });

                return Ok(new ApiSuccess("Pending donations retrieved successfully", donationDtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending donations");
                return StatusCode(500, new ApiError("Error retrieving pending donations", ex.Message));
            }
        }

        /// <summary>
        /// Get donation by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDonationById(Guid id)
        {
            try
            {
                var donation = await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                    .Where(d => d.Id == id && d.IsDeleted == false)
                    .Include(d => d.Member)
                    .Include(d => d.Fund)
                    .Include(d => d.Confirmer)
                    .FirstOrDefaultAsync();

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
                    donation.PaymentTransactionId
                };

                return Ok(new ApiSuccess("Donation details retrieved successfully", donationDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting donation {DonationId}", id);
                return StatusCode(500, new ApiError("Error retrieving donation details", ex.Message));
            }
        }

        /// <summary>
        /// Confirm a donation (for cash payments or manual confirmation)
        /// </summary>
        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmDonation(Guid id, [FromBody] ConfirmDonationRequest request)
        {
            try
            {
                var donation = await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                    .Include(d => d.Fund)
                    .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false);

                if (donation == null)
                {
                    return NotFound(new ApiError("Donation not found"));
                }

                if (donation.Status == DonationStatus.Confirmed)
                {
                    return BadRequest(new ApiError("Donation already confirmed"));
                }

                // Confirm the donation
                donation.Status = DonationStatus.Confirmed;
                donation.ConfirmedBy = request.ConfirmerId;
                donation.ConfirmedOn = DateTimeOffset.UtcNow;
                donation.ConfirmationNotes = request.Notes;
                donation.LastModifiedOn = DateTimeOffset.UtcNow;

                // Update fund balance
                if (donation.Fund != null)
                {
                    donation.Fund.CurrentMoney += donation.DonationMoney;
                    donation.Fund.LastModifiedOn = DateTimeOffset.UtcNow;
                }

                _unitOfWork.Repository<FTFundDonation>().Update(donation);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Confirmed donation {DonationId} by user {ConfirmerId}", id, request.ConfirmerId);

                return Ok(new ApiSuccess("Donation confirmed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming donation {DonationId}", id);
                return StatusCode(500, new ApiError("Error confirming donation", ex.Message));
            }
        }

        /// <summary>
        /// Reject a donation
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectDonation(Guid id, [FromBody] RejectDonationRequest request)
        {
            try
            {
                var donation = await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                    .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false);

                if (donation == null)
                {
                    return NotFound(new ApiError("Donation not found"));
                }

                if (donation.Status != DonationStatus.Pending)
                {
                    return BadRequest(new ApiError("Can only reject pending donations"));
                }

                donation.Status = DonationStatus.Rejected;
                donation.ConfirmedBy = request.RejectedBy;
                donation.ConfirmedOn = DateTimeOffset.UtcNow;
                donation.ConfirmationNotes = request.Reason;
                donation.LastModifiedOn = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<FTFundDonation>().Update(donation);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Rejected donation {DonationId} by user {RejectedBy}: {Reason}", 
                    id, request.RejectedBy, request.Reason);

                return Ok(new ApiSuccess("Donation rejected successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting donation {DonationId}", id);
                return StatusCode(500, new ApiError("Error rejecting donation", ex.Message));
            }
        }

        /// <summary>
        /// Get donation statistics for a fund
        /// </summary>
        [HttpGet("fund/{fundId}/stats")]
        public async Task<IActionResult> GetDonationStats(Guid fundId)
        {
            try
            {
                var fund = await _unitOfWork.Repository<FTFund>().GetQuery()
                    .Where(f => f.Id == fundId && f.IsDeleted == false)
                    .Include(f => f.Donations.Where(d => d.IsDeleted == false))
                    .FirstOrDefaultAsync();

                if (fund == null)
                {
                    return NotFound(new ApiError("Fund not found"));
                }

                var donations = fund.Donations ?? new List<FTFundDonation>();

                var stats = new
                {
                    TotalDonations = donations.Count,
                    TotalAmount = donations.Where(d => d.Status == DonationStatus.Confirmed).Sum(d => d.DonationMoney),
                    PendingCount = donations.Count(d => d.Status == DonationStatus.Pending),
                    PendingAmount = donations.Where(d => d.Status == DonationStatus.Pending).Sum(d => d.DonationMoney),
                    ConfirmedCount = donations.Count(d => d.Status == DonationStatus.Confirmed),
                    RejectedCount = donations.Count(d => d.Status == DonationStatus.Rejected),
                    CashDonations = donations.Count(d => d.PaymentMethod == PaymentMethod.Cash),
                    OnlineDonations = donations.Count(d => d.PaymentMethod == PaymentMethod.BankTransfer),
                    AverageAmount = donations.Where(d => d.Status == DonationStatus.Confirmed && d.DonationMoney > 0)
                        .DefaultIfEmpty()
                        .Average(d => d?.DonationMoney ?? 0),
                    Fund = new
                    {
                        fund.Id,
                        fund.FundName,
                        CurrentBalance = fund.CurrentMoney,
                        fund.FundNote
                    }
                };

                return Ok(new ApiSuccess("Donation statistics retrieved successfully", stats));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting donation stats for fund {FundId}", fundId);
                return StatusCode(500, new ApiError("Error retrieving donation statistics", ex.Message));
            }
        }

        /// <summary>
        /// Update donation details (before confirmation)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDonation(Guid id, [FromBody] UpdateDonationRequest request)
        {
            try
            {
                var donation = await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                    .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false);

                if (donation == null)
                {
                    return NotFound(new ApiError("Donation not found"));
                }

                if (donation.Status != DonationStatus.Pending)
                {
                    return BadRequest(new ApiError("Can only update pending donations"));
                }

                donation.DonationMoney = request.Amount;
                donation.DonorName = request.DonorName;
                donation.PaymentNotes = request.PaymentNotes;
                donation.LastModifiedOn = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<FTFundDonation>().Update(donation);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Updated donation {DonationId}", id);

                return Ok(new ApiSuccess("Donation updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating donation {DonationId}", id);
                return StatusCode(500, new ApiError("Error updating donation", ex.Message));
            }
        }

        /// <summary>
        /// Delete donation (soft delete, only for pending donations)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonation(Guid id)
        {
            try
            {
                var donation = await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                    .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false);

                if (donation == null)
                {
                    return NotFound(new ApiError("Donation not found"));
                }

                if (donation.Status == DonationStatus.Confirmed)
                {
                    return BadRequest(new ApiError("Cannot delete confirmed donations"));
                }

                donation.IsDeleted = true;
                donation.LastModifiedOn = DateTimeOffset.UtcNow;

                _unitOfWork.Repository<FTFundDonation>().Update(donation);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Deleted donation {DonationId}", id);

                return Ok(new ApiSuccess("Donation deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting donation {DonationId}", id);
                return StatusCode(500, new ApiError("Error deleting donation", ex.Message));
            }
        }
    }

    #region DTOs

    public class ConfirmDonationRequest
    {
        public Guid ConfirmerId { get; set; }
        public string? Notes { get; set; }
    }

    public class RejectDonationRequest
    {
        public Guid RejectedBy { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class UpdateDonationRequest
    {
        public decimal Amount { get; set; }
        public string? DonorName { get; set; }
        public string? PaymentNotes { get; set; }
    }

    #endregion
}
