using AutoMapper;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using FTM.Domain.Interface;
using FTM.Domain.Specification;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.Application.Services
{
    public class FTCampaignService : IFTCampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserResolver _currentUserResolver;

        public FTCampaignService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserResolver currentUserResolver)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserResolver = currentUserResolver;
        }

        public async Task<FTFundCampaign?> GetByIdAsync(Guid id)
        {
            var campaign = await _unitOfWork.Repository<FTFundCampaign>().GetByIdAsync(id);
            if (campaign == null || campaign.IsDeleted == true)
                return null;
            return campaign;
        }

        public async Task<FTFundCampaign> AddAsync(FTFundCampaign campaign)
        {
            campaign.Id = Guid.NewGuid();
            
            // Auto-set status based on start date
            var now = DateTimeOffset.UtcNow;
            if (campaign.StartDate <= now && campaign.EndDate > now)
            {
                campaign.Status = CampaignStatus.Active;
            }
            else if (campaign.StartDate > now)
            {
                campaign.Status = CampaignStatus.Upcoming;
            }
            else if (campaign.EndDate <= now)
            {
                campaign.Status = CampaignStatus.Completed;
            }
            
            campaign.CurrentBalance = 0;
            campaign.CreatedOn = DateTimeOffset.UtcNow;
            campaign.CreatedBy = _currentUserResolver?.Username ?? "System";
            campaign.LastModifiedOn = DateTimeOffset.UtcNow;
            campaign.LastModifiedBy = _currentUserResolver?.Username ?? "System";

            await _unitOfWork.Repository<FTFundCampaign>().AddAsync(campaign);
            await _unitOfWork.CompleteAsync();
            return campaign;
        }

        public async Task<FTFundCampaign> UpdateAsync(FTFundCampaign campaign)
        {
            var existing = await GetByIdAsync(campaign.Id);
            if (existing == null)
                throw new InvalidOperationException("Campaign not found");

            if (existing.Status == CampaignStatus.Completed || existing.Status == CampaignStatus.Canceled)
                throw new InvalidOperationException($"Cannot update {existing.Status} campaigns");

            campaign.LastModifiedOn = DateTimeOffset.UtcNow;
            campaign.LastModifiedBy = _currentUserResolver?.Username ?? "System";

            _unitOfWork.Repository<FTFundCampaign>().Update(campaign);
            await _unitOfWork.CompleteAsync();
            return campaign;
        }

        public async Task<PaginatedResponse<FTCampaignResponseDto>> GetCampaignsByFamilyTreeAsync(
            Guid familyTreeId, int page, int pageSize)
        {
            var spec = new CampaignsByFamilyTreeSpec(familyTreeId, page, pageSize);
            var campaigns = await _unitOfWork.Repository<FTFundCampaign>().ListAsync(spec);
            var totalCount = await _unitOfWork.Repository<FTFundCampaign>().CountAsync(spec);

            var campaignDtos = campaigns.Select(c => _mapper.Map<FTCampaignResponseDto>(c)).ToList();

            return new PaginatedResponse<FTCampaignResponseDto>
            {
                Items = campaignDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResponse<FTCampaignResponseDto>> GetCampaignsByManagerAsync(
            Guid managerId, int page, int pageSize)
        {
            var spec = new CampaignsByManagerSpec(managerId, page, pageSize);
            var campaigns = await _unitOfWork.Repository<FTFundCampaign>().ListAsync(spec);
            var totalCount = await _unitOfWork.Repository<FTFundCampaign>().CountAsync(spec);

            var campaignDtos = campaigns.Select(c => _mapper.Map<FTCampaignResponseDto>(c)).ToList();

            return new PaginatedResponse<FTCampaignResponseDto>
            {
                Items = campaignDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResponse<FTCampaignResponseDto>> GetActiveCampaignsAsync(int page, int pageSize)
        {
            var spec = new ActiveCampaignsSpec(page, pageSize);
            var campaigns = await _unitOfWork.Repository<FTFundCampaign>().ListAsync(spec);
            var totalCount = await _unitOfWork.Repository<FTFundCampaign>().CountAsync(spec);

            var campaignDtos = campaigns.Select(c => _mapper.Map<FTCampaignResponseDto>(c)).ToList();

            return new PaginatedResponse<FTCampaignResponseDto>
            {
                Items = campaignDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResponse<FTCampaignDonationResponseDto>> GetCampaignDonationsAsync(
            ISpecification<FTCampaignDonation> specification, int page, int pageSize)
        {
            var donations = await _unitOfWork.Repository<FTCampaignDonation>().ListAsync(specification);
            var totalCount = await _unitOfWork.Repository<FTCampaignDonation>().CountAsync(specification);

            var donationDtos = donations.Select(d => _mapper.Map<FTCampaignDonationResponseDto>(d)).ToList();

            return new PaginatedResponse<FTCampaignDonationResponseDto>
            {
                Items = donationDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResponse<FTCampaignExpenseResponseDto>> GetCampaignExpensesAsync(
            ISpecification<FTCampaignExpense> specification, int page, int pageSize)
        {
            var expenses = await _unitOfWork.Repository<FTCampaignExpense>().ListAsync(specification);
            var totalCount = await _unitOfWork.Repository<FTCampaignExpense>().CountAsync(specification);

            var expenseDtos = expenses.Select(e => _mapper.Map<FTCampaignExpenseResponseDto>(e)).ToList();

            return new PaginatedResponse<FTCampaignExpenseResponseDto>
            {
                Items = expenseDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<CampaignFinancialSummaryDto> GetCampaignFinancialSummaryAsync(Guid campaignId)
        {
            var campaign = await GetByIdAsync(campaignId);
            if (campaign == null)
                throw new InvalidOperationException("Campaign not found");

            // Get all donations using query directly
            var donationQuery = _unitOfWork.Repository<FTCampaignDonation>().GetQuery();
            var donations = await donationQuery
                .Where(d => d.CampaignId == campaignId && d.IsDeleted != true)
                .ToListAsync();

            // Get all expenses using query directly
            var expenseQuery = _unitOfWork.Repository<FTCampaignExpense>().GetQuery();
            var expenses = await expenseQuery
                .Where(e => e.CampaignId == campaignId && e.IsDeleted != true)
                .ToListAsync();

            var totalDonations = donations
                .Where(d => d.Status== DonationStatus.Completed)
                .Sum(d => d.DonationAmount);

            var totalExpenses = expenses
                .Where(e => e.ApprovalStatus == ApprovalStatus.Approved)
                .Sum(e => e.ExpenseAmount);

            return new CampaignFinancialSummaryDto
            {
                CampaignId = campaignId,
                CampaignName = campaign.CampaignName,
                TargetAmount = campaign.FundGoal,
                AvailableBalance = campaign.CurrentBalance,
                TotalDonations = totalDonations,
                TotalExpenses = totalExpenses,
                TotalDonors = donations.Count(d => d.Status == DonationStatus.Completed),
                TotalExpenseRequests = expenses.Count(e => e.ApprovalStatus == ApprovalStatus.Approved)
            };
        }

        public async Task<FTFundCampaign> UpdateCampaignAmountAsync(Guid campaignId, decimal amount)
        {
            var campaign = await GetByIdAsync(campaignId);
            if (campaign == null)
                throw new InvalidOperationException("Campaign not found");

            campaign.CurrentBalance += amount;
            campaign.LastModifiedOn = DateTimeOffset.UtcNow;
            campaign.LastModifiedBy = _currentUserResolver.Username ?? "System";

            _unitOfWork.Repository<FTFundCampaign>().Update(campaign);
            await _unitOfWork.CompleteAsync();
            return campaign;
        }
    }

    // Specification classes
    public class CampaignsByFamilyTreeSpec : BaseSpecifcation<FTFundCampaign>
    {
        public CampaignsByFamilyTreeSpec(Guid familyTreeId, int page, int pageSize)
            : base(c => c.FTId == familyTreeId && c.IsDeleted != true)
        {
            AddOrderByDescending(c => c.CreatedOn);
            ApplyPaging((page - 1) * pageSize, pageSize);
        }
    }

    public class CampaignsByManagerSpec : BaseSpecifcation<FTFundCampaign>
    {
        public CampaignsByManagerSpec(Guid managerId, int page, int pageSize)
            : base(c => c.CampaignManagerId == managerId && c.IsDeleted != true)
        {
            AddOrderByDescending(c => c.CreatedOn);
            ApplyPaging((page - 1) * pageSize, pageSize);
        }
    }

    public class ActiveCampaignsSpec : BaseSpecifcation<FTFundCampaign>
    {
        public ActiveCampaignsSpec(int page, int pageSize)
            : base(c => c.Status== CampaignStatus.Active && c.IsDeleted != true)
        {
            AddOrderByDescending(c => c.CreatedOn);
            ApplyPaging((page - 1) * pageSize, pageSize);
        }
    }
}
