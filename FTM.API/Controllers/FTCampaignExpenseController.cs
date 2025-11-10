using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FTCampaignExpenseController : ControllerBase
    {
        private readonly IFTCampaignExpenseService _expenseService;
        private readonly IBlobStorageService _blobStorageService;

        public FTCampaignExpenseController(
            IFTCampaignExpenseService expenseService,
            IBlobStorageService blobStorageService)
        {
            _expenseService = expenseService;
            _blobStorageService = blobStorageService;
        }

        #region Query Operations

        /// <summary>
        /// Get expense by ID
        /// </summary>
        [HttpGet("{id:guid}")]

        public async Task<IActionResult> GetExpenseById(Guid id)
        {
            try
            {
                var expense = await _expenseService.GetByIdAsync(id);
                if (expense == null)
                    return NotFound(new ApiError("Expense not found"));

                return Ok(new ApiSuccess(expense));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get all expenses for a campaign (paginated, with optional status filter)
        /// </summary>
        [HttpGet("campaign/{campaignId:guid}")]
   
        public async Task<IActionResult> GetCampaignExpenses(
            Guid campaignId,
            [FromQuery] ApprovalStatus? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _expenseService.GetCampaignExpensesAsync(campaignId, status, page, pageSize);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get pending expenses for campaigns managed by a user
        /// </summary>
        [HttpGet("pending/manager/{managerId:guid}")]

        public async Task<IActionResult> GetPendingExpensesForManager(
            Guid managerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _expenseService.GetPendingExpensesForManagerAsync(managerId, page, pageSize);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get expense statistics for a campaign
        /// </summary>
        [HttpGet("campaign/{campaignId:guid}/statistics")]

        public async Task<IActionResult> GetExpenseStatistics(Guid campaignId)
        {
            try
            {
                var result = await _expenseService.GetExpenseStatisticsAsync(campaignId);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        #endregion

        #region CRUD Operations

        /// <summary>
        /// Create new expense request with receipt images
        /// Member uploads receipt when creating expense
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromForm] CreateExpenseDto request)
        {
            try
            {
                // Upload receipt images to blob storage
                var receiptUrls = new List<string>();
                if (request.ReceiptImages != null && request.ReceiptImages.Any())
                {
                    foreach (var file in request.ReceiptImages)
                    {
                        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                        var url = await _blobStorageService.UploadFileAsync(file, "campaign-expense-receipts", fileName);
                        receiptUrls.Add(url);
                    }
                }

                var expense = new FTCampaignExpense
                {
                    CampaignId = request.CampaignId,
                    ExpenseAmount = request.Amount,
                    ExpenseDescription = request.Description,
                    Category = request.Category,
                    ReceiptImages = receiptUrls.Any() ? string.Join(",", receiptUrls) : null,
                    AuthorizedBy = request.AuthorizedBy,
                    ApprovalStatus = ApprovalStatus.Pending
                };

                var result = await _expenseService.AddAsync(expense);
                
                return Ok(new ApiSuccess("Expense request created with receipts, pending approval", new
                {
                    ExpenseId = result.Id,
                    Amount = result.ExpenseAmount,
                    Description = result.ExpenseDescription,
                    Status = result.ApprovalStatus.ToString(),
                    ReceiptCount = receiptUrls.Count,
                    ReceiptUrls = receiptUrls
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Update expense request (only for Pending expenses)
        /// </summary>
        [HttpPut("{id:guid}")]

        public async Task<IActionResult> UpdateExpense(Guid id, [FromBody] UpdateExpenseDto request)
        {
            try
            {
                var expense = await _expenseService.GetByIdAsync(id);
                if (expense == null)
                    return NotFound(new ApiError("Expense not found"));

                expense.ExpenseAmount = request.Amount;
                expense.ExpenseDescription = request.Description;
                expense.Category = request.Category;
                expense.ReceiptImages = request.ReceiptImages;

                var result = await _expenseService.UpdateAsync(expense);
                return Ok(new ApiSuccess("Expense updated successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Delete expense request (only for Pending expenses)
        /// </summary>
        [HttpDelete("{id:guid}")]

        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            try
            {
                await _expenseService.DeleteAsync(id);
                return Ok(new ApiSuccess("Expense deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        #endregion

        #region Approval Workflow

        /// <summary>
        /// Approve expense request with payment proof image
        /// Manager uploads proof of payment transfer when approving
        /// </summary>
        [HttpPut("{id:guid}/approve")]
        public async Task<IActionResult> ApproveExpense(Guid id, [FromForm] ApproveExpenseDto request)
        {
            try
            {
                // Upload payment proof image
                string? paymentProofUrl = null;
                if (request.PaymentProofImage != null)
                {
                    var fileName = $"{Guid.NewGuid()}_{request.PaymentProofImage.FileName}";
                    paymentProofUrl = await _blobStorageService.UploadFileAsync(
                        request.PaymentProofImage, 
                        "campaign-expense-payment-proofs", 
                        fileName);
                }

                // Approve expense and attach payment proof
                var expense = await _expenseService.GetByIdAsync(id);
                if (expense == null)
                    return NotFound(new ApiError("Expense not found"));

                // Store payment proof URL in ApprovalNotes or create new field
                var approvalNotes = request.ApprovalNotes ?? "";
                if (!string.IsNullOrEmpty(paymentProofUrl))
                {
                    approvalNotes += $"\nPayment Proof: {paymentProofUrl}";
                }

                await _expenseService.ApproveExpenseAsync(id, request.ApproverId, approvalNotes);
                
                return Ok(new ApiSuccess("Expense approved successfully with payment proof", new
                {
                    ExpenseId = id,
                    Status = "Approved",
                    PaymentProofUrl = paymentProofUrl,
                    ApprovedBy = request.ApproverId,
                    ApprovalNotes = approvalNotes
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Reject expense request
        /// </summary>
        [HttpPut("{id:guid}/reject")]

        public async Task<IActionResult> RejectExpense(Guid id, [FromBody] RejectExpenseDto request)
        {
            try
            {
                await _expenseService.RejectExpenseAsync(id, request.ApproverId, request.RejectionReason);
                return Ok(new ApiSuccess("Expense rejected"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        #endregion
    }

    #region DTOs

    public class CreateExpenseDto
    {
        public Guid CampaignId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public ExpenseCategory Category { get; set; }
        public Guid AuthorizedBy { get; set; }
        
        /// <summary>
        /// Receipt images uploaded by member when creating expense
        /// </summary>
        public List<IFormFile>? ReceiptImages { get; set; }
    }

    public class UpdateExpenseDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public ExpenseCategory Category { get; set; }
        public string? ReceiptImages { get; set; }
    }

    public class ApproveExpenseDto
    {
        public Guid ApproverId { get; set; }
        public string? ApprovalNotes { get; set; }
        
        /// <summary>
        /// Payment proof image uploaded by manager when approving
        /// Proves that money has been transferred to member
        /// </summary>
        public IFormFile? PaymentProofImage { get; set; }
    }

    public class RejectExpenseDto
    {
        public Guid ApproverId { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
    }

    #endregion
}
