using FTM.Domain.Entities.Funds;
using FTM.Infrastructure.Repositories.Interface;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IFTFundRepository : IGenericRepository<FTFund>
    {
        /// <summary>
        /// Get fund by family tree ID
        /// </summary>
        Task<FTFund?> GetByFTIdAsync(Guid FTId);

        /// <summary>
        /// Check if fund exists for a family tree
        /// </summary>
        Task<bool> ExistsByFTIdAsync(Guid FTId);

        /// <summary>
        /// Update fund balance (thread-safe)
        /// </summary>
        Task<bool> UpdateBalanceAsync(Guid fundId, decimal amount, string modifiedBy);

        /// <summary>
        /// Get fund with transaction history
        /// </summary>
        Task<FTFund?> GetWithHistoryAsync(Guid fundId, int skip = 0, int take = 20);
    }
}
