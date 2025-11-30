using FTM.Domain.Entities.Funds;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FTM.Infrastructure.Repositories
{
    public class FTFundRepository : GenericRepository<FTFund>, IFTFundRepository
    {
        public FTFundRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver)
            : base(context, currentUserResolver)
        {
        }

        public async Task<FTFund?> GetByFTIdAsync(Guid FTId)
        {
            return await Context.FTFunds
                .FirstOrDefaultAsync(f => f.FTId == FTId && f.IsDeleted == false);
        }

        public async Task<bool> ExistsByFTIdAsync(Guid FTId)
        {
            return await Context.FTFunds
                .AnyAsync(f => f.FTId == FTId && f.IsDeleted == false);
        }

        public async Task<bool> UpdateBalanceAsync(Guid fundId, decimal amount, string modifiedBy)
        {
            var fund = await Context.FTFunds.FindAsync(fundId);
            if (fund == null || fund.IsDeleted == true)
                return false;

            fund.CurrentMoney += amount;
            fund.LastModifiedOn = DateTimeOffset.Now;
            fund.LastModifiedBy = modifiedBy;

            Context.FTFunds.Update(fund);
            return await Context.SaveChangesAsync() > 0;
        }

        public async Task<FTFund?> GetWithHistoryAsync(Guid fundId, int skip = 0, int take = 20)
        {
            return await Context.FTFunds
                .Include(f => f.Donations.OrderByDescending(h => h.CreatedOn).Skip(skip).Take(take))
                .FirstOrDefaultAsync(f => f.Id == fundId && f.IsDeleted == false);
        }
    }
}
