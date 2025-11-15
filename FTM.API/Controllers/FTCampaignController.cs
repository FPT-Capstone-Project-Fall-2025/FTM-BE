using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FTCampaignController : ControllerBase
    {
        private readonly IFTCampaignService _campaignService;
        private readonly ILogger<FTCampaignController> _logger;

        public FTCampaignController(IFTCampaignService campaignService, ILogger<FTCampaignController> logger)
        {
            _campaignService = campaignService;
            _logger = logger;
        }

        #region CRUD Operations

        /// <summary>
        /// Get campaign by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCampaignById(Guid id)
        {
            try
            {
                var campaign = await _campaignService.GetByIdAsync(id);
                if (campaign == null)
                    return NotFound(new ApiError("Campaign not found"));

                return Ok(new ApiSuccess(campaign));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get campaigns by family tree with pagination
        /// </summary>
        [HttpGet("family-tree/{familyTreeId:guid}")]
        public async Task<IActionResult> GetCampaignsByFamilyTree(
            Guid familyTreeId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _campaignService.GetCampaignsByFamilyTreeAsync(familyTreeId, page, pageSize);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get campaigns managed by specific user
        /// </summary>
        [HttpGet("manager/{managerId:guid}")]
      
        public async Task<IActionResult> GetCampaignsByManager(
            Guid managerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _campaignService.GetCampaignsByManagerAsync(managerId, page, pageSize);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get all active campaigns
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCampaigns(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _campaignService.GetActiveCampaignsAsync(page, pageSize);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Create a new campaign
        /// </summary>
        [HttpPost]
        // [Authorize] // TODO: Uncomment for production
        public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignRequest? request)
        {
            try
            {
                // Debug logging
                _logger.LogInformation("CreateCampaign called. Request is null: {IsNull}", request == null);
                
                // Validate request
                if (request == null)
                {
                    _logger.LogWarning("Request body is null");
                    return BadRequest(new ApiError("Request body is required"));
                }

                _logger.LogInformation("Request received: FamilyTreeId={FamilyTreeId}, ManagerId={ManagerId}, CampaignName={Name}", 
                    request.FamilyTreeId, request.CampaignManagerId, request.CampaignName);

                // Validate campaign manager ID
                if (!request.CampaignManagerId.HasValue || request.CampaignManagerId == Guid.Empty)
                {
                    return BadRequest(new ApiError("Campaign manager ID is required"));
                }

                // Validate family tree ID
                if (request.FamilyTreeId == Guid.Empty)
                {
                    _logger.LogWarning("Family tree ID is empty: {FamilyTreeId}", request.FamilyTreeId);
                    return BadRequest(new ApiError("Family tree ID is required"));
                }

                // Map request to entity
                var campaign = new FTFundCampaign
                {
                    FTId = request.FamilyTreeId,
                    CampaignName = request.CampaignName ?? string.Empty,
                    CampaignDescription = request.CampaignDescription ?? string.Empty,
                    CampaignManagerId = request.CampaignManagerId.Value,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    FundGoal = request.FundGoal,
                    CurrentBalance = 0,
                    Status = CampaignStatus.Active,
                    BankAccountNumber = request.BankAccountNumber,
                    BankName = request.BankName,
                    BankCode = request.BankCode,
                    AccountHolderName = request.AccountHolderName
                };

                var result = await _campaignService.AddAsync(campaign);
                return Ok(new ApiSuccess("Campaign created successfully", result));
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " | Inner: " + ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        errorMessage += " | Details: " + ex.InnerException.InnerException.Message;
                    }
                }
                _logger?.LogError(ex, "Error creating campaign: {Error}", errorMessage);
                return BadRequest(new ApiError("Error creating campaign", errorMessage));
            }
        }

        /// <summary>
        /// Update an existing campaign
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCampaign(Guid id, [FromBody] UpdateCampaignRequest request)
        {
            try
            {
                // Get the campaign entity
                var campaignEntity = await _campaignService.GetByIdAsync(id);
                if (campaignEntity == null)
                {
                    return NotFound(new ApiError("Campaign not found"));
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(request.CampaignName))
                    campaignEntity.CampaignName = request.CampaignName;

                if (!string.IsNullOrEmpty(request.CampaignDescription))
                    campaignEntity.CampaignDescription = request.CampaignDescription;

                if (request.CampaignManagerId.HasValue)
                    campaignEntity.CampaignManagerId = request.CampaignManagerId.Value;

                if (request.StartDate.HasValue)
                    campaignEntity.StartDate = request.StartDate.Value;

                if (request.EndDate.HasValue)
                    campaignEntity.EndDate = request.EndDate.Value;

                if (request.FundGoal.HasValue)
                    campaignEntity.FundGoal = request.FundGoal.Value;

                if (request.Status.HasValue)
                    campaignEntity.Status = request.Status.Value;

                // Update bank account info
                if (request.BankAccountNumber != null)
                    campaignEntity.BankAccountNumber = request.BankAccountNumber;

                if (request.BankCode != null)
                    campaignEntity.BankCode = request.BankCode;

                if (request.BankName != null)
                    campaignEntity.BankName = request.BankName;

                if (request.AccountHolderName != null)
                    campaignEntity.AccountHolderName = request.AccountHolderName;

                if (request.Notes != null)
                    campaignEntity.Notes = request.Notes;

                var result = await _campaignService.UpdateAsync(campaignEntity);
                
                _logger.LogInformation("Updated campaign {CampaignId}", id);
                
                return Ok(new ApiSuccess("Campaign updated successfully", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating campaign {CampaignId}", id);
                return BadRequest(new ApiError("Error updating campaign", ex.Message));
            }
        }

        #endregion

        #region Campaign Statistics & Financial Info

        /// <summary>
        /// Get campaign statistics (money raised, expenses, balance, progress)
        /// </summary>
        [HttpGet("{campaignId:guid}/statistics")]
        public async Task<IActionResult> GetCampaignStatistics(Guid campaignId)
        {
            try
            {
                var campaign = await _campaignService.GetByIdAsync(campaignId);
                if (campaign == null)
                    return NotFound(new ApiError("Campaign not found"));

                // Calculate statistics
                var stats = new
                {
                    campaignId = campaign.Id,
                    campaignName = campaign.CampaignName,
                    fundGoal = campaign.FundGoal,
                    currentBalance = campaign.CurrentBalance,
                    raisedAmount = campaign.CurrentBalance, // Same as current balance
                    progressPercentage = campaign.FundGoal > 0 ? (campaign.CurrentBalance / campaign.FundGoal * 100) : 0,
                    status = campaign.Status.ToString(),
                    startDate = campaign.StartDate,
                    endDate = campaign.EndDate,
                    daysRemaining = campaign.EndDate > DateTimeOffset.UtcNow 
                        ? (campaign.EndDate - DateTimeOffset.UtcNow).Days 
                        : 0,
                    isActive = campaign.Status == CampaignStatus.Active,
                    isCompleted = campaign.Status == CampaignStatus.Completed,
                    bankInfo = new
                    {
                        bankAccountNumber = campaign.BankAccountNumber,
                        bankName = campaign.BankName,
                        bankCode = campaign.BankCode,
                        accountHolderName = campaign.AccountHolderName
                    }
                };

                return Ok(new ApiSuccess("Campaign statistics retrieved successfully", stats));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaign statistics for {CampaignId}", campaignId);
                return BadRequest(new ApiError("Error retrieving campaign statistics", ex.Message));
            }
        }

        /// <summary>
        /// Get campaign financial summary (donations count, expenses count, total amounts)
        /// </summary>
        [HttpGet("{campaignId:guid}/financial-summary")]
        public async Task<IActionResult> GetFinancialSummary(Guid campaignId)
        {
            try
            {
                var campaign = await _campaignService.GetByIdAsync(campaignId);
                if (campaign == null)
                    return NotFound(new ApiError("Campaign not found"));

                var summary = await _campaignService.GetCampaignFinancialSummaryAsync(campaignId);
                
                return Ok(new ApiSuccess("Financial summary retrieved successfully", summary));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting financial summary for {CampaignId}", campaignId);
                return BadRequest(new ApiError("Error retrieving financial summary", ex.Message));
            }
        }

        #endregion

        #region Campaign Donations & Expenses (delegated to specific controllers)

        /// <summary>
        /// Get donations for a campaign (paginated)
        /// Use /api/FTCampaignDonation/campaign/{campaignId} instead
        /// </summary>
        [HttpGet("{campaignId:guid}/donations")]
        public IActionResult GetCampaignDonations(Guid campaignId)
        {
            return Ok(new ApiSuccess("Use /api/FTCampaignDonation/campaign/{campaignId} endpoint"));
        }

        /// <summary>
        /// Get expenses for a campaign (paginated)
        /// Use /api/FTCampaignExpense/campaign/{campaignId} instead
        /// </summary>
        [HttpGet("{campaignId:guid}/expenses")]
        public IActionResult GetCampaignExpenses(Guid campaignId)
        {
            return Ok(new ApiSuccess("Use /api/FTCampaignExpense/campaign/{campaignId} endpoint"));
        }

        #endregion
    }
}
