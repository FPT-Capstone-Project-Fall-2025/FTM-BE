using FTM.Domain.Models.Applications;

namespace FTM.Application.IServices
{
    public interface IBiographyService
    {
        // Biography Description methods
        Task<BiographyDescriptionResponse> GetCurrentUserBiographyDescriptionAsync();
        Task<BiographyDescriptionResponse> UpdateCurrentUserBiographyDescriptionAsync(UpdateBiographyDescriptionRequest request);
        
        // Biography Events methods
        Task<IEnumerable<BiographyEventResponse>> GetCurrentUserBiographyEventsAsync();
        Task<BiographyEventResponse?> GetBiographyEventByIdAsync(Guid eventId);
        Task<BiographyEventResponse> CreateBiographyEventAsync(CreateBiographyEventRequest request);
        Task<BiographyEventResponse?> UpdateBiographyEventAsync(Guid eventId, UpdateBiographyEventRequest request);
        Task<bool> DeleteBiographyEventAsync(Guid eventId);
    }
}