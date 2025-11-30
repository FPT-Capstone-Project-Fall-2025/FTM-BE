using FTM.Application.IServices;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FTM.Application.Services
{
    public class FTFundDonationService : IFTFundDonationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserResolver _currentUserResolver;

        public FTFundDonationService(
            IUnitOfWork unitOfWork,
            ICurrentUserResolver currentUserResolver)
        {
            _unitOfWork = unitOfWork;
            _currentUserResolver = currentUserResolver;
        }

        public async Task<FTFundDonation?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                .Include(d => d.Fund)
                .Include(d => d.Member)
                .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false);
        }

        public async Task<PaginatedResponse<FTFundDonation>> GetDonationsByFundAsync(
            Guid fundId, int page, int pageSize, DonationStatus? status = null)
        {
            var query = _unitOfWork.Repository<FTFundDonation>().GetQuery()
                .Where(d => d.FTFundId == fundId && d.IsDeleted == false);

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

            var donations = await query
                .Include(d => d.Fund)
                .Include(d => d.Member)
                .OrderByDescending(d => d.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<FTFundDonation>
            {
                Items = donations,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedResponse<FTFundDonation>> GetPendingDonationsAsync(
            Guid? fundId, int page, int pageSize)
        {
            IQueryable<FTFundDonation> query = _unitOfWork.Repository<FTFundDonation>().GetQuery()
                .Where(d => d.Status == DonationStatus.Pending && d.IsDeleted == false);

            if (fundId.HasValue)
            {
                query = query.Where(d => d.FTFundId == fundId.Value);
            }

            var totalCount = await query.CountAsync();

            var donations = await query
                .Include(d => d.Fund)
                .Include(d => d.Member)
                .OrderByDescending(d => d.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<FTFundDonation>
            {
                Items = donations,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<List<FTFundDonation>> GetUserPendingDonationsAsync(Guid userId)
        {
            return await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                .Where(d => d.FTMemberId == userId && 
                           d.Status == DonationStatus.Pending && 
                           d.IsDeleted == false)
                .Include(d => d.Fund)
                .Include(d => d.Member)
                .OrderByDescending(d => d.CreatedOn)
                .ToListAsync();
        }

        public async Task<FTFundDonation> CreateDonationAsync(FTFundDonation donation)
        {
            donation.Id = Guid.NewGuid();
            donation.IsDeleted = false;
            donation.CreatedOn = DateTimeOffset.UtcNow;

            await _unitOfWork.Repository<FTFundDonation>().AddAsync(donation);
            await _unitOfWork.CompleteAsync();

            return donation;
        }

        public async Task<FTFundDonation> UpdateDonationAsync(FTFundDonation donation)
        {
            var existing = await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                .FirstOrDefaultAsync(d => d.Id == donation.Id && d.IsDeleted == false);

            if (existing == null)
                throw new InvalidOperationException("Donation not found");

            if (existing.Status != DonationStatus.Pending)
                throw new InvalidOperationException("Can only update pending donations");

            // Update fields
            if (donation.DonationMoney > 0)
                existing.DonationMoney = donation.DonationMoney;

            if (!string.IsNullOrEmpty(donation.DonorName))
                existing.DonorName = donation.DonorName;

            if (donation.PaymentNotes != null)
                existing.PaymentNotes = donation.PaymentNotes;

            if (donation.ProofImages != null)
                existing.ProofImages = donation.ProofImages;

            _unitOfWork.Repository<FTFundDonation>().Update(existing);
            await _unitOfWork.CompleteAsync();

            return existing;
        }

        public async Task DeleteDonationAsync(Guid id)
        {
            var donation = await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false);

            if (donation == null)
                throw new InvalidOperationException("Donation not found");

            if (donation.Status != DonationStatus.Pending)
                throw new InvalidOperationException("Can only delete pending donations");

            donation.IsDeleted = true;
            _unitOfWork.Repository<FTFundDonation>().Update(donation);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<FTFundDonation> ConfirmDonationAsync(Guid id, Guid confirmerId, string? notes)
        {
            var donation = await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                .Include(d => d.Fund)
                .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false);

            if (donation == null)
                throw new InvalidOperationException("Donation not found");

            if (donation.Status != DonationStatus.Pending)
                throw new InvalidOperationException("Donation is not pending");

            // Update donation status
            donation.Status = DonationStatus.Completed;
            donation.ConfirmedBy = confirmerId;
            donation.ConfirmedOn = DateTimeOffset.UtcNow;
            donation.ConfirmationNotes = notes;

            _unitOfWork.Repository<FTFundDonation>().Update(donation);

            // Update fund balance
            if (donation.Fund != null)
            {
                donation.Fund.CurrentMoney += donation.DonationMoney;
                _unitOfWork.Repository<FTFund>().Update(donation.Fund);
            }

            await _unitOfWork.CompleteAsync();

            return donation;
        }

        public async Task<FTFundDonation> RejectDonationAsync(Guid id, string reason)
        {
            var donation = await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false);

            if (donation == null)
                throw new InvalidOperationException("Donation not found");

            if (donation.Status != DonationStatus.Pending)
                throw new InvalidOperationException("Can only reject pending donations");

            donation.Status = DonationStatus.Rejected;
            donation.ConfirmationNotes = $"Rejected: {reason}";
            donation.ConfirmedOn = DateTimeOffset.UtcNow;

            _unitOfWork.Repository<FTFundDonation>().Update(donation);
            await _unitOfWork.CompleteAsync();

            return donation;
        }

        public async Task<object> GetDonationStatisticsAsync(Guid fundId)
        {
            var donations = await _unitOfWork.Repository<FTFundDonation>().GetQuery()
                .Where(d => d.FTFundId == fundId && d.IsDeleted == false)
                .ToListAsync();

            var completedDonations = donations.Where(d => d.Status == DonationStatus.Completed).ToList();
            var pendingDonations = donations.Where(d => d.Status == DonationStatus.Pending).ToList();

            return new
            {
                TotalDonations = completedDonations.Sum(d => d.DonationMoney),
                TotalDonors = completedDonations.Count,
                PendingDonations = pendingDonations.Sum(d => d.DonationMoney),
                PendingCount = pendingDonations.Count,
                CompletedCount = completedDonations.Count,
                AverageDonation = completedDonations.Any() ? completedDonations.Average(d => d.DonationMoney) : 0,
                HighestDonation = completedDonations.Any() ? completedDonations.Max(d => d.DonationMoney) : 0,
                LowestDonation = completedDonations.Any() ? completedDonations.Min(d => d.DonationMoney) : 0
            };
        }
    }
}
