using Microsoft.AspNetCore.Mvc;
using FTM.Domain.Entities.Funds;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using FTM.API.Reponses;
using FTM.Domain.Enums;
using FTM.Infrastructure.Services;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/funds")]
    public class FTFundController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayOSPaymentService _paymentService;
        private readonly ILogger<FTFundController> _logger;

        public FTFundController(
            IUnitOfWork unitOfWork,
            IPayOSPaymentService paymentService,
            ILogger<FTFundController> logger)
        {
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Get all funds for a family tree
        /// </summary>
        [HttpGet("tree/{treeId}")]
        public async Task<IActionResult> GetFundsByTreeId(Guid treeId)
        {
            try
            {
                var funds = await _unitOfWork.Repository<FTFund>().GetQuery()
                    .Where(f => f.FTId == treeId && f.IsDeleted == false)
                    .ToListAsync();

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
        public async Task<IActionResult> CreateFund([FromBody] CreateFundRequest request)
        {
            try
            {
                var fund = new FTFund
                {
                    Id = Guid.NewGuid(),
                    FTId = request.FamilyTreeId,
                    FundName = request.FundName,
                    FundNote = request.Description,
                    CurrentMoney = 0,
                    IsDeleted = false,
                    // Bank account info
                    BankAccountNumber = request.BankAccountNumber,
                    BankCode = request.BankCode,
                    BankName = request.BankName,
                    AccountHolderName = request.AccountHolderName
                };

                await _unitOfWork.Repository<FTFund>().AddAsync(fund);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Created new fund {FundId} for tree {TreeId}", fund.Id, request.FamilyTreeId);

                return Ok(new ApiSuccess("Fund created successfully", new { FundId = fund.Id }));
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
        public async Task<IActionResult> UpdateFund(Guid fundId, [FromBody] UpdateFundRequest request)
        {
            try
            {
                var fund = await _unitOfWork.Repository<FTFund>().GetQuery()
                    .FirstOrDefaultAsync(f => f.Id == fundId && f.IsDeleted == false);

                if (fund == null)
                {
                    return NotFound(new ApiError("Fund not found"));
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(request.FundName))
                    fund.FundName = request.FundName;

                if (request.Description != null)
                    fund.FundNote = request.Description;

                // Update bank account info
                if (request.BankAccountNumber != null)
                    fund.BankAccountNumber = request.BankAccountNumber;

                if (request.BankCode != null)
                    fund.BankCode = request.BankCode;

                if (request.BankName != null)
                    fund.BankName = request.BankName;

                if (request.AccountHolderName != null)
                    fund.AccountHolderName = request.AccountHolderName;

                if (request.FundManagers != null)
                    fund.FundManagers = request.FundManagers;

                _unitOfWork.Repository<FTFund>().Update(fund);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Updated fund {FundId}", fundId);

                return Ok(new ApiSuccess("Fund updated successfully", new { FundId = fund.Id }));
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
        public async Task<IActionResult> DonateTo(Guid fundId, [FromBody] DonateRequest request)
        {
            try
            {
                var fund = await _unitOfWork.Repository<FTFund>().GetQuery()
                    .FirstOrDefaultAsync(f => f.Id == fundId && f.IsDeleted == false);

                if (fund == null)
                {
                    return NotFound(new ApiError("Fund not found"));
                }

                var donation = new FTFundDonation
                {
                    Id = Guid.NewGuid(),
                    FTFundId = fundId,
                    FTMemberId = request.MemberId,
                    DonationMoney = request.Amount,
                    DonorName = request.DonorName,
                    PaymentMethod = request.PaymentMethod,
                    PaymentNotes = request.PaymentNotes,
                    Status = DonationStatus.Pending,
                    IsDeleted = false
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

                await _unitOfWork.Repository<FTFundDonation>().AddAsync(donation);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Created donation {DonationId} for fund {FundId}", donation.Id, fundId);

                var result = new
                {
                    DonationId = donation.Id,
                    OrderCode = donation.PayOSOrderCode?.ToString(),
                    QRCodeUrl = qrCodeUrl,
                    BankInfo = request.PaymentMethod == PaymentMethod.BankTransfer ? new
                    {
                        BankCode = fund.BankCode,
                        BankName = fund.BankName,
                        AccountNumber = fund.BankAccountNumber,
                        AccountHolderName = fund.AccountHolderName,
                        Amount = donation.DonationMoney,
                        Description = $"UH {donation.PayOSOrderCode}"
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