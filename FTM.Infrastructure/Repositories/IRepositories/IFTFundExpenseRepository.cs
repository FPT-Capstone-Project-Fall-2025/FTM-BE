using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Infrastructure.Repositories.Interface;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IFTFundExpenseRepository : IGenericRepository<FTFundExpense>
    {
        /// <summary>
        /// Get all expenses for a specific fund
        /// </summary>
        Task<IEnumerable<FTFundExpense>> GetExpensesByFundIdAsync(Guid fundId);

        /// <summary>
        /// Get pending withdrawal requests requiring approval
        /// </summary>
        Task<IEnumerable<FTFundExpense>> GetPendingExpensesAsync(Guid fundId);

        /// <summary>
        /// Get spending history for a fund (approved expenses)
        /// </summary>
        Task<IEnumerable<FTFundExpense>> GetSpendingHistoryAsync(Guid fundId);

        /// <summary>
        /// Get expenses by status
        /// </summary>
        Task<IEnumerable<FTFundExpense>> GetExpensesByStatusAsync(Guid fundId, TransactionStatus status);

        /// <summary>
        /// Get expenses for a specific campaign
        /// </summary>
        Task<IEnumerable<FTFundExpense>> GetExpensesByCampaignIdAsync(Guid campaignId);

        /// <summary>
        /// Get total approved expenses for a fund
        /// </summary>
        Task<decimal> GetTotalApprovedExpensesAsync(Guid fundId);

        /// <summary>
        /// Get expense with full details (includes approver, campaign)
        /// </summary>
        Task<FTFundExpense?> GetExpenseWithDetailsAsync(Guid expenseId);
    }
}