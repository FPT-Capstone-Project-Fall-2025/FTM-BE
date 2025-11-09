using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FTM.Infrastructure.Repositories.Implement
{
    public class FTFundExpenseRepository : GenericRepository<FTFundExpense>, IFTFundExpenseRepository
    {
        private readonly FTMDbContext _context;

        public FTFundExpenseRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) : base(context, currentUserResolver)
        {
            _context = context;
        }

        public async Task<IEnumerable<FTFundExpense>> GetExpensesByFundIdAsync(Guid fundId)
        {
            return await _context.FTFundExpenses
                .Include(e => e.Approver)
                .Include(e => e.Campaign)
                .Where(e => e.FTFundId == fundId && e.IsDeleted != true)
                .OrderByDescending(e => e.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFundExpense>> GetPendingExpensesAsync(Guid fundId)
        {
            return await _context.FTFundExpenses
                .Include(e => e.Approver)
                .Include(e => e.Campaign)
                .Where(e => e.FTFundId == fundId && e.Status == TransactionStatus.Pending && e.IsDeleted != true)
                .OrderBy(e => e.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFundExpense>> GetSpendingHistoryAsync(Guid fundId)
        {
            return await _context.FTFundExpenses
                .Include(e => e.Approver)
                .Include(e => e.Campaign)
                .Where(e => e.FTFundId == fundId && e.Status == TransactionStatus.Approved && e.IsDeleted != true)
                .OrderByDescending(e => e.ApprovedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFundExpense>> GetExpensesByStatusAsync(Guid fundId, TransactionStatus status)
        {
            return await _context.FTFundExpenses
                .Include(e => e.Approver)
                .Include(e => e.Campaign)
                .Where(e => e.FTFundId == fundId && e.Status == status && e.IsDeleted != true)
                .OrderByDescending(e => e.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<FTFundExpense>> GetExpensesByCampaignIdAsync(Guid campaignId)
        {
            return await _context.FTFundExpenses
                .Include(e => e.Approver)
                .Where(e => e.CampaignId == campaignId && e.IsDeleted != true)
                .OrderByDescending(e => e.CreatedOn)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalApprovedExpensesAsync(Guid fundId)
        {
            return await _context.FTFundExpenses
                .Where(e => e.FTFundId == fundId && e.Status == TransactionStatus.Approved && e.IsDeleted != true)
                .SumAsync(e => e.ExpenseAmount);
        }

        public async Task<FTFundExpense?> GetExpenseWithDetailsAsync(Guid expenseId)
        {
            return await _context.FTFundExpenses
                .Include(e => e.Fund)
                .Include(e => e.Approver)
                .Include(e => e.Campaign)
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.IsDeleted != true);
        }
    }
}

