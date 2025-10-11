using FTM.Domain.Entities.Applications;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IWorkRepository
    {
        Task<IEnumerable<WorkExperience>> GetWorkByUserIdAsync(Guid userId);
        Task<WorkExperience?> GetWorkByIdAsync(Guid id);
        Task<WorkExperience> CreateWorkAsync(WorkExperience workExperience);
        Task<WorkExperience> UpdateWorkAsync(WorkExperience workExperience);
        Task<bool> DeleteWorkAsync(Guid id);
        Task<bool> UserOwnsWorkAsync(Guid userId, Guid workId);
        
        // Position management
        Task<WorkPosition> AddPositionAsync(WorkPosition position);
        Task<WorkPosition> UpdatePositionAsync(WorkPosition position);
        Task<bool> DeletePositionAsync(Guid positionId);
    }
}
