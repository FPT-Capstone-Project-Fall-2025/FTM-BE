using FTM.Application.IServices;
using FTM.Domain.Entities.Funds;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FTM.Application.Services
{
    public class FTFundService : IFTFundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserResolver _currentUserResolver;

        public FTFundService(
            IUnitOfWork unitOfWork,
            ICurrentUserResolver currentUserResolver)
        {
            _unitOfWork = unitOfWork;
            _currentUserResolver = currentUserResolver;
        }

        public async Task<List<FTFund>> GetFundsByTreeAsync(Guid treeId)
        {
            return await _unitOfWork.Repository<FTFund>().GetQuery()
                .Where(f => f.FTId == treeId && f.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<FTFund> CreateFundAsync(FTFund fund)
        {
            fund.Id = Guid.NewGuid();
            fund.IsDeleted = false;
            fund.CurrentMoney = 0;

            await _unitOfWork.Repository<FTFund>().AddAsync(fund);
            await _unitOfWork.CompleteAsync();

            return fund;
        }

        public async Task<FTFund> UpdateFundAsync(Guid fundId, FTFund fund)
        {
            var existing = await _unitOfWork.Repository<FTFund>().GetQuery()
                .FirstOrDefaultAsync(f => f.Id == fundId && f.IsDeleted == false);

            if (existing == null)
                throw new InvalidOperationException("Fund not found");

            // Update fields
            if (!string.IsNullOrEmpty(fund.FundName))
                existing.FundName = fund.FundName;

            if (fund.FundNote != null)
                existing.FundNote = fund.FundNote;

            if (fund.BankAccountNumber != null)
                existing.BankAccountNumber = fund.BankAccountNumber;

            if (fund.BankCode != null)
                existing.BankCode = fund.BankCode;

            if (fund.BankName != null)
                existing.BankName = fund.BankName;

            if (fund.AccountHolderName != null)
                existing.AccountHolderName = fund.AccountHolderName;

            if (fund.FundManagers != null)
                existing.FundManagers = fund.FundManagers;

            _unitOfWork.Repository<FTFund>().Update(existing);
            await _unitOfWork.CompleteAsync();

            return existing;
        }

        public async Task<FTFund?> GetByIdAsync(Guid fundId)
        {
            return await _unitOfWork.Repository<FTFund>().GetQuery()
                .FirstOrDefaultAsync(f => f.Id == fundId && f.IsDeleted == false);
        }
    }
}
