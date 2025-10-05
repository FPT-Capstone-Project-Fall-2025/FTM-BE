using FTM.Domain.Entities.Applications;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IBiographyRepository
    {
        // Biography Description methods
        Task<Biography?> GetDescriptionByUserIdAsync(Guid userId);
        Task<Biography> CreateDescriptionAsync(Biography biography);
        Task<Biography> UpdateDescriptionAsync(Biography biography);
        
        // Biography Events methods
        Task<IEnumerable<Biography>> GetEventsByUserIdAsync(Guid userId);
        Task<Biography?> GetEventByIdAsync(Guid id);
        Task<Biography?> GetUserEventByIdAsync(Guid userId, Guid eventId);
        Task<Biography> CreateEventAsync(Biography biography);
        Task<Biography> UpdateEventAsync(Biography biography);
        Task<bool> DeleteEventAsync(Guid id);
        Task<bool> UserOwnsEventAsync(Guid userId, Guid eventId);
    }
}