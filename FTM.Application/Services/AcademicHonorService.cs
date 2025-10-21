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
    public class AcademicHonorService : IAcademicHonorService
    {
        private readonly IAcademicHonorRepository _academicHonorRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBlobStorageService _blobStorageService;

        public AcademicHonorService(
            IAcademicHonorRepository academicHonorRepository,
            IHttpContextAccessor httpContextAccessor,
            IBlobStorageService blobStorageService)
        {
            _academicHonorRepository = academicHonorRepository;
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

        public async Task<Pagination<AcademicHonorDto>> GetAcademicHonorsAsync(AcademicHonorSpecParams specParams)
        {
            var honors = await _academicHonorRepository.GetAcademicHonorsAsync(specParams);
            var totalCount = await _academicHonorRepository.GetTotalCountAsync(specParams);

            var honorDtos = honors.Select(h => new AcademicHonorDto
            {
                Id = h.Id,
                GPMemberId = h.GPMemberId,
                MemberFullName = h.GPMember.Fullname,
                MemberPhotoUrl = h.GPMember.Picture,
                FamilyTreeId = h.FamilyTreeId,
                AchievementTitle = h.AchievementTitle,
                InstitutionName = h.InstitutionName,
                DegreeOrCertificate = h.DegreeOrCertificate,
                YearOfAchievement = h.YearOfAchievement,
                Description = h.Description,
                PhotoUrl = h.PhotoUrl,
                IsDisplayed = h.IsDisplayed,
                CreatedOn = h.CreatedOn,
                LastModifiedOn = h.LastModifiedOn
            }).ToList();

            var pageSize = specParams.Take > 0 ? specParams.Take : 10;
            var pageIndex = specParams.Skip > 0 ? (specParams.Skip / pageSize + 1) : 1;

            return new Pagination<AcademicHonorDto>(pageIndex, pageSize, totalCount, honorDtos);
        }

        public async Task<AcademicHonorDto?> GetAcademicHonorByIdAsync(Guid id)
        {
            var honor = await _academicHonorRepository.GetAcademicHonorByIdAsync(id);
            if (honor == null) return null;

            return new AcademicHonorDto
            {
                Id = honor.Id,
                GPMemberId = honor.GPMemberId,
                MemberFullName = honor.GPMember.Fullname,
                MemberPhotoUrl = honor.GPMember.Picture,
                FamilyTreeId = honor.FamilyTreeId,
                AchievementTitle = honor.AchievementTitle,
                InstitutionName = honor.InstitutionName,
                DegreeOrCertificate = honor.DegreeOrCertificate,
                YearOfAchievement = honor.YearOfAchievement,
                Description = honor.Description,
                PhotoUrl = honor.PhotoUrl,
                IsDisplayed = honor.IsDisplayed,
                CreatedOn = honor.CreatedOn,
                LastModifiedOn = honor.LastModifiedOn
            };
        }

        public async Task<AcademicHonorDto> CreateAcademicHonorAsync(CreateAcademicHonorRequest request)
        {
            // Verify member exists in family tree
            var memberExists = await _academicHonorRepository.MemberExistsInFamilyTreeAsync(request.GPMemberId, request.FamilyTreeId);
            if (!memberExists)
                throw new InvalidOperationException("Member not found in the specified family tree");

            // Upload photo if provided
            string? photoUrl = null;
            if (request.Photo != null)
            {
                photoUrl = await _blobStorageService.UploadFileAsync(request.Photo, "honor-certificates", null);
            }

            var honor = new AcademicHonor
            {
                Id = Guid.NewGuid(),
                GPMemberId = request.GPMemberId,
                FamilyTreeId = request.FamilyTreeId,
                AchievementTitle = request.AchievementTitle,
                InstitutionName = request.InstitutionName,
                DegreeOrCertificate = request.DegreeOrCertificate,
                YearOfAchievement = request.YearOfAchievement,
                Description = request.Description,
                PhotoUrl = photoUrl,
                IsDisplayed = request.IsDisplayed
            };

            var created = await _academicHonorRepository.CreateAcademicHonorAsync(honor);

            return new AcademicHonorDto
            {
                Id = created.Id,
                GPMemberId = created.GPMemberId,
                MemberFullName = created.GPMember.Fullname,
                MemberPhotoUrl = created.GPMember.Picture,
                FamilyTreeId = created.FamilyTreeId,
                AchievementTitle = created.AchievementTitle,
                InstitutionName = created.InstitutionName,
                DegreeOrCertificate = created.DegreeOrCertificate,
                YearOfAchievement = created.YearOfAchievement,
                Description = created.Description,
                PhotoUrl = created.PhotoUrl,
                IsDisplayed = created.IsDisplayed,
                CreatedOn = created.CreatedOn,
                LastModifiedOn = created.LastModifiedOn
            };
        }

        public async Task<AcademicHonorDto?> UpdateAcademicHonorAsync(Guid id, UpdateAcademicHonorRequest request)
        {
            var existing = await _academicHonorRepository.GetAcademicHonorByIdAsync(id);
            
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

            if (!string.IsNullOrWhiteSpace(request.InstitutionName))
                existing.InstitutionName = request.InstitutionName;

            if (request.DegreeOrCertificate != null)
                existing.DegreeOrCertificate = request.DegreeOrCertificate;

            if (request.YearOfAchievement.HasValue)
                existing.YearOfAchievement = request.YearOfAchievement.Value;

            if (request.Description != null)
                existing.Description = request.Description;

            if (request.IsDisplayed.HasValue)
                existing.IsDisplayed = request.IsDisplayed.Value;

            var updated = await _academicHonorRepository.UpdateAcademicHonorAsync(existing);

            return new AcademicHonorDto
            {
                Id = updated.Id,
                GPMemberId = updated.GPMemberId,
                MemberFullName = updated.GPMember.Fullname,
                MemberPhotoUrl = updated.GPMember.Picture,
                FamilyTreeId = updated.FamilyTreeId,
                AchievementTitle = updated.AchievementTitle,
                InstitutionName = updated.InstitutionName,
                DegreeOrCertificate = updated.DegreeOrCertificate,
                YearOfAchievement = updated.YearOfAchievement,
                Description = updated.Description,
                PhotoUrl = updated.PhotoUrl,
                IsDisplayed = updated.IsDisplayed,
                CreatedOn = updated.CreatedOn,
                LastModifiedOn = updated.LastModifiedOn
            };
        }

        public async Task<bool> DeleteAcademicHonorAsync(Guid id)
        {
            var existing = await _academicHonorRepository.GetAcademicHonorByIdAsync(id);
            
            if (existing == null) return false;

            return await _academicHonorRepository.DeleteAcademicHonorAsync(id);
        }
    }
}
