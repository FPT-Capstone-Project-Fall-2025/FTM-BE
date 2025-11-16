using AutoMapper;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using FTM.Domain.Specification;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.Application.Services
{
    public class FTCampaignDonationService : IFTCampaignDonationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserResolver _currentUserResolver;
        private readonly IFTCampaignService _campaignService;

        public FTCampaignDonationService(
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

        public async Task<FTCampaignDonation?> GetByIdAsync(Guid id)
        {
            var donation = await _unitOfWork.Repository<FTCampaignDonation>().GetByIdAsync(id);
            if (donation == null || donation.IsDeleted == true)
                return null;
            return donation;
        }

        public async Task<FTCampaignDonation> AddAsync(FTCampaignDonation donation)
        {
            // Validate campaign exists
            var campaign = await _campaignService.GetByIdAsync(donation.CampaignId);
            if (campaign == null)
                throw new InvalidOperationException("Campaign not found");

            donation.Id = Guid.NewGuid();
            donation.Status = DonationStatus.Pending;
            donation.CreatedOn = DateTimeOffset.UtcNow;
            donation.CreatedOn = DateTimeOffset.UtcNow;
            donation.CreatedBy = _currentUserResolver.Username ?? "System";
            donation.LastModifiedOn = DateTimeOffset.UtcNow;
            donation.LastModifiedBy = _currentUserResolver.Username ?? "System";

            await _unitOfWork.Repository<FTCampaignDonation>().AddAsync(donation);
            await _unitOfWork.CompleteAsync();
            return donation;
        }

        public async Task<FTCampaignDonation> UpdateAsync(FTCampaignDonation donation)
        {
            // Set modified timestamp
            donation.LastModifiedOn = DateTimeOffset.UtcNow;
            donation.LastModifiedBy = _currentUserResolver.Username ?? "System";

            // Update without fetching existing (entity already tracked from GetByIdAsync)
            _unitOfWork.Repository<FTCampaignDonation>().Update(donation);
            await _unitOfWork.CompleteAsync();
            
            return donation;
        }

        public async Task<FTCampaignDonation?> GetByOrderCodeAsync(string orderCode)
        {
            if (!long.TryParse(orderCode, out var code))
                return null;

            var query = _unitOfWork.Repository<FTCampaignDonation>().GetQuery();
            var donation = await query
                .FirstOrDefaultAsync(d => d.PayOSOrderCode == code && d.IsDeleted != true);
            
            return donation;
        }

        public async Task<PaginatedResponse<FTCampaignDonationResponseDto>> GetCampaignDonationsAsync(
            Guid campaignId, int page, int pageSize)
        {
            var query = _unitOfWork.Repository<FTCampaignDonation>().GetQuery();
            
            var totalCount = await query
                .Where(d => d.CampaignId == campaignId && d.IsDeleted != true)
                .CountAsync();

            var donations = await query
                .Where(d => d.CampaignId == campaignId && d.IsDeleted != true)
                .OrderByDescending(d => d.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var donationDtos = donations.Select(d => MapToDonationDto(d)).ToList();

            return new PaginatedResponse<FTCampaignDonationResponseDto>
            {
                Items = donationDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResponse<FTCampaignDonationResponseDto>> GetUserDonationsAsync(
            Guid userId, int page, int pageSize)
        {
            var query = _unitOfWork.Repository<FTCampaignDonation>().GetQuery();
            
            var totalCount = await query
                .Where(d => d.FTMemberId == userId && d.IsDeleted != true)
                .CountAsync();

            var donations = await query
                .Where(d => d.FTMemberId == userId && d.IsDeleted != true)
                .OrderByDescending(d => d.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var donationDtos = donations.Select(d => MapToDonationDto(d)).ToList();

            return new PaginatedResponse<FTCampaignDonationResponseDto>
            {
                Items = donationDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<List<TopDonorDto>> GetTopDonorsAsync(Guid campaignId, int limit)
        {
            var query = _unitOfWork.Repository<FTCampaignDonation>().GetQuery();
            
            var topDonors = await query
                .Where(d => d.CampaignId == campaignId && 
                           d.Status == DonationStatus.Completed && 
                           !d.IsAnonymous && 
                           d.IsDeleted != true)
                .GroupBy(d => new { d.FTMemberId, d.DonorName })
                .Select(g => new TopDonorDto
                {
                    DonorId = g.Key.FTMemberId,
                    DonorName = g.Key.DonorName ?? "Unknown",
                    TotalDonated = g.Sum(d => d.DonationAmount),
                    DonationCount = g.Count()
                })
                .OrderByDescending(d => d.TotalDonated)
                .Take(limit)
                .ToListAsync();

            return topDonors;
        }

        public async Task<DonationStatisticsDto> GetDonationStatisticsAsync(Guid campaignId)
        {
            var query = _unitOfWork.Repository<FTCampaignDonation>().GetQuery();
            var donations = await query
                .Where(d => d.CampaignId == campaignId && d.IsDeleted != true)
                .ToListAsync();

            var completedDonations = donations.Where(d => d.Status == DonationStatus.Completed).ToList();
            
            var stats = new DonationStatisticsDto
            {
                CampaignId = campaignId,
                TotalDonations = completedDonations.Sum(d => d.DonationAmount),
                TotalDonors = completedDonations.Count,
                UniqueDonors = completedDonations.Where(d => d.FTMemberId.HasValue)
                    .Select(d => d.FTMemberId.Value).Distinct().Count(),
                AverageDonation = completedDonations.Any() ? completedDonations.Average(d => d.DonationAmount) : 0,
                HighestDonation = completedDonations.Any() ? completedDonations.Max(d => d.DonationAmount) : 0,
                LowestDonation = completedDonations.Any() ? completedDonations.Min(d => d.DonationAmount) : 0,
                FirstDonationDate = completedDonations.Any() ? completedDonations.Min(d => d.CreatedOn).DateTime : (DateTime?)null,
                LastDonationDate = completedDonations.Any() ? completedDonations.Max(d => d.CreatedOn).DateTime : (DateTime?)null,
                DonationsByStatus = donations.GroupBy(d => d.Status)
                    .ToDictionary(g => g.Key, g => g.Count()),
                MonthlyTrend = completedDonations
                    .GroupBy(d => new { d.CreatedOn.Year, d.CreatedOn.Month })
                    .Select(g => new MonthlyDonationDto
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Amount = g.Sum(d => d.DonationAmount),
                        Count = g.Count()
                    })
                    .OrderBy(m => m.Year).ThenBy(m => m.Month)
                    .ToList()
            };

            return stats;
        }

        public async Task<FTCampaignDonation> ProcessCompletedDonationAsync(string orderCode)
        {
            var donation = await GetByOrderCodeAsync(orderCode);
            if (donation == null)
                throw new InvalidOperationException("Donation not found");

            if (donation.Status == DonationStatus.Completed)
                return donation; // Already processed

            donation.Status = DonationStatus.Completed;
            donation.LastModifiedOn = DateTimeOffset.UtcNow;
            donation.LastModifiedBy = _currentUserResolver.Username ?? "PayOS";

            _unitOfWork.Repository<FTCampaignDonation>().Update(donation);

            // Update campaign balance
            await _campaignService.UpdateCampaignAmountAsync(donation.CampaignId, donation.DonationAmount);

            await _unitOfWork.CompleteAsync();
            return donation;
        }

        public async Task<FTFundCampaign?> GetCampaignForDonationAsync(Guid campaignId)
        {
            var campaign = await _unitOfWork.Repository<FTFundCampaign>().GetByIdAsync(campaignId);
            if (campaign == null || campaign.IsDeleted == true)
                return null;
            return campaign;
        }

        public async Task<List<FTCampaignDonationResponseDto>> GetPendingDonationsByCampaignAsync(Guid campaignId)
        {
            var donations = await _unitOfWork.Repository<FTCampaignDonation>().GetQuery()
                .Where(d => d.CampaignId == campaignId 
                    && d.Status == DonationStatus.Pending 
                    && d.IsDeleted == false)
                .Include(d => d.Member)
                .Include(d => d.Campaign)
                .OrderBy(d => d.CreatedOn)
                .ToListAsync();

            return donations.Select(MapToDonationDto).ToList();
        }

        public async Task<List<FTCampaignDonationResponseDto>> GetAllPendingDonationsAsync(Guid? familyTreeId)
        {
            IQueryable<FTCampaignDonation> query = _unitOfWork.Repository<FTCampaignDonation>().GetQuery()
                .Where(d => d.Status == DonationStatus.Pending && d.IsDeleted == false)
                .Include(d => d.Member)
                .Include(d => d.Campaign);

            // Filter by family tree if provided
            if (familyTreeId.HasValue)
            {
                query = query.Where(d => d.Campaign.FTId == familyTreeId.Value);
            }

            var donations = await query
                .OrderBy(d => d.CreatedOn)
                .ToListAsync();

            return donations.Select(MapToDonationDto).ToList();
        }

        public async Task<List<FTCampaignDonationResponseDto>> GetUserPendingDonationsAsync(Guid userId)
        {
            var donations = await _unitOfWork.Repository<FTCampaignDonation>().GetQuery()
                .Where(d => d.FTMemberId == userId 
                    && d.Status == DonationStatus.Pending 
                    && d.IsDeleted == false)
                .Include(d => d.Member)
                .Include(d => d.Campaign)
                .OrderByDescending(d => d.CreatedOn) // Newest first
                .ToListAsync();

            return donations.Select(MapToDonationDto).ToList();
        }

        private FTCampaignDonationResponseDto MapToDonationDto(FTCampaignDonation donation)
        {
            var dto = _mapper.Map<FTCampaignDonationResponseDto>(donation);
            
            // Mask anonymous donations
            if (donation.IsAnonymous)
            {
                dto.DonorName = "Anonymous";
            }
            
            return dto;
        }
    }
}
