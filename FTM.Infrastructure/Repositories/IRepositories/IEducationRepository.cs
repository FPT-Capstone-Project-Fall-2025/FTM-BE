using FTM.Domain.Entities.Applications;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IEducationRepository
    {
        Task<IEnumerable<Education>> GetEducationsByUserIdAsync(Guid userId);
        Task<Education?> GetEducationByIdAsync(Guid id);
        Task<Education> CreateEducationAsync(Education education);
        Task<Education> UpdateEducationAsync(Education education);
        Task<bool> DeleteEducationAsync(Guid id);
        Task<bool> UserOwnsEducationAsync(Guid userId, Guid educationId);
    }
}
