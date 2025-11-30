using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FTM.Domain.Models;
using FTM.Domain.DTOs.HonorBoard;
using FTM.Domain.Specification.HonorBoard;

namespace FTM.Application.IServices
{
    public interface IAcademicHonorService
    {
        Task<Pagination<AcademicHonorDto>> GetAcademicHonorsAsync(AcademicHonorSpecParams specParams);
        Task<AcademicHonorDto?> GetAcademicHonorByIdAsync(Guid id);
        Task<AcademicHonorDto> CreateAcademicHonorAsync(CreateAcademicHonorRequest request);
        Task<AcademicHonorDto?> UpdateAcademicHonorAsync(Guid id, UpdateAcademicHonorRequest request);
        Task<bool> DeleteAcademicHonorAsync(Guid id);
    }
}
