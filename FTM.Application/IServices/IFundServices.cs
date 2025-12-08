using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Domain.Helpers;

namespace FTM.Application.IServices
{
    public interface IFTFundService
    {
        /// <summary>
        /// Get all funds for a family tree
        /// </summary>
        Task<List<FTFund>> GetFundsByTreeAsync(Guid treeId);

        /// <summary>
        /// Create a new fund
        /// </summary>
        Task<FTFund> CreateFundAsync(FTFund fund);

        /// <summary>
        /// Update fund information
        /// </summary>
        Task<FTFund> UpdateFundAsync(Guid fundId, FTFund fund);

        /// <summary>
        /// Get fund by ID
        /// </summary>
        Task<FTFund?> GetByIdAsync(Guid fundId);
    }

    public interface IFTFundDonationService
    {
        /// <summary>
        /// Get donation by ID
        /// </summary>
        Task<FTFundDonation?> GetByIdAsync(Guid id);

        /// <summary>
        /// Get donations by fund with pagination and optional status filter
        /// </summary>
        Task<PaginatedResponse<FTFundDonation>> GetDonationsByFundAsync(
            Guid fundId, int page, int pageSize, DonationStatus? status = null);

        /// <summary>
        /// Get pending donations with pagination (optional fund filter)
        /// </summary>
        Task<PaginatedResponse<FTFundDonation>> GetPendingDonationsAsync(
            Guid? fundId, int page, int pageSize);

        /// <summary>
        /// Get user's pending donations (for uploading proof)
        /// </summary>
        Task<List<FTFundDonation>> GetUserPendingDonationsAsync(Guid userId);

        /// <summary>
        /// Create a new donation
        /// </summary>
        Task<FTFundDonation> CreateDonationAsync(FTFundDonation donation);

        /// <summary>
        /// Update donation (before confirmation)
        /// </summary>
        Task<FTFundDonation> UpdateDonationAsync(FTFundDonation donation);

        /// <summary>
        /// Soft delete donation (only pending)
        /// </summary>
        Task DeleteDonationAsync(Guid id);

        /// <summary>
        /// Confirm donation and update fund balance
        /// </summary>
        Task<FTFundDonation> ConfirmDonationAsync(Guid id, Guid confirmerId, string? notes);

        /// <summary>
        /// Reject donation
        /// </summary>
        Task<FTFundDonation> RejectDonationAsync(Guid id, Guid rejectedBy, string reason);

        /// <summary>
        /// Get donation statistics for a fund
        /// </summary>
        Task<object> GetDonationStatisticsAsync(Guid fundId);
    }

    public interface IFTFundExpenseService
    {
        /// <summary>
        /// Get expense by ID
        /// </summary>
        Task<FTFundExpense?> GetByIdAsync(Guid id);

        /// <summary>
        /// Get expenses by fund with pagination and optional status filter
        /// </summary>
        Task<PaginatedResponse<FTFundExpense>> GetExpensesByFundAsync(
            Guid fundId, int page, int pageSize, TransactionStatus? status = null);

        /// <summary>
        /// Get pending expenses with pagination (optional fund filter)
        /// </summary>
        Task<PaginatedResponse<FTFundExpense>> GetPendingExpensesAsync(
            Guid? fundId, int page, int pageSize);

        /// <summary>
        /// Create a new expense
        /// </summary>
        Task<FTFundExpense> CreateExpenseAsync(FTFundExpense expense);

        /// <summary>
        /// Update expense (only pending)
        /// </summary>
        Task<FTFundExpense> UpdateExpenseAsync(FTFundExpense expense);

        /// <summary>
        /// Soft delete expense (only pending)
        /// </summary>
        Task DeleteExpenseAsync(Guid id);

        /// <summary>
        /// Approve expense and deduct from fund balance
        /// </summary>
        Task<FTFundExpense> ApproveExpenseAsync(Guid id, Guid approverId, string? notes, string? paymentProof);

        /// <summary>
        /// Reject expense
        /// </summary>
        Task<FTFundExpense> RejectExpenseAsync(Guid id, Guid rejectedBy, string reason);

        /// <summary>
        /// Get expense statistics for a fund
        /// </summary>
        Task<object> GetExpenseStatisticsAsync(Guid fundId);
    }
}
