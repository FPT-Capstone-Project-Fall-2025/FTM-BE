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

        public FTCampaignExpenseController(IFTCampaignExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        #region Query Operations

        /// <summary>
        /// Get expense by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        /// Create new expense request
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseDto request)
        {
            try
            {
                var expense = new FTCampaignExpense
                {
                    CampaignId = request.CampaignId,
                    ExpenseAmount = request.Amount,
                    ExpenseDescription = request.Description,
                    Category = request.Category,
                    ReceiptImages = request.ReceiptImages,
                    AuthorizedBy = request.AuthorizedBy,
                    ApprovalStatus = ApprovalStatus.Pending
                };

                var result = await _expenseService.AddAsync(expense);
                return Ok(new ApiSuccess("Expense request created, pending approval", result));
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
        [Authorize]
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
        [Authorize]
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
        /// Approve expense request
        /// </summary>
        [HttpPut("{id:guid}/approve")]
        [Authorize]
        public async Task<IActionResult> ApproveExpense(Guid id, [FromBody] ApproveExpenseDto request)
        {
            try
            {
                await _expenseService.ApproveExpenseAsync(id, request.ApproverId, request.ApprovalNotes);
                return Ok(new ApiSuccess("Expense approved successfully"));
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
        [Authorize]
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

    #region DTOs (if not already defined)

    public class CreateExpenseDto
    {
        public Guid CampaignId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public ExpenseCategory Category { get; set; }
        public string? ReceiptImages { get; set; }
        public Guid AuthorizedBy { get; set; }
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
    }

    public class RejectExpenseDto
    {
        public Guid ApproverId { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
    }

    #endregion
}
