using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FTM.Domain.Models;
using FTM.Domain.DTOs.HonorBoard;
using FTM.Domain.Specification.HonorBoard;

namespace FTM.Application.IServices
{
    public interface ICareerHonorService
    {
        Task<Pagination<CareerHonorDto>> GetCareerHonorsAsync(CareerHonorSpecParams specParams);
        Task<CareerHonorDto?> GetCareerHonorByIdAsync(Guid id);
        Task<CareerHonorDto> CreateCareerHonorAsync(CreateCareerHonorRequest request);
        Task<CareerHonorDto?> UpdateCareerHonorAsync(Guid id, UpdateCareerHonorRequest request);
        Task<bool> DeleteCareerHonorAsync(Guid id);
    }
}
