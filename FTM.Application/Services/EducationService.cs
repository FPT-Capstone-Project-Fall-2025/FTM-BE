using FTM.Application.IServices;
using FTM.Domain.Models.Applications;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FTM.Application.Services
{
    public class EducationService : IEducationService
    {
        private readonly IEducationRepository _educationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EducationService(IEducationRepository educationRepository, IHttpContextAccessor httpContextAccessor)
        {
            _educationRepository = educationRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private Guid GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("User not authenticated");
            if (!Guid.TryParse(userId, out var guidUserId)) throw new UnauthorizedAccessException("Invalid user ID format");
            return guidUserId;
        }

        public async Task<IEnumerable<EducationResponse>> GetCurrentUserEducationsAsync()
        {
            var userId = GetCurrentUserId();
            var educations = await _educationRepository.GetEducationsByUserIdAsync(userId);
            return educations.Select(e => new EducationResponse
            {
                Id = e.Id,
                InstitutionName = e.InstitutionName,
                Major = e.Major,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsCurrent = e.IsCurrent,
                Description = e.Description,
                Location = e.Location,
                CreatedAt = e.CreatedOn,
                UpdatedAt = e.LastModifiedOn
            });
        }

        public async Task<EducationResponse?> GetEducationByIdAsync(Guid educationId)
        {
            var userId = GetCurrentUserId();
            var education = await _educationRepository.GetEducationByIdAsync(educationId);
            if (education == null || education.UserId != userId) return null;
            return new EducationResponse
            {
                Id = education.Id,
                InstitutionName = education.InstitutionName,
                Major = education.Major,
                StartDate = education.StartDate,
                EndDate = education.EndDate,
                IsCurrent = education.IsCurrent,
                Description = education.Description,
                Location = education.Location,
                CreatedAt = education.CreatedOn,
                UpdatedAt = education.LastModifiedOn
            };
        }

        public async Task<EducationResponse> CreateEducationAsync(CreateEducationRequest request)
        {
            var userId = GetCurrentUserId();
            var education = new FTM.Domain.Entities.Applications.Education
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                InstitutionName = request.InstitutionName,
                Major = request.Major,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsCurrent = request.IsCurrent,
                Description = request.Description,
                Location = request.Location
            };

            var created = await _educationRepository.CreateEducationAsync(education);

            return new EducationResponse
            {
                Id = created.Id,
                InstitutionName = created.InstitutionName,
                Major = created.Major,
                StartDate = created.StartDate,
                EndDate = created.EndDate,
                IsCurrent = created.IsCurrent,
                Description = created.Description,
                Location = created.Location,
                CreatedAt = created.CreatedOn,
                UpdatedAt = created.LastModifiedOn
            };
        }

        public async Task<EducationResponse?> UpdateEducationAsync(Guid educationId, UpdateEducationRequest request)
        {
            var userId = GetCurrentUserId();
            var existing = await _educationRepository.GetEducationByIdAsync(educationId);
            if (existing == null || existing.UserId != userId) return null;

            existing.InstitutionName = request.InstitutionName;
            existing.Major = request.Major;
            existing.StartDate = request.StartDate;
            existing.EndDate = request.EndDate;
            existing.IsCurrent = request.IsCurrent;
            existing.Description = request.Description;
            existing.Location = request.Location;

            var updated = await _educationRepository.UpdateEducationAsync(existing);

            return new EducationResponse
            {
                Id = updated.Id,
                InstitutionName = updated.InstitutionName,
                Major = updated.Major,
                StartDate = updated.StartDate,
                EndDate = updated.EndDate,
                IsCurrent = updated.IsCurrent,
                Description = updated.Description,
                Location = updated.Location,
                CreatedAt = updated.CreatedOn,
                UpdatedAt = updated.LastModifiedOn
            };
        }

        public async Task<bool> DeleteEducationAsync(Guid educationId)
        {
            var userId = GetCurrentUserId();
            var owns = await _educationRepository.UserOwnsEducationAsync(userId, educationId);
            if (!owns) return false;
            return await _educationRepository.DeleteEducationAsync(educationId);
        }
    }
}
