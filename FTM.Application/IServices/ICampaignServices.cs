using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using FTM.Domain.Interface;
using FTM.Domain.Specification;

namespace FTM.Application.IServices
{
    public interface IFTCampaignService
    {
        /// <summary>
        /// Get campaign by ID
        /// </summary>
        Task<FTFundCampaign?> GetByIdAsync(Guid id);

        /// <summary>
        /// Add new campaign
        /// </summary>
        Task<FTFundCampaign> AddAsync(FTFundCampaign campaign);

        /// <summary>
        /// Update campaign
        /// </summary>
        Task<FTFundCampaign> UpdateAsync(FTFundCampaign campaign);

        /// <summary>
        /// Get campaigns by family tree with pagination
        /// </summary>
        /// <param name="familyTreeId">Family tree ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated campaigns</returns>
        Task<PaginatedResponse<FTCampaignResponseDto>> GetCampaignsByFamilyTreeAsync(
            Guid familyTreeId, int page, int pageSize);

        /// <summary>
        /// Get campaigns managed by a specific user
        /// </summary>
        /// <param name="managerId">Manager user ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated campaigns</returns>
        Task<PaginatedResponse<FTCampaignResponseDto>> GetCampaignsByManagerAsync(
            Guid managerId, int page, int pageSize);

        /// <summary>
        /// Get active campaigns
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated active campaigns</returns>
        Task<PaginatedResponse<FTCampaignResponseDto>> GetActiveCampaignsAsync(
            int page, int pageSize);

        /// <summary>
        /// Get campaign donations with pagination
        /// </summary>
        /// <param name="specification">Donation specification</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated donations</returns>
        Task<PaginatedResponse<FTCampaignDonationResponseDto>> GetCampaignDonationsAsync(
            ISpecification<FTCampaignDonation> specification, int page, int pageSize);

        /// <summary>
        /// Get campaign expenses with pagination
        /// </summary>
        /// <param name="specification">Expense specification</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated expenses</returns>
        Task<PaginatedResponse<FTCampaignExpenseResponseDto>> GetCampaignExpensesAsync(
            ISpecification<FTCampaignExpense> specification, int page, int pageSize);

        /// <summary>
        /// Get financial summary for a campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <returns>Financial summary</returns>
        Task<CampaignFinancialSummaryDto> GetCampaignFinancialSummaryAsync(Guid campaignId);

        /// <summary>
        /// Update campaign current amount after donation
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="amount">Donation amount to add</param>
        /// <returns>Updated campaign</returns>
        Task<FTFundCampaign> UpdateCampaignAmountAsync(Guid campaignId, decimal amount);
    }

    public interface IFTCampaignDonationService
    {
        /// <summary>
        /// Get donation by ID
        /// </summary>
        Task<FTCampaignDonation?> GetByIdAsync(Guid id);

        /// <summary>
        /// Add new donation
        /// </summary>
        Task<FTCampaignDonation> AddAsync(FTCampaignDonation donation);

        /// <summary>
        /// Update donation
        /// </summary>
        Task<FTCampaignDonation> UpdateAsync(FTCampaignDonation donation);

        /// <summary>
        /// Get donation by PayOS order code
        /// </summary>
        /// <param name="orderCode">PayOS order code</param>
        /// <returns>Campaign donation</returns>
        Task<FTCampaignDonation?> GetByOrderCodeAsync(string orderCode);

        /// <summary>
        /// Get donations for a specific campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated donations</returns>
        Task<PaginatedResponse<FTCampaignDonationResponseDto>> GetCampaignDonationsAsync(
            Guid campaignId, int page, int pageSize);

        /// <summary>
        /// Get user's donation history
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated user donations</returns>
        Task<PaginatedResponse<FTCampaignDonationResponseDto>> GetUserDonationsAsync(
            Guid userId, int page, int pageSize);

        /// <summary>
        /// Get top donors for a campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="limit">Number of top donors</param>
        /// <returns>Top donors list</returns>
        Task<List<TopDonorDto>> GetTopDonorsAsync(Guid campaignId, int limit);

        /// <summary>
        /// Get donation statistics for a campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <returns>Donation statistics</returns>
        Task<DonationStatisticsDto> GetDonationStatisticsAsync(Guid campaignId);

