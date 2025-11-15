using Microsoft.AspNetCore.Mvc;
using FTM.Domain.Entities.Funds;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using FTM.API.Reponses;
using FTM.Domain.Enums;
using FTM.Application.IServices;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/fund-expenses")]
    public class FTFundExpenseController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FTFundExpenseController> _logger;
        private readonly IBlobStorageService _blobStorageService;

        public FTFundExpenseController(
            IUnitOfWork unitOfWork,
            ILogger<FTFundExpenseController> logger,
            IBlobStorageService blobStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _blobStorageService = blobStorageService;
        }

        /// <summary>
        /// Get all expenses for a fund
        /// </summary>
        [HttpGet("fund/{fundId}")]
        public async Task<IActionResult> GetExpensesByFund(Guid fundId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] TransactionStatus? status = null)
        {
            try
            {
                IQueryable<FTFundExpense> query = _unitOfWork.Repository<FTFundExpense>().GetQuery()
                    .Where(e => e.FTFundId == fundId && e.IsDeleted == false)
                    .Include(e => e.Fund)
                    .Include(e => e.Approver)
                    .Include(e => e.Campaign);

                // Filter by status if provided
                if (status.HasValue)
                {
                    query = query.Where(e => e.Status == status.Value);
                }

                var totalCount = await query.CountAsync();

                var expenses = await query
                    .OrderByDescending(e => e.CreatedOn)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var expenseDtos = expenses.Select(e => new
                {
                    e.Id,
                    e.ExpenseAmount,
                    e.ExpenseDescription,
                    e.PlannedDate,
                    e.ExpenseEvent,
                    e.Recipient,
                    Status = e.Status.ToString(),
                    CreatedDate = e.CreatedOn,
                    e.ApprovalFeedback,
                    e.ApprovedBy,
                    e.ApprovedOn,
                    ApproverName = e.Approver?.Fullname,
                    FundName = e.Fund?.FundName,
                    CampaignName = e.Campaign?.CampaignName
                });

                var result = new
                {
                    Expenses = expenseDtos,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Ok(new ApiSuccess("Expenses retrieved successfully", result));
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
        public async Task<IActionResult> GetPendingExpenses([FromQuery] Guid? treeId = null)
        {
            try
            {
                IQueryable<FTFundExpense> query = _unitOfWork.Repository<FTFundExpense>().GetQuery()
                    .Where(e => e.Status == TransactionStatus.Pending && e.IsDeleted == false)
                    .Include(e => e.Fund)
                    .Include(e => e.Campaign);

                // Apply tree filter if provided
                if (treeId.HasValue)
                {
                    query = query.Where(e => e.Fund.FTId == treeId.Value);
                }

                var expenses = await query
                    .OrderBy(e => e.CreatedOn)
                    .ToListAsync();

                var expenseDtos = expenses.Select(e => new
                {
                    e.Id,
                    e.ExpenseAmount,
                    e.ExpenseDescription,
                    e.PlannedDate,
                    e.ExpenseEvent,
                    e.Recipient,
                    e.ReceiptImages, // Receipt/proof images uploaded with expense request
                    e.Status, // Approval status (Pending/Approved/Rejected)
                    CreatedDate = e.CreatedOn,
                    FundName = e.Fund?.FundName,
                    FundId = e.FTFundId,
                    CampaignName = e.Campaign?.CampaignName,
                    CurrentFundBalance = e.Fund?.CurrentMoney
                });

                return Ok(new ApiSuccess("Pending expenses retrieved successfully", expenseDtos));
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpenseById(Guid id)
        {
            try
            {
                var expense = await _unitOfWork.Repository<FTFundExpense>().GetQuery()
                    .Where(e => e.Id == id && e.IsDeleted == false)
                    .Include(e => e.Fund)
                    .Include(e => e.Approver)
                    .Include(e => e.Campaign)
                    .FirstOrDefaultAsync();

                if (expense == null)
                    return NotFound(new ApiError("Expense not found"));

                var expenseDto = new
                {
                    expense.Id,
                    expense.ExpenseAmount,
                    expense.ExpenseDescription,
                    expense.PlannedDate,
                    expense.ExpenseEvent,
                    expense.Recipient,
                    Status = expense.Status.ToString(),
                    CreatedDate = expense.CreatedOn,
                    expense.ApprovalFeedback,
                    expense.ApprovedBy,
                    expense.ApprovedOn,
                    ApproverName = expense.Approver?.Fullname,
                    FundName = expense.Fund?.FundName,
                    FundId = expense.FTFundId,
                    CurrentFundBalance = expense.Fund?.CurrentMoney,
                    CampaignName = expense.Campaign?.CampaignName
                };

                return Ok(new ApiSuccess("Expense retrieved successfully", expenseDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expense {ExpenseId}", id);
                return StatusCode(500, new ApiError("Error retrieving expense", ex.Message));
            }
        }

        /// <summary>
        /// Get expense statistics for a fund
        /// </summary>
        [HttpGet("fund/{fundId}/statistics")]
        public async Task<IActionResult> GetExpenseStatistics(Guid fundId)
        {
            try
            {
                var expenses = await _unitOfWork.Repository<FTFundExpense>().GetQuery()
                    .Where(e => e.FTFundId == fundId && e.IsDeleted == false)
                    .ToListAsync();

                var stats = new
                {
                    TotalExpenses = expenses.Count,
                    PendingCount = expenses.Count(e => e.Status == TransactionStatus.Pending),
                    ApprovedCount = expenses.Count(e => e.Status == TransactionStatus.Approved),
                    RejectedCount = expenses.Count(e => e.Status == TransactionStatus.Rejected),
                    TotalApprovedAmount = expenses.Where(e => e.Status == TransactionStatus.Approved).Sum(e => e.ExpenseAmount),
                    TotalPendingAmount = expenses.Where(e => e.Status == TransactionStatus.Pending).Sum(e => e.ExpenseAmount),
                    EventBreakdown = expenses
                        .Where(e => e.Status == TransactionStatus.Approved && !string.IsNullOrEmpty(e.ExpenseEvent))
                        .GroupBy(e => e.ExpenseEvent!)
                        .Select(g => new
                        {
                            Event = g.Key,
                            Count = g.Count(),
                            TotalAmount = g.Sum(e => e.ExpenseAmount)
                        })
                        .ToList()
                };

                return Ok(new ApiSuccess("Statistics retrieved successfully", stats));
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
                // Validate fund exists
                var fund = await _unitOfWork.Repository<FTFund>().GetByIdAsync(request.FundId);
                if (fund == null)
                    return NotFound(new ApiError("Fund not found"));

                // Upload receipt images to blob storage
                var receiptUrls = new List<string>();
                if (request.ReceiptImages != null && request.ReceiptImages.Any())
                {
                    foreach (var file in request.ReceiptImages)
                    {
                        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                        var url = await _blobStorageService.UploadFileAsync(file, "fund-expense-receipts", fileName);
                        receiptUrls.Add(url);
                    }
                }

                // Create expense
                var expense = new FTFundExpense
                {
                    Id = Guid.NewGuid(),
                    FTFundId = request.FundId,
                    CampaignId = request.CampaignId,
                    ExpenseAmount = request.Amount,
                    ExpenseDescription = request.Description,
                    PlannedDate = request.PlannedDate ?? DateTimeOffset.UtcNow,
                    ExpenseEvent = request.ExpenseEvent,
                    Recipient = request.Recipient,
                    ReceiptImages = receiptUrls.Any() ? string.Join(",", receiptUrls) : null,
                    Status = TransactionStatus.Pending,
                    CreatedOn = DateTimeOffset.UtcNow,
                    IsDeleted = false
                };

                await _unitOfWork.Repository<FTFundExpense>().AddAsync(expense);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Created expense request {ExpenseId} for fund {FundId} with {Count} receipt images",
                    expense.Id, request.FundId, receiptUrls.Count);

                // Warn if expense would exceed current balance
                string? warning = null;
                if (fund.CurrentMoney < expense.ExpenseAmount)
                {
                    warning = "Expense exceeds current fund balance";
                    _logger.LogWarning("Expense {ExpenseId} amount ({Amount}) exceeds fund balance ({Balance})",
                        expense.Id, expense.ExpenseAmount, fund.CurrentMoney);
                }

                return Ok(new ApiSuccess("Expense request created with receipts, pending approval", new
                {
                    ExpenseId = expense.Id,
                    Amount = expense.ExpenseAmount,
                    Description = expense.ExpenseDescription,
                    Status = expense.Status.ToString(),
                    ReceiptCount = receiptUrls.Count,
                    ReceiptUrls = receiptUrls,
                    Warning = warning
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseRequest request)
        {
            try
            {
                var expense = await _unitOfWork.Repository<FTFundExpense>().GetByIdAsync(id);
                if (expense == null || expense.IsDeleted == true)
                    return NotFound(new ApiError("Expense not found"));

                // Only allow updating pending expenses
                if (expense.Status != TransactionStatus.Pending)
                    return BadRequest(new ApiError("Cannot update non-pending expense"));

                // Update fields
                if (request.Amount.HasValue)
                    expense.ExpenseAmount = request.Amount.Value;
                if (request.Description != null)
                    expense.ExpenseDescription = request.Description;
                if (request.PlannedDate.HasValue)
                    expense.PlannedDate = request.PlannedDate.Value;
                if (request.ExpenseEvent != null)
                    expense.ExpenseEvent = request.ExpenseEvent;
                if (request.Recipient != null)
                    expense.Recipient = request.Recipient;

                _unitOfWork.Repository<FTFundExpense>().Update(expense);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Updated expense {ExpenseId}", id);

                var result = new
                {
                    expense.Id,
                    expense.ExpenseAmount,
                    expense.ExpenseDescription,
                    expense.PlannedDate,
                    expense.ExpenseEvent,
                    expense.Recipient,
                    Status = expense.Status.ToString()
                };

                return Ok(new ApiSuccess("Expense updated successfully", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expense {ExpenseId}", id);
                return StatusCode(500, new ApiError("Error updating expense", ex.Message));
            }
        }

        /// <summary>
        /// Delete pending expense (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            try
            {
                var expense = await _unitOfWork.Repository<FTFundExpense>().GetByIdAsync(id);
                if (expense == null || expense.IsDeleted == true)
                    return NotFound(new ApiError("Expense not found"));

                // Only allow deleting pending expenses
                if (expense.Status != TransactionStatus.Pending)
                    return BadRequest(new ApiError("Cannot delete non-pending expense"));

                _unitOfWork.Repository<FTFundExpense>().Delete(expense);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Deleted expense {ExpenseId}", id);

                return Ok(new ApiSuccess("Expense deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense {ExpenseId}", id);
                return StatusCode(500, new ApiError("Error deleting expense", ex.Message));
            }
        }

        /// <summary>
        /// Approve expense with payment proof and deduct from fund balance
        /// Manager must upload payment proof when approving
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveExpense(Guid id, [FromForm] ApproveExpenseRequest request)
        {
            try
            {
                // Validate payment proof is provided
                if (request.PaymentProofImage == null)
                    return BadRequest(new ApiError("Payment proof image is required for approval"));

                var expense = await _unitOfWork.Repository<FTFundExpense>().GetQuery()
                    .Where(e => e.Id == id && e.IsDeleted == false)
                    .Include(e => e.Fund)
                    .FirstOrDefaultAsync();

                if (expense == null)
                    return NotFound(new ApiError("Expense not found"));

                // Only approve pending expenses
                if (expense.Status != TransactionStatus.Pending)
                    return BadRequest(new ApiError("Expense is not pending approval"));

                // Validate fund has sufficient balance
                if (expense.Fund.CurrentMoney < expense.ExpenseAmount)
                {
                    _logger.LogWarning("Insufficient balance to approve expense {ExpenseId}. Required: {Required}, Available: {Available}",
                        id, expense.ExpenseAmount, expense.Fund.CurrentMoney);
                    return BadRequest(new ApiError($"Insufficient fund balance. Required: {expense.ExpenseAmount:N0} VND, Available: {expense.Fund.CurrentMoney:N0} VND"));
                }

                // Upload payment proof image to blob storage
                var fileName = $"{Guid.NewGuid()}_{request.PaymentProofImage.FileName}";
                var paymentProofUrl = await _blobStorageService.UploadFileAsync(request.PaymentProofImage, "fund-expense-payment-proofs", fileName);

                // Approve expense
                expense.Status = TransactionStatus.Approved;
                expense.ApprovedBy = request.ApproverId;
                expense.ApprovedOn = DateTimeOffset.UtcNow;
                expense.ApprovalFeedback = $"{request.Notes}\nPayment Proof: {paymentProofUrl}";
                expense.PaymentProofImage = paymentProofUrl;

                // Deduct from fund balance (CRITICAL LOGIC)
                expense.Fund.CurrentMoney -= expense.ExpenseAmount;

                _unitOfWork.Repository<FTFundExpense>().Update(expense);
                _unitOfWork.Repository<FTFund>().Update(expense.Fund);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Approved expense {ExpenseId}. Deducted {Amount} VND from fund {FundId}. New balance: {Balance} VND. Payment proof uploaded.",
                    id, expense.ExpenseAmount, expense.Fund.Id, expense.Fund.CurrentMoney);

                var result = new
                {
                    expense.Id,
                    Status = expense.Status.ToString(),
                    expense.ApprovedBy,
                    expense.ApprovedOn,
                    DeductedAmount = expense.ExpenseAmount,
                    NewFundBalance = expense.Fund.CurrentMoney,
                    PaymentProofUrl = paymentProofUrl
                };

                return Ok(new ApiSuccess("Expense approved with payment proof", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving expense {ExpenseId}", id);
                return StatusCode(500, new ApiError("Error approving expense", ex.Message));
            }
        }

        /// <summary>
        /// Reject expense with reason
        /// </summary>
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectExpense(Guid id, [FromBody] RejectExpenseRequest request)
        {
            try
            {
                var expense = await _unitOfWork.Repository<FTFundExpense>().GetByIdAsync(id);
                if (expense == null || expense.IsDeleted == true)
                    return NotFound(new ApiError("Expense not found"));

                // Only reject pending expenses
                if (expense.Status != TransactionStatus.Pending)
                    return BadRequest(new ApiError("Expense is not pending approval"));

                expense.Status = TransactionStatus.Rejected;
                expense.ApprovedBy = request.RejectedBy;
                expense.ApprovedOn = DateTimeOffset.UtcNow;
                expense.ApprovalFeedback = request.Reason;

                _unitOfWork.Repository<FTFundExpense>().Update(expense);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Rejected expense {ExpenseId}. Reason: {Reason}", id, request.Reason);

                var result = new
                {
                    expense.Id,
                    Status = expense.Status.ToString(),
                    expense.ApprovedBy,
                    expense.ApprovedOn,
                    expense.ApprovalFeedback
                };

                return Ok(new ApiSuccess("Expense rejected successfully", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting expense {ExpenseId}", id);
                return StatusCode(500, new ApiError("Error rejecting expense", ex.Message));
            }
        }

        #region DTOs

        public class CreateExpenseRequest
        {
            public Guid FundId { get; set; }
            public Guid? CampaignId { get; set; }
            public decimal Amount { get; set; }
            public string Description { get; set; } = string.Empty;
            public DateTimeOffset? PlannedDate { get; set; }
            public string? ExpenseEvent { get; set; }
            public string? Recipient { get; set; }
            public List<IFormFile>? ReceiptImages { get; set; }
        }

        public class UpdateExpenseRequest
        {
            public decimal? Amount { get; set; }
            public string? Description { get; set; }
            public DateTimeOffset? PlannedDate { get; set; }
            public string? ExpenseEvent { get; set; }
            public string? Recipient { get; set; }
        }

        public class ApproveExpenseRequest
        {
            public Guid ApproverId { get; set; }
            public string? Notes { get; set; }
            public IFormFile? PaymentProofImage { get; set; }
        }

        public class RejectExpenseRequest
        {
            public Guid RejectedBy { get; set; }
            public string Reason { get; set; } = string.Empty;
        }

        #endregion
    }
}
