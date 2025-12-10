using FTM.Application.IServices;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FTM.Application.Services
{
    public class FTFundExpenseService : IFTFundExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserResolver _currentUserResolver;

        public FTFundExpenseService(
            IUnitOfWork unitOfWork,
            ICurrentUserResolver currentUserResolver)
        {
            _unitOfWork = unitOfWork;
            _currentUserResolver = currentUserResolver;
        }

        public async Task<FTFundExpense?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<FTFundExpense>().GetQuery()
                .Include(e => e.Fund)
                .Include(e => e.Approver)
                .Include(e => e.Approver)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted == false);
        }

        public async Task<PaginatedResponse<FTFundExpense>> GetExpensesByFundAsync(
            Guid fundId, int page, int pageSize, TransactionStatus? status = null)
        {
            var query = _unitOfWork.Repository<FTFundExpense>().GetQuery()
                .Where(e => e.FTFundId == fundId && e.IsDeleted == false);

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == (TransactionStatus)status.Value);
            }

            var totalCount = await query.CountAsync();

            var expenses = await query
                .Include(e => e.Fund)
                .Include(e => e.Approver)
                .Include(e => e.Approver)
                .OrderByDescending(e => e.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<FTFundExpense>
            {
                Items = expenses,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResponse<FTFundExpense>> GetPendingExpensesAsync(
            Guid fundId, int page, int pageSize)
        {
            IQueryable<FTFundExpense> query = _unitOfWork.Repository<FTFundExpense>().GetQuery()
                .Where(e => e.Status == TransactionStatus.Pending && 
                           e.IsDeleted == false &&
                           e.FTFundId == fundId);

            var totalCount = await query.CountAsync();

            var expenses = await query
                .Include(e => e.Fund)
                .Include(e => e.Approver)
                .Include(e => e.Approver)
                .OrderByDescending(e => e.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<FTFundExpense>
            {
                Items = expenses,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<FTFundExpense> CreateExpenseAsync(FTFundExpense expense)
        {
            var userId = _currentUserResolver.UserId;
            
            expense.Id = Guid.NewGuid();
            expense.IsDeleted = false;
            expense.Status = TransactionStatus.Pending;
            expense.CreatedOn = DateTimeOffset.UtcNow;
            expense.CreatedByUserId = userId;

            await _unitOfWork.Repository<FTFundExpense>().AddAsync(expense);
            await _unitOfWork.CompleteAsync();

            return expense;
        }

        public async Task<FTFundExpense> UpdateExpenseAsync(FTFundExpense expense)
        {
            var existing = await _unitOfWork.Repository<FTFundExpense>().GetByIdAsync(expense.Id);

            if (existing == null || existing.IsDeleted == true)
                throw new InvalidOperationException("Expense not found");

            if (existing.Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Can only update pending expenses");

            // Update fields
            if (expense.ExpenseAmount > 0)
                existing.ExpenseAmount = expense.ExpenseAmount;

            if (!string.IsNullOrEmpty(expense.ExpenseDescription))
                existing.ExpenseDescription = expense.ExpenseDescription;

            if (expense.ExpenseEvent != null)
                existing.ExpenseEvent = expense.ExpenseEvent;

            if (expense.ReceiptImages != null)
                existing.ReceiptImages = expense.ReceiptImages;

            _unitOfWork.Repository<FTFundExpense>().Update(existing);
            await _unitOfWork.CompleteAsync();

            return existing;
        }

        public async Task DeleteExpenseAsync(Guid id)
        {
            var expense = await _unitOfWork.Repository<FTFundExpense>().GetByIdAsync(id);

            if (expense == null || expense.IsDeleted == true)
                throw new InvalidOperationException("Expense not found");

            if (expense.Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Can only delete pending expenses");

            expense.IsDeleted = true;
            _unitOfWork.Repository<FTFundExpense>().Delete(expense);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<FTFundExpense> ApproveExpenseAsync(Guid id, Guid approverId, string? notes, string? paymentProof)
        {
            var expense = await _unitOfWork.Repository<FTFundExpense>().GetQuery()
                .Include(e => e.Fund)
                .FirstOrDefaultAsync(e => e.Id == id && e.IsDeleted == false);

            if (expense == null)
                throw new InvalidOperationException("Expense not found");

            if (expense.Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Expense is not pending");

            // Check fund balance
            if (expense.Fund != null && expense.Fund.CurrentMoney < expense.ExpenseAmount)
                throw new InvalidOperationException("Insufficient fund balance");

            // Update expense
            expense.Status = TransactionStatus.Approved;
            expense.ApprovedBy = approverId;
            expense.ApprovedOn = DateTimeOffset.UtcNow;
            expense.ApprovalFeedback = notes;
            expense.PaymentProofImage = paymentProof;

            _unitOfWork.Repository<FTFundExpense>().Update(expense);

            // Deduct from fund balance - Load fund separately to avoid tracking conflicts
            if (expense.FTFundId != Guid.Empty)
            {
                var fund = await _unitOfWork.Repository<FTFund>().GetQuery()
                    .FirstOrDefaultAsync(f => f.Id == expense.FTFundId);
                    
                if (fund != null)
                {
                    fund.CurrentMoney -= expense.ExpenseAmount;
                    _unitOfWork.Repository<FTFund>().Update(fund);
                }
            }

            await _unitOfWork.CompleteAsync();

            return expense;
        }

        public async Task<FTFundExpense> RejectExpenseAsync(Guid id, Guid rejectedBy, string reason)
        {
            var expense = await _unitOfWork.Repository<FTFundExpense>().GetByIdAsync(id);

            if (expense == null || expense.IsDeleted == true)
                throw new InvalidOperationException("Expense not found");

            if (expense.Status != TransactionStatus.Pending)
                throw new InvalidOperationException("Can only reject pending expenses");

            expense.Status = TransactionStatus.Rejected;
            expense.ApprovedBy = rejectedBy;
            expense.ApprovalFeedback = $"Rejected: {reason}";
            expense.ApprovedOn = DateTimeOffset.UtcNow;

            _unitOfWork.Repository<FTFundExpense>().Update(expense);
            await _unitOfWork.CompleteAsync();

            return expense;
        }

        public async Task<object> GetExpenseStatisticsAsync(Guid fundId)
        {
            var expenses = await _unitOfWork.Repository<FTFundExpense>().GetQuery()
                .Where(e => e.FTFundId == fundId && e.IsDeleted == false)
                .ToListAsync();

            var approvedExpenses = expenses.Where(e => e.Status == TransactionStatus.Approved).ToList();
            var pendingExpenses = expenses.Where(e => e.Status == TransactionStatus.Pending).ToList();
            var rejectedExpenses = expenses.Where(e => e.Status == TransactionStatus.Rejected).ToList();

            return new
            {
                TotalExpenses = approvedExpenses.Sum(e => e.ExpenseAmount),
                PendingExpenses = pendingExpenses.Sum(e => e.ExpenseAmount),
                RejectedExpenses = rejectedExpenses.Sum(e => e.ExpenseAmount),
                ApprovedCount = approvedExpenses.Count,
                PendingCount = pendingExpenses.Count,
                RejectedCount = rejectedExpenses.Count,
                TotalRequests = expenses.Count
            };
        }
    }
}
