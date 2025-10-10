using FTM.Domain.Models.Applications;

namespace FTM.Application.IServices
{
    public interface IWorkService
    {
        Task<IEnumerable<WorkResponse>> GetCurrentUserWorkAsync();
        Task<WorkResponse?> GetWorkByIdAsync(Guid workId);
        Task<WorkResponse> CreateWorkAsync(CreateWorkRequest request);
        Task<WorkResponse?> UpdateWorkAsync(Guid workId, UpdateWorkRequest request);
        Task<bool> DeleteWorkAsync(Guid workId);
    }
}
