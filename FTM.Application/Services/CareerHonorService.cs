using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FTM.Domain.Models;
using FTM.Application.IServices;
using FTM.Domain.DTOs.HonorBoard;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Specification.HonorBoard;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;

namespace FTM.Application.Services
{
    public class CareerHonorService : ICareerHonorService
    {
        private readonly ICareerHonorRepository _careerHonorRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBlobStorageService _blobStorageService;

        public CareerHonorService(
            ICareerHonorRepository careerHonorRepository,
            IHttpContextAccessor httpContextAccessor,
            IBlobStorageService blobStorageService)
        {
            _careerHonorRepository = careerHonorRepository;
            _httpContextAccessor = httpContextAccessor;
            _blobStorageService = blobStorageService;
        }

        private Guid GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");
            return Guid.Parse(userId);
        }

        public async Task<Pagination<CareerHonorDto>> GetCareerHonorsAsync(CareerHonorSpecParams specParams)
        {
            var honors = await _careerHonorRepository.GetCareerHonorsAsync(specParams);
            var totalCount = await _careerHonorRepository.GetTotalCountAsync(specParams);

            var honorDtos = honors.Select(h => new CareerHonorDto
            {
                Id = h.Id,
                GPMemberId = h.GPMemberId,
                MemberFullName = h.GPMember.Fullname,
                MemberPhotoUrl = h.GPMember.Picture,
                FamilyTreeId = h.FamilyTreeId,
                AchievementTitle = h.AchievementTitle,
                OrganizationName = h.OrganizationName,
                Position = h.Position,
                YearOfAchievement = h.YearOfAchievement,
                Description = h.Description,
                PhotoUrl = h.PhotoUrl,
                IsDisplayed = h.IsDisplayed,
                CreatedOn = h.CreatedOn,
                LastModifiedOn = h.LastModifiedOn
            }).ToList();

            var pageSize = specParams.Take > 0 ? specParams.Take : 10;
            var pageIndex = specParams.Skip > 0 ? (specParams.Skip / pageSize + 1) : 1;

            return new Pagination<CareerHonorDto>(pageIndex, pageSize, totalCount, honorDtos);
        }

        public async Task<CareerHonorDto?> GetCareerHonorByIdAsync(Guid id)
        {
            var honor = await _careerHonorRepository.GetCareerHonorByIdAsync(id);
            if (honor == null) return null;

            return new CareerHonorDto
            {
                Id = honor.Id,
                GPMemberId = honor.GPMemberId,
                MemberFullName = honor.GPMember.Fullname,
                MemberPhotoUrl = honor.GPMember.Picture,
                FamilyTreeId = honor.FamilyTreeId,
                AchievementTitle = honor.AchievementTitle,
                OrganizationName = honor.OrganizationName,
                Position = honor.Position,
                YearOfAchievement = honor.YearOfAchievement,
                Description = honor.Description,
                PhotoUrl = honor.PhotoUrl,
                IsDisplayed = honor.IsDisplayed,
                CreatedOn = honor.CreatedOn,
                LastModifiedOn = honor.LastModifiedOn
            };
        }

        public async Task<CareerHonorDto> CreateCareerHonorAsync(CreateCareerHonorRequest request)
        {
            // Verify member exists in family tree
            var memberExists = await _careerHonorRepository.MemberExistsInFamilyTreeAsync(request.GPMemberId, request.FamilyTreeId);
            if (!memberExists)
                throw new InvalidOperationException("Member not found in the specified family tree");

            // Upload photo if provided
            string? photoUrl = null;
            if (request.Photo != null)
            {
                photoUrl = await _blobStorageService.UploadFileAsync(request.Photo, "honor-certificates", null);
            }

            var honor = new CareerHonor
            {
                Id = Guid.NewGuid(),
                GPMemberId = request.GPMemberId,
                FamilyTreeId = request.FamilyTreeId,
                AchievementTitle = request.AchievementTitle,
                OrganizationName = request.OrganizationName,
                Position = request.Position,
                YearOfAchievement = request.YearOfAchievement,
                Description = request.Description,
                PhotoUrl = photoUrl,
                IsDisplayed = request.IsDisplayed
            };

            var created = await _careerHonorRepository.CreateCareerHonorAsync(honor);

            return new CareerHonorDto
            {
                Id = created.Id,
                GPMemberId = created.GPMemberId,
                MemberFullName = created.GPMember.Fullname,
                MemberPhotoUrl = created.GPMember.Picture,
                FamilyTreeId = created.FamilyTreeId,
                AchievementTitle = created.AchievementTitle,
                OrganizationName = created.OrganizationName,
                Position = created.Position,
                YearOfAchievement = created.YearOfAchievement,
                Description = created.Description,
                PhotoUrl = created.PhotoUrl,
                IsDisplayed = created.IsDisplayed,
                CreatedOn = created.CreatedOn,
                LastModifiedOn = created.LastModifiedOn
            };
        }

        public async Task<CareerHonorDto?> UpdateCareerHonorAsync(Guid id, UpdateCareerHonorRequest request)
        {
            var existing = await _careerHonorRepository.GetCareerHonorByIdAsync(id);
            
            if (existing == null) return null;

            // Upload new photo if provided
            if (request.Photo != null)
            {
                // Delete old photo if exists
                if (!string.IsNullOrEmpty(existing.PhotoUrl))
                {
                    var oldFileName = existing.PhotoUrl.Split('/').Last();
                    await _blobStorageService.DeleteFileAsync("honor-certificates", oldFileName);
                }

                // Upload new photo
                existing.PhotoUrl = await _blobStorageService.UploadFileAsync(request.Photo, "honor-certificates", null);
            }

            // Only update fields that are provided
            if (!string.IsNullOrWhiteSpace(request.AchievementTitle))
                existing.AchievementTitle = request.AchievementTitle;

            if (!string.IsNullOrWhiteSpace(request.OrganizationName))
                existing.OrganizationName = request.OrganizationName;

            if (request.Position != null)
                existing.Position = request.Position;

            if (request.YearOfAchievement.HasValue)
                existing.YearOfAchievement = request.YearOfAchievement.Value;

            if (request.Description != null)
                existing.Description = request.Description;

            if (request.IsDisplayed.HasValue)
                existing.IsDisplayed = request.IsDisplayed.Value;

            var updated = await _careerHonorRepository.UpdateCareerHonorAsync(existing);

            return new CareerHonorDto
            {
                Id = updated.Id,
                GPMemberId = updated.GPMemberId,
                MemberFullName = updated.GPMember.Fullname,
                MemberPhotoUrl = updated.GPMember.Picture,
                FamilyTreeId = updated.FamilyTreeId,
                AchievementTitle = updated.AchievementTitle,
                OrganizationName = updated.OrganizationName,
                Position = updated.Position,
                YearOfAchievement = updated.YearOfAchievement,
                Description = updated.Description,
                PhotoUrl = updated.PhotoUrl,
                IsDisplayed = updated.IsDisplayed,
                CreatedOn = updated.CreatedOn,
                LastModifiedOn = updated.LastModifiedOn
            };
        }

        public async Task<bool> DeleteCareerHonorAsync(Guid id)
        {
            var existing = await _careerHonorRepository.GetCareerHonorByIdAsync(id);
            
            if (existing == null) return false;

            return await _careerHonorRepository.DeleteCareerHonorAsync(id);
        }
    }
}