        /// <summary>
        /// Process completed donation and update campaign amount
        /// </summary>
        /// <param name="orderCode">PayOS order code</param>
        /// <returns>Updated donation</returns>
        Task<FTCampaignDonation> ProcessCompletedDonationAsync(string orderCode);

        /// <summary>
        /// Get campaign with bank account info for donation
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <returns>Campaign with bank info</returns>
        Task<FTFundCampaign?> GetCampaignForDonationAsync(Guid campaignId);

        /// <summary>
        /// Get pending donations for a specific campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <returns>List of pending donations</returns>
        Task<List<FTCampaignDonationResponseDto>> GetPendingDonationsByCampaignAsync(Guid campaignId);

        /// <summary>
        /// Get all pending donations across all campaigns (optional filter by family tree)
        /// </summary>
        /// <param name="familyTreeId">Optional family tree ID filter</param>
        /// <returns>List of pending donations</returns>
        Task<List<FTCampaignDonationResponseDto>> GetAllPendingDonationsAsync(Guid? familyTreeId);

        /// <summary>
        /// Get user's pending donations (for uploading proof images)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user's pending donations</returns>
        Task<List<FTCampaignDonationResponseDto>> GetUserPendingDonationsAsync(Guid userId);
    }

    public interface IFTCampaignExpenseService
    {
        /// <summary>
        /// Get expense by ID
        /// </summary>
        Task<FTCampaignExpense?> GetByIdAsync(Guid id);

        /// <summary>
        /// Add new expense
        /// </summary>
        Task<FTCampaignExpense> AddAsync(FTCampaignExpense expense);

        /// <summary>
        /// Update expense
        /// </summary>
        Task<FTCampaignExpense> UpdateAsync(FTCampaignExpense expense);

        /// <summary>
        /// Delete expense
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Get expenses for a campaign with optional status filter
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <param name="status">Approval status filter</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated expenses</returns>
        Task<PaginatedResponse<FTCampaignExpenseResponseDto>> GetCampaignExpensesAsync(
            Guid campaignId, ApprovalStatus? status, int page, int pageSize);

        /// <summary>
        /// Get expenses requested by a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated user expenses</returns>
        Task<PaginatedResponse<FTCampaignExpenseResponseDto>> GetUserExpensesAsync(
            Guid userId, int page, int pageSize);

        /// <summary>
        /// Get pending expenses for campaigns managed by user
        /// </summary>
        /// <param name="managerId">Manager user ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated pending expenses</returns>
        Task<PaginatedResponse<FTCampaignExpenseResponseDto>> GetPendingExpensesForManagerAsync(
            Guid managerId, int page, int pageSize);

        /// <summary>
        /// Get expense statistics for a campaign
        /// </summary>
        /// <param name="campaignId">Campaign ID</param>
        /// <returns>Expense statistics</returns>
        Task<ExpenseStatisticsDto> GetExpenseStatisticsAsync(Guid campaignId);

        /// <summary>
        /// Approve an expense
        /// </summary>
        /// <param name="expenseId">Expense ID</param>
        /// <param name="approverId">Approver user ID</param>
        /// <param name="notes">Approval notes</param>
        /// <returns>Updated expense</returns>
        Task<FTCampaignExpense> ApproveExpenseAsync(Guid expenseId, Guid approverId, string? notes);

        /// <summary>
        /// Reject an expense
        /// </summary>
        /// <param name="expenseId">Expense ID</param>
        /// <param name="approverId">Approver user ID</param>
        /// <param name="reason">Rejection reason</param>
        /// <returns>Updated expense</returns>
        Task<FTCampaignExpense> RejectExpenseAsync(Guid expenseId, Guid approverId, string reason);
    }

    /// <summary>
    /// DTO for donation statistics
    /// </summary>
    public class DonationStatisticsDto
    {
        public Guid CampaignId { get; set; }
        public decimal TotalDonations { get; set; }
        public int TotalDonors { get; set; }
        public int UniqueDonors { get; set; }
        public decimal AverageDonation { get; set; }
        public decimal HighestDonation { get; set; }
        public decimal LowestDonation { get; set; }
        public DateTime? FirstDonationDate { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public Dictionary<DonationStatus, int> DonationsByStatus { get; set; } = new();
        public List<MonthlyDonationDto> MonthlyTrend { get; set; } = new();
    }

    /// <summary>
    /// DTO for monthly donation trend
    /// </summary>
    public class MonthlyDonationDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }
}