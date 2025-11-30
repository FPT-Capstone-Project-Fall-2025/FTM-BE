using FTM.API.Helpers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/funds")]
    public class FTFundController : ControllerBase
    {
        private readonly IFTFundService _fundService;
        private readonly IFTFundDonationService _donationService;
        private readonly IPayOSPaymentService _paymentService;
        private readonly ILogger<FTFundController> _logger;

        public FTFundController(
            IFTFundService fundService,
            IFTFundDonationService donationService,
            IPayOSPaymentService paymentService,
            ILogger<FTFundController> logger)
        {
            _fundService = fundService;
            _donationService = donationService;
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Get all funds for a family tree
        /// </summary>
        [HttpGet("tree/{treeId}")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<IActionResult> GetFundsByTreeId(Guid treeId)
        {
            try
            {
                var funds = await _fundService.GetFundsByTreeAsync(treeId);

                var fundDtos = funds.Select(f => new
                {
                    f.Id,
                    f.FundName,
                    Description = f.FundNote,
                    f.CurrentMoney,
                    DonationCount = f.Donations?.Count ?? 0,
                    ExpenseCount = f.Expenses?.Count ?? 0,
                    BankInfo = new
                    {
                        f.BankAccountNumber,
                        f.BankName,
                        f.BankCode,
                        f.AccountHolderName
                    }
                });

                return Ok(new ApiSuccess("Funds retrieved successfully", fundDtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting funds for tree {TreeId}", treeId);
                return StatusCode(500, new ApiError("Error retrieving funds", ex.Message));
            }
        }

        /// <summary>
        /// Create a new fund
        /// </summary>
        [HttpPost]
        [FTAuthorize(MethodType.ADD, FeatureType.FUND)]
        public async Task<IActionResult> CreateFund([FromBody] CreateFundRequest request)
        {
            try
            {
                var fund = new FTFund
                {
                    FTId = request.FamilyTreeId,
                    FundName = request.FundName,
                    FundNote = request.Description,
                    BankAccountNumber = request.BankAccountNumber,
                    BankCode = request.BankCode,
                    BankName = request.BankName,
                    AccountHolderName = request.AccountHolderName
                };

                var created = await _fundService.CreateFundAsync(fund);

                _logger.LogInformation("Created new fund {FundId} for tree {TreeId}", created.Id, request.FamilyTreeId);

                return Ok(new ApiSuccess("Fund created successfully", new { FundId = created.Id }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating fund for tree {TreeId}", request.FamilyTreeId);
                return StatusCode(500, new ApiError("Error creating fund", ex.Message));
            }
        }

        /// <summary>
        /// Update fund information
        /// </summary>
        [HttpPut("{fundId}")]
        [FTAuthorize(MethodType.UPDATE, FeatureType.FUND)]
        public async Task<IActionResult> UpdateFund(Guid fundId, [FromBody] UpdateFundRequest request)
        {
            try
            {
                var fund = new FTFund
                {
                    FundName = request.FundName,
                    FundNote = request.Description,
                    BankAccountNumber = request.BankAccountNumber,
                    BankCode = request.BankCode,
                    BankName = request.BankName,
                    AccountHolderName = request.AccountHolderName,
                    FundManagers = request.FundManagers
                };

                var updated = await _fundService.UpdateFundAsync(fundId, fund);

                _logger.LogInformation("Updated fund {FundId}", fundId);

                return Ok(new ApiSuccess("Fund updated successfully", new { FundId = updated.Id }));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating fund {FundId}", fundId);
                return StatusCode(500, new ApiError("Error updating fund", ex.Message));
            }
        }

        /// <summary>
        /// Make a donation to fund
        /// </summary>
        [HttpPost("{fundId}/donate")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<IActionResult> DonateTo(Guid fundId, [FromBody] DonateRequest request)
        {
            try
            {
                var fund = await _fundService.GetByIdAsync(fundId);

                if (fund == null)
                {
                    return NotFound(new ApiError("Fund not found"));
                }

                var donation = new FTFundDonation
                {
                    FTFundId = fundId,
                    FTMemberId = request.MemberId,
                    DonationMoney = request.Amount,
                    DonorName = request.DonorName,
                    PaymentMethod = request.PaymentMethod,
                    PaymentNotes = request.PaymentNotes,
                    Status = DonationStatus.Pending
                };

                string? qrCodeUrl = null;

                // For online payments, create VietQR
                if (request.PaymentMethod == PaymentMethod.BankTransfer)
                {
                    // Validate bank account info
                    if (string.IsNullOrEmpty(fund.BankAccountNumber) || 
                        string.IsNullOrEmpty(fund.BankName) ||
                        string.IsNullOrEmpty(fund.AccountHolderName))
                    {
                        return BadRequest(new ApiError("Fund has not set up bank account information. Please contact fund manager."));
                    }

                    // Generate order code
                    var orderCode = _paymentService.GenerateOrderCode();
                    donation.PayOSOrderCode = orderCode;

                    // Generate VietQR code
                    var description = $"UH {orderCode}"; // Transfer content
                    qrCodeUrl = _paymentService.GenerateVietQRUrl(
                        fund.BankCode ?? "970436", // Default to Vietcombank
                        fund.BankAccountNumber,
                        fund.AccountHolderName,
                        donation.DonationMoney,
                        description);

                    _logger.LogInformation("Created VietQR for fund donation {DonationId}, OrderCode: {OrderCode}", 
                        donation.Id, orderCode);
                }

                var created = await _donationService.CreateDonationAsync(donation);

                _logger.LogInformation("Created donation {DonationId} for fund {FundId}", created.Id, fundId);

                var result = new
                {
                    DonationId = created.Id,
                    OrderCode = created.PayOSOrderCode?.ToString(),
                    QRCodeUrl = qrCodeUrl,
                    BankInfo = request.PaymentMethod == PaymentMethod.BankTransfer ? new
                    {
                        BankCode = fund.BankCode,
                        BankName = fund.BankName,
                        AccountNumber = fund.BankAccountNumber,
                        AccountHolderName = fund.AccountHolderName,
                        Amount = created.DonationMoney,
                        Description = $"UH {created.PayOSOrderCode}"
                    } : null,
                    RequiresManualConfirmation = request.PaymentMethod == PaymentMethod.Cash || qrCodeUrl == null,
                    Message = request.PaymentMethod == PaymentMethod.Cash 
                        ? "Cash donation recorded. Waiting for manager confirmation." 
                        : "Please scan QR code to complete payment. Donation will be confirmed after successful transfer."
                };

                return Ok(new ApiSuccess("Donation created successfully", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating donation for fund {FundId}", fundId);
                return StatusCode(500, new ApiError("Error creating donation", ex.Message));
            }
        }
    }

    #region DTOs

    public class CreateFundRequest
    {
        public Guid FamilyTreeId { get; set; }
        public string FundName { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        // Bank account information for receiving donations
        public string? BankAccountNumber { get; set; }
        public string? BankCode { get; set; }
        public string? BankName { get; set; }
        public string? AccountHolderName { get; set; }
    }

    public class UpdateFundRequest
    {
        public string? FundName { get; set; }
        public string? Description { get; set; }
        
        // Bank account information for receiving donations
        public string? BankAccountNumber { get; set; }
        public string? BankCode { get; set; }
        public string? BankName { get; set; }
        public string? AccountHolderName { get; set; }
        
        // Fund managers list (JSON array of member IDs)
        public string? FundManagers { get; set; }
    }

    public class DonateRequest
    {
        public Guid? MemberId { get; set; }
        public string? DonorName { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? PaymentNotes { get; set; }
        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
    }

    #endregion
}