using FTM.Application.IServices;
using FTM.Domain.Entities.Applications;
using FTM.Domain.Models.Applications;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FTM.Application.Services
{
    public class BiographyService : IBiographyService
    {
        private readonly IBiographyRepository _biographyRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BiographyService(
            IBiographyRepository biographyRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _biographyRepository = biographyRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private Guid GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");
            
            if (!Guid.TryParse(userId, out var guidUserId))
                throw new UnauthorizedAccessException("Invalid user ID format");
                
            return guidUserId;
        }

        #region Biography Description

        public async Task<BiographyDescriptionResponse> GetCurrentUserBiographyDescriptionAsync()
        {
            var userId = GetCurrentUserId();
            var biography = await _biographyRepository.GetDescriptionByUserIdAsync(userId);

            return new BiographyDescriptionResponse
            {
                Description = biography?.Description,
                CreatedAt = biography?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = biography?.UpdatedAt
            };
        }

        public async Task<BiographyDescriptionResponse> UpdateCurrentUserBiographyDescriptionAsync(UpdateBiographyDescriptionRequest request)
        {
            var userId = GetCurrentUserId();
            var existingBiography = await _biographyRepository.GetDescriptionByUserIdAsync(userId);

            Biography biography;
            if (existingBiography == null)
            {
                // Tạo mới nếu chưa có
                biography = new Biography
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Description = request.Description
                };
                biography = await _biographyRepository.CreateDescriptionAsync(biography);
            }
            else
            {
                // Cập nhật nếu đã có
                existingBiography.Description = request.Description;
                biography = await _biographyRepository.UpdateDescriptionAsync(existingBiography);
            }

            return new BiographyDescriptionResponse
            {
                Description = biography.Description,
                CreatedAt = biography.CreatedAt,
                UpdatedAt = biography.UpdatedAt
            };
        }

        #endregion

        #region Biography Events

        public async Task<IEnumerable<BiographyEventResponse>> GetCurrentUserBiographyEventsAsync()
        {
            var userId = GetCurrentUserId();
            var events = await _biographyRepository.GetEventsByUserIdAsync(userId);
            
            return events.Select(e => new BiographyEventResponse
            {
                Id = e.Id,
                Title = e.Title!,
                Description = e.Description,
                EventDate = e.EventDate!.Value,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            });
        }

        public async Task<BiographyEventResponse?> GetBiographyEventByIdAsync(Guid eventId)
        {
            var userId = GetCurrentUserId();
            var biographyEvent = await _biographyRepository.GetUserEventByIdAsync(userId, eventId);
            
            if (biographyEvent == null)
                return null;

            return new BiographyEventResponse
            {
                Id = biographyEvent.Id,
                Title = biographyEvent.Title!,
                Description = biographyEvent.Description,
                EventDate = biographyEvent.EventDate!.Value,
                CreatedAt = biographyEvent.CreatedAt,
                UpdatedAt = biographyEvent.UpdatedAt
            };
        }

        public async Task<BiographyEventResponse> CreateBiographyEventAsync(CreateBiographyEventRequest request)
        {
            var userId = GetCurrentUserId();
            
            var biographyEvent = new Biography
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title,
                Description = request.Description,
                EventDate = request.EventDate
            };

            var createdEvent = await _biographyRepository.CreateEventAsync(biographyEvent);

            return new BiographyEventResponse
            {
                Id = createdEvent.Id,
                Title = createdEvent.Title!,
                Description = createdEvent.Description,
                EventDate = createdEvent.EventDate!.Value,
                CreatedAt = createdEvent.CreatedAt,
                UpdatedAt = createdEvent.UpdatedAt
            };
        }

        public async Task<BiographyEventResponse?> UpdateBiographyEventAsync(Guid eventId, UpdateBiographyEventRequest request)
        {
            var userId = GetCurrentUserId();
            var existingEvent = await _biographyRepository.GetUserEventByIdAsync(userId, eventId);
            
            if (existingEvent == null)
                return null;

            existingEvent.Title = request.Title;
            existingEvent.Description = request.Description;
            existingEvent.EventDate = request.EventDate;

            var updatedEvent = await _biographyRepository.UpdateEventAsync(existingEvent);

            return new BiographyEventResponse
            {
                Id = updatedEvent.Id,
                Title = updatedEvent.Title!,
                Description = updatedEvent.Description,
                EventDate = updatedEvent.EventDate!.Value,
                CreatedAt = updatedEvent.CreatedAt,
                UpdatedAt = updatedEvent.UpdatedAt
            };
        }

        public async Task<bool> DeleteBiographyEventAsync(Guid eventId)
        {
            var userId = GetCurrentUserId();
            var userOwnsEvent = await _biographyRepository.UserOwnsEventAsync(userId, eventId);
            
            if (!userOwnsEvent)
                return false;

            return await _biographyRepository.DeleteEventAsync(eventId);
        }

        #endregion
    }
}