using FTM.API.Helpers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/fund-expenses")]
    public class FTFundExpenseController : ControllerBase
    {
        private readonly IFTFundExpenseService _expenseService;
        private readonly ILogger<FTFundExpenseController> _logger;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IFTMemberService _memberService;

        public FTFundExpenseController(
            IFTFundExpenseService expenseService,
            ILogger<FTFundExpenseController> logger,
            IBlobStorageService blobStorageService,
            IFTMemberService memberService)
        {
            _expenseService = expenseService;
            _logger = logger;
            _blobStorageService = blobStorageService;
            _memberService = memberService;
        }

        /// <summary>
        /// Get all expenses for a fund
        /// </summary>
        [HttpGet("fund/{fundId}")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<IActionResult> GetExpensesByFund(Guid fundId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _expenseService.GetExpensesByFundAsync(fundId, page, pageSize);

                var expenseDtos = result.Items.Select(e => new
                {
                    e.Id,
                    e.ExpenseAmount,
                    e.ExpenseDescription,
                    e.ExpenseEvent,
                    e.Recipient,
                    e.Status,
                    CreatedDate = e.CreatedOn,
                    e.ApprovedOn,
                    ApproverName = e.Approver?.Fullname,
                    e.ApprovalFeedback,
                    FundName = e.Fund?.FundName,
                    e.ReceiptImages,
                    e.PaymentProofImage
                });

                var response = new
                {
                    Expenses = expenseDtos,
                    TotalCount = result.TotalCount,
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalPages = (int)Math.Ceiling((double)result.TotalCount / result.PageSize)
                };

                return Ok(new ApiSuccess("Expenses retrieved successfully", response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expenses for fund {FundId}", fundId);
                return StatusCode(500, new ApiError("Error retrieving expenses", ex.Message));
            }
        }

        /// <summary>
        /// Get pending expenses for approval
        /// </summary>
        [HttpGet("pending")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<IActionResult> GetPendingExpenses([FromQuery] Guid fundId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var result = await _expenseService.GetPendingExpensesAsync(fundId, page, pageSize);

                var expenseDtos = result.Items.Select(e => new
                {
                    e.Id,
                    e.ExpenseAmount,
                    e.ExpenseDescription,
                    e.ExpenseEvent,
                    e.Recipient,
                    e.Status,
                    CreatedDate = e.CreatedOn,
                    FundId = e.FTFundId,
                    FundName = e.Fund?.FundName,
                    e.ReceiptImages
                });

                var response = new
                {
                    Expenses = expenseDtos,
                    TotalCount = result.TotalCount,
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalPages = (int)Math.Ceiling((double)result.TotalCount / result.PageSize)
                };

                return Ok(new ApiSuccess("Pending expenses retrieved successfully", response));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending expenses");
                return StatusCode(500, new ApiError("Error retrieving pending expenses", ex.Message));
            }
        }

        /// <summary>
        /// Get expense by ID
        /// </summary>
        [HttpGet("{expenseId}")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<IActionResult> GetExpenseById(Guid expenseId)
        {
            try
            {
                var expense = await _expenseService.GetByIdAsync(expenseId);

                if (expense == null)
                {
                    return NotFound(new ApiError("Expense not found"));
                }

                var expenseDto = new
                {
                    expense.Id,
                    expense.ExpenseAmount,
                    expense.ExpenseDescription,
                    expense.ExpenseEvent,
                    expense.Recipient,
                    expense.Status,
                    CreatedDate = expense.CreatedOn,
                    expense.ApprovedOn,
                    ApproverName = expense.Approver?.Fullname,
                    expense.ApprovalFeedback,
                    FundName = expense.Fund?.FundName,
                    expense.ReceiptImages,
                    expense.PaymentProofImage
                };

                return Ok(new ApiSuccess("Expense retrieved successfully", expenseDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expense {ExpenseId}", expenseId);
                return StatusCode(500, new ApiError("Error retrieving expense", ex.Message));
            }
        }

        /// <summary>
        /// Get expense statistics for a fund
        /// </summary>
        [HttpGet("fund/{fundId}/statistics")]
        [FTAuthorize(MethodType.VIEW, FeatureType.FUND)]
        public async Task<IActionResult> GetExpenseStatistics(Guid fundId)
        {
            try
            {
                var stats = await _expenseService.GetExpenseStatisticsAsync(fundId);

                return Ok(new ApiSuccess("Expense statistics retrieved successfully", stats));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expense statistics for fund {FundId}", fundId);
                return StatusCode(500, new ApiError("Error retrieving statistics", ex.Message));
            }
        }

        /// <summary>
        /// Create a new expense request with receipt images
        /// Member uploads receipts when creating expense
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromForm] CreateExpenseRequest request)
        {
            try
            {
                var receiptUrls = new List<string>();

                if (request.ReceiptImages != null && request.ReceiptImages.Count > 0)
                {
                    foreach (var image in request.ReceiptImages)
                    {
                        var imageUrl = await _blobStorageService.UploadFileAsync(
                            image,
                            "expenses",
                            $"{Guid.NewGuid()}/receipts/{Guid.NewGuid()}{Path.GetExtension(image.FileName)}");

                        receiptUrls.Add(imageUrl);
                    }
                }

                var expense = new FTFundExpense
                {
                    FTFundId = request.FundId,
                    CampaignId = request.CampaignId,
                    ExpenseAmount = request.Amount,
                    ExpenseDescription = request.Description,
                    ExpenseEvent = request.Event,
                    Recipient = request.Recipient,
                    PlannedDate = request.PlannedDate,
                    ReceiptImages = receiptUrls.Count > 0 ? string.Join(",", receiptUrls) : null
                };

                var created = await _expenseService.CreateExpenseAsync(expense);

                _logger.LogInformation("Created expense {ExpenseId} for fund {FundId}", created.Id, request.FundId);

                return Ok(new ApiSuccess("Expense created successfully", new
                {
                    ExpenseId = created.Id,
                    ReceiptCount = receiptUrls.Count
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating expense for fund {FundId}", request.FundId);
                return StatusCode(500, new ApiError("Error creating expense", ex.Message));
            }
        }

        /// <summary>
        /// Update pending expense
        /// </summary>
        [HttpPut("{expenseId}")]
        [FTAuthorize(MethodType.UPDATE, FeatureType.FUND)]
        public async Task<IActionResult> UpdateExpense(Guid expenseId, [FromBody] UpdateExpenseRequest request)
        {
            try
            {
                var expense = new FTFundExpense
                {
                    Id = expenseId,
                    ExpenseAmount = request.Amount,
                    ExpenseDescription = request.Description,
                    ExpenseEvent = request.Event,
                    ReceiptImages = request.ReceiptImages
                };

                var updated = await _expenseService.UpdateExpenseAsync(expense);

                _logger.LogInformation("Updated expense {ExpenseId}", expenseId);

                return Ok(new ApiSuccess("Expense updated successfully", new { ExpenseId = updated.Id }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expense {ExpenseId}", expenseId);
                return StatusCode(500, new ApiError("Error updating expense", ex.Message));
            }
        }

        /// <summary>
        /// Delete pending expense (soft delete)
        /// </summary>
        [HttpDelete("{expenseId}")]
        [FTAuthorize(MethodType.DELETE, FeatureType.FUND)]
        public async Task<IActionResult> DeleteExpense(Guid expenseId)
        {
            try
            {
                await _expenseService.DeleteExpenseAsync(expenseId);

                _logger.LogInformation("Deleted expense {ExpenseId}", expenseId);

                return Ok(new ApiSuccess("Expense deleted successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense {ExpenseId}", expenseId);
                return StatusCode(500, new ApiError("Error deleting expense", ex.Message));
            }
        }

        /// <summary>
        /// Approve expense with payment proof and deduct from fund balance
        /// Manager must upload payment proof when approving
        /// </summary>
        [HttpPost("{expenseId}/approve")]
        [FTAuthorize(MethodType.UPDATE, FeatureType.FUND)]
        [FTAuthorizeOwner]
        public async Task<IActionResult> ApproveExpense(Guid expenseId, [FromForm] ApproveExpenseRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
                var ftId = Guid.Parse(Request.Headers["X-Ftid"].ToString());

                // Get FTMember from UserId and FTId
                var memberDto = await _memberService.GetByUserId(ftId, userId);
                if (memberDto == null)
                    return BadRequest(new ApiError("Member not found in this family tree"));

                string? paymentProofUrl = null;

                if (request.PaymentProof != null)
                {
                    paymentProofUrl = await _blobStorageService.UploadFileAsync(
                        request.PaymentProof,
                        "expenses",
                        $"{expenseId}/payment-proof/{Guid.NewGuid()}{Path.GetExtension(request.PaymentProof.FileName)}");
                }

                var expense = await _expenseService.ApproveExpenseAsync(expenseId, memberDto.Id, request.Notes, paymentProofUrl);

                _logger.LogInformation("Approved expense {ExpenseId} by member {MemberId}", expenseId, memberDto.Id);

                return Ok(new ApiSuccess("Expense approved successfully", new
                {
                    ExpenseId = expense.Id,
                    expense.Status,
                    Amount = expense.ExpenseAmount,
                    ApprovedAt = expense.ApprovedOn,
                    PaymentProof = paymentProofUrl
                }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving expense {ExpenseId}", expenseId);
                return StatusCode(500, new ApiError("Error approving expense", ex.Message));
            }
        }

        /// <summary>
        /// Reject expense with reason
        /// </summary>
        [HttpPost("{expenseId}/reject")]
        [FTAuthorize(MethodType.UPDATE, FeatureType.FUND)]
        [FTAuthorizeOwner]
        public async Task<IActionResult> RejectExpense(Guid expenseId, [FromBody] RejectExpenseRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
                var ftId = Guid.Parse(Request.Headers["X-Ftid"].ToString());

                // Get FTMember from UserId and FTId
                var memberDto = await _memberService.GetByUserId(ftId, userId);
                if (memberDto == null)
                    return BadRequest(new ApiError("Member not found in this family tree"));

                var expense = await _expenseService.RejectExpenseAsync(expenseId, memberDto.Id, request.Reason);

                _logger.LogInformation("Rejected expense {ExpenseId} by member {MemberId}", expenseId, memberDto.Id);

                return Ok(new ApiSuccess("Expense rejected successfully", new
                {
                    ExpenseId = expense.Id,
                    expense.Status,
                    Reason = expense.ApprovalFeedback
                }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting expense {ExpenseId}", expenseId);
                return StatusCode(500, new ApiError("Error rejecting expense", ex.Message));
            }
        }
    }

    #region DTOs

    public class CreateExpenseRequest
    {
        public Guid FundId { get; set; }
        public Guid? CampaignId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Event { get; set; }
        public string? Recipient { get; set; }
        public DateTimeOffset? PlannedDate { get; set; }
        public List<IFormFile>? ReceiptImages { get; set; }
    }

    public class UpdateExpenseRequest
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Event { get; set; }
        public string? ReceiptImages { get; set; }
    }

    public class ApproveExpenseRequest
    {
        public string? Notes { get; set; }
        public IFormFile? PaymentProof { get; set; }
    }

    public class RejectExpenseRequest
    {
        public string Reason { get; set; } = string.Empty;
    }

    #endregion
}
