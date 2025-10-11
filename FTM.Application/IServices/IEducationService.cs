using FTM.Domain.Models.Applications;

namespace FTM.Application.IServices
{
    public interface IEducationService
    {
        Task<IEnumerable<EducationResponse>> GetCurrentUserEducationsAsync();
        Task<EducationResponse?> GetEducationByIdAsync(Guid educationId);
        Task<EducationResponse> CreateEducationAsync(CreateEducationRequest request);
        Task<EducationResponse?> UpdateEducationAsync(Guid educationId, UpdateEducationRequest request);
        Task<bool> DeleteEducationAsync(Guid educationId);
    }
}
