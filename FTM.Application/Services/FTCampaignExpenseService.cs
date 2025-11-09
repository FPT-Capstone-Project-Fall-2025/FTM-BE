using AutoMapper;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.Application.Services
{
    public class FTCampaignExpenseService : IFTCampaignExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserResolver _currentUserResolver;
        private readonly IFTCampaignService _campaignService;

        public FTCampaignExpenseService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserResolver currentUserResolver,
            IFTCampaignService campaignService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserResolver = currentUserResolver;
            _campaignService = campaignService;
        }

        public async Task<FTCampaignExpense?> GetByIdAsync(Guid id)
        {
            var expense = await _unitOfWork.Repository<FTCampaignExpense>().GetByIdAsync(id);
            if (expense == null || expense.IsDeleted == true)
                return null;
            return expense;
        }

        public async Task<FTCampaignExpense> AddAsync(FTCampaignExpense expense)
        {
            // Validate campaign exists
            var campaign = await _campaignService.GetByIdAsync(expense.CampaignId);
            if (campaign == null)
                throw new InvalidOperationException("Campaign not found");

            expense.Id = Guid.NewGuid();
            expense.ApprovalStatus = ApprovalStatus.Pending;
            expense.CreatedOn = DateTimeOffset.UtcNow;
            expense.CreatedBy = _currentUserResolver.Username ?? "System";
            expense.LastModifiedOn = DateTimeOffset.UtcNow;
            expense.LastModifiedBy = _currentUserResolver.Username ?? "System";

            await _unitOfWork.Repository<FTCampaignExpense>().AddAsync(expense);
            await _unitOfWork.CompleteAsync();
            return expense;
        }

        public async Task<FTCampaignExpense> UpdateAsync(FTCampaignExpense expense)
        {
            var existing = await GetByIdAsync(expense.Id);
            if (existing == null)
                throw new InvalidOperationException("Expense not found");

            if (existing.ApprovalStatus != ApprovalStatus.Pending)
                throw new InvalidOperationException("Can only update pending expenses");

            expense.LastModifiedOn = DateTimeOffset.UtcNow;
            expense.LastModifiedBy = _currentUserResolver.Username ?? "System";

            _unitOfWork.Repository<FTCampaignExpense>().Update(expense);
            await _unitOfWork.CompleteAsync();
            return expense;
        }

        public async Task DeleteAsync(Guid id)
        {
            var expense = await GetByIdAsync(id);
            if (expense == null)
                throw new InvalidOperationException("Expense not found");

            if (expense.ApprovalStatus == ApprovalStatus.Approved)
                throw new InvalidOperationException("Cannot delete approved expenses");

            expense.IsDeleted = true;
            expense.LastModifiedOn = DateTimeOffset.UtcNow;
            expense.LastModifiedBy = _currentUserResolver.Username ?? "System";

            _unitOfWork.Repository<FTCampaignExpense>().Update(expense);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<PaginatedResponse<FTCampaignExpenseResponseDto>> GetCampaignExpensesAsync(
            Guid campaignId, ApprovalStatus? status, int page, int pageSize)
        {
            var query = _unitOfWork.Repository<FTCampaignExpense>().GetQuery();
            
            var filteredQuery = query.Where(e => e.CampaignId == campaignId && e.IsDeleted != true);
            
            if (status.HasValue)
            {
                filteredQuery = filteredQuery.Where(e => e.ApprovalStatus == status.Value);
            }

            var totalCount = await filteredQuery.CountAsync();

            var expenses = await filteredQuery
                .OrderByDescending(e => e.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var expenseDtos = expenses.Select(e => _mapper.Map<FTCampaignExpenseResponseDto>(e)).ToList();

            return new PaginatedResponse<FTCampaignExpenseResponseDto>
            {
                Items = expenseDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResponse<FTCampaignExpenseResponseDto>> GetUserExpensesAsync(
            Guid userId, int page, int pageSize)
        {
            var query = _unitOfWork.Repository<FTCampaignExpense>().GetQuery();
            
            var totalCount = await query
                .Where(e => e.AuthorizedBy == userId && e.IsDeleted != true)
                .CountAsync();

            var expenses = await query
                .Where(e => e.AuthorizedBy == userId && e.IsDeleted != true)
                .OrderByDescending(e => e.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var expenseDtos = expenses.Select(e => _mapper.Map<FTCampaignExpenseResponseDto>(e)).ToList();

            return new PaginatedResponse<FTCampaignExpenseResponseDto>
            {
                Items = expenseDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResponse<FTCampaignExpenseResponseDto>> GetPendingExpensesForManagerAsync(
            Guid managerId, int page, int pageSize)
        {
            // First, get campaigns managed by this user
            var campaignQuery = _unitOfWork.Repository<FTFundCampaign>().GetQuery();
            var campaignIds = await campaignQuery
                .Where(c => c.CampaignManagerId == managerId && c.IsDeleted != true)
                .Select(c => c.Id)
                .ToListAsync();

            // Then get pending expenses for those campaigns
            var expenseQuery = _unitOfWork.Repository<FTCampaignExpense>().GetQuery();
            
            var totalCount = await expenseQuery
                .Where(e => campaignIds.Contains(e.CampaignId) && 
                           e.ApprovalStatus == ApprovalStatus.Pending && 
                           e.IsDeleted != true)
                .CountAsync();

            var expenses = await expenseQuery
                .Where(e => campaignIds.Contains(e.CampaignId) && 
                           e.ApprovalStatus == ApprovalStatus.Pending && 
                           e.IsDeleted != true)
                .OrderByDescending(e => e.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var expenseDtos = expenses.Select(e => _mapper.Map<FTCampaignExpenseResponseDto>(e)).ToList();

            return new PaginatedResponse<FTCampaignExpenseResponseDto>
            {
                Items = expenseDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ExpenseStatisticsDto> GetExpenseStatisticsAsync(Guid campaignId)
        {
            var query = _unitOfWork.Repository<FTCampaignExpense>().GetQuery();
            var expenses = await query
                .Where(e => e.CampaignId == campaignId && e.IsDeleted != true)
                .ToListAsync();

            var approvedExpenses = expenses.Where(e => e.ApprovalStatus == ApprovalStatus.Approved).ToList();
            var pendingExpenses = expenses.Where(e => e.ApprovalStatus == ApprovalStatus.Pending).ToList();
            var rejectedExpenses = expenses.Where(e => e.ApprovalStatus == ApprovalStatus.Rejected).ToList();

            var expensesByCategory = approvedExpenses
                .GroupBy(e => e.Category)
                .ToDictionary(g => g.Key, g => g.Sum(e => e.ExpenseAmount));

            return new ExpenseStatisticsDto
            {
                CampaignId = campaignId,
                TotalExpenses = approvedExpenses.Sum(e => e.ExpenseAmount),
                ApprovedExpenses = approvedExpenses.Sum(e => e.ExpenseAmount),
                PendingExpenses = pendingExpenses.Sum(e => e.ExpenseAmount),
                RejectedExpenses = rejectedExpenses.Sum(e => e.ExpenseAmount),
                TotalExpenseRequests = expenses.Count,
                ApprovedRequests = approvedExpenses.Count,
                PendingRequests = pendingExpenses.Count,
                RejectedRequests = rejectedExpenses.Count,
                ExpensesByCategory = expensesByCategory
            };
        }

        public async Task<FTCampaignExpense> ApproveExpenseAsync(Guid expenseId, Guid approverId, string? notes)
        {
            var expense = await GetByIdAsync(expenseId);
            if (expense == null)
                throw new InvalidOperationException("Expense not found");

            if (expense.ApprovalStatus != ApprovalStatus.Pending)
                throw new InvalidOperationException("Can only approve pending expenses");

            // Check campaign has sufficient balance
            var campaign = await _campaignService.GetByIdAsync(expense.CampaignId);
            if (campaign == null)
                throw new InvalidOperationException("Campaign not found");

            if (campaign.CurrentBalance < expense.ExpenseAmount)
                throw new InvalidOperationException("Insufficient campaign balance");

            expense.ApprovalStatus = ApprovalStatus.Approved;
            expense.ApprovedBy = approverId;
            expense.ApprovedOn = DateTimeOffset.UtcNow;
            expense.ApprovalNotes = notes;
            expense.LastModifiedOn = DateTimeOffset.UtcNow;
            expense.LastModifiedBy = _currentUserResolver.Username ?? "System";

            _unitOfWork.Repository<FTCampaignExpense>().Update(expense);

            // Deduct from campaign balance
            await _campaignService.UpdateCampaignAmountAsync(expense.CampaignId, -expense.ExpenseAmount);

            await _unitOfWork.CompleteAsync();
            return expense;
        }

        public async Task<FTCampaignExpense> RejectExpenseAsync(Guid expenseId, Guid approverId, string reason)
        {
            var expense = await GetByIdAsync(expenseId);
            if (expense == null)
                throw new InvalidOperationException("Expense not found");

            if (expense.ApprovalStatus != ApprovalStatus.Pending)
                throw new InvalidOperationException("Can only reject pending expenses");

            expense.ApprovalStatus = ApprovalStatus.Rejected;
            expense.ApprovedBy = approverId;
            expense.ApprovedOn = DateTimeOffset.UtcNow;
            expense.ApprovalNotes = $"Rejected: {reason}";
            expense.LastModifiedOn = DateTimeOffset.UtcNow;
            expense.LastModifiedBy = _currentUserResolver.Username ?? "System";

            _unitOfWork.Repository<FTCampaignExpense>().Update(expense);
            await _unitOfWork.CompleteAsync();
            return expense;
        }
    }
}
