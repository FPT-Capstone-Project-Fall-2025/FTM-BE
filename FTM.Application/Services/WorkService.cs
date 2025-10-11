using FTM.Application.IServices;
using FTM.Domain.Entities.Applications;
using FTM.Domain.Models.Applications;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FTM.Application.Services
{
    public class WorkService : IWorkService
    {
        private readonly IWorkRepository _workRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WorkService(IWorkRepository workRepository, IHttpContextAccessor httpContextAccessor)
        {
            _workRepository = workRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private Guid GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("User not authenticated");
            if (!Guid.TryParse(userId, out var guidUserId)) throw new UnauthorizedAccessException("Invalid user ID format");
            return guidUserId;
        }

        public async Task<IEnumerable<WorkResponse>> GetCurrentUserWorkAsync()
        {
            var userId = GetCurrentUserId();
            var works = await _workRepository.GetWorkByUserIdAsync(userId);
            return works.Select(w => new WorkResponse
            {
                Id = w.Id,
                CompanyName = w.CompanyName,
                Description = w.Description,
                Location = w.Location,
                StartDate = w.StartDate,
                EndDate = w.EndDate,
                IsCurrent = w.IsCurrent,
                Positions = w.Positions?.Select(p => new WorkPositionResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Description = p.Description
                }).ToList() ?? new List<WorkPositionResponse>(),
                CreatedAt = w.CreatedOn,
                UpdatedAt = w.LastModifiedOn
            });
        }

        public async Task<WorkResponse?> GetWorkByIdAsync(Guid workId)
        {
            var userId = GetCurrentUserId();
            var work = await _workRepository.GetWorkByIdAsync(workId);
            if (work == null || work.UserId != userId) return null;
            return new WorkResponse
            {
                Id = work.Id,
                CompanyName = work.CompanyName,
                Description = work.Description,
                Location = work.Location,
                StartDate = work.StartDate,
                EndDate = work.EndDate,
                IsCurrent = work.IsCurrent,
                Positions = work.Positions?.Select(p => new WorkPositionResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Description = p.Description
                }).ToList() ?? new List<WorkPositionResponse>(),
                CreatedAt = work.CreatedOn,
                UpdatedAt = work.LastModifiedOn
            };
        }

        public async Task<WorkResponse> CreateWorkAsync(CreateWorkRequest request)
        {
            var userId = GetCurrentUserId();
            var work = new WorkExperience
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CompanyName = request.CompanyName,
                Description = request.Description,
                Location = request.Location,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsCurrent = request.IsCurrent,
            };

            if (request.Positions != null && request.Positions.Any())
            {
                work.Positions = request.Positions.Select(p => new WorkPosition
                {
                    Id = Guid.NewGuid(),
                    Title = p.Title,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Description = p.Description
                }).ToList();
            }

            var created = await _workRepository.CreateWorkAsync(work);

            return new WorkResponse
            {
                Id = created.Id,
                CompanyName = created.CompanyName,
                Description = created.Description,
                Location = created.Location,
                StartDate = created.StartDate,
                EndDate = created.EndDate,
                IsCurrent = created.IsCurrent,
                Positions = created.Positions?.Select(p => new WorkPositionResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Description = p.Description
                }).ToList() ?? new List<WorkPositionResponse>(),
                CreatedAt = created.CreatedOn,
                UpdatedAt = created.LastModifiedOn
            };
        }

        public async Task<WorkResponse?> UpdateWorkAsync(Guid workId, UpdateWorkRequest request)
        {
            var userId = GetCurrentUserId();
            var existing = await _workRepository.GetWorkByIdAsync(workId);
            if (existing == null || existing.UserId != userId) return null;

            // Update work experience basic info
            existing.CompanyName = request.CompanyName;
            existing.Description = request.Description;
            existing.Location = request.Location;
            existing.StartDate = request.StartDate;
            existing.EndDate = request.EndDate;
            existing.IsCurrent = request.IsCurrent;

            // Smart position management
            var existingPositions = existing.Positions ?? new List<WorkPosition>();
            var requestPositions = request.Positions ?? new List<UpdateWorkPositionRequest>();

            // Get IDs from request
            var requestPositionIds = requestPositions
                .Where(p => p.Id.HasValue)
                .Select(p => p.Id!.Value)
                .ToHashSet();

            var existingPositionIds = existingPositions.Select(p => p.Id).ToHashSet();

            // 1. DELETE: Positions that exist in DB but not in request
            var positionsToDelete = existingPositions
                .Where(p => !requestPositionIds.Contains(p.Id))
                .ToList();

            foreach (var position in positionsToDelete)
            {
                await _workRepository.DeletePositionAsync(position.Id);
            }

            // 2. UPDATE or ADD positions from request
            foreach (var reqPosition in requestPositions)
            {
                if (reqPosition.Id.HasValue && existingPositionIds.Contains(reqPosition.Id.Value))
                {
                    // UPDATE existing position
                    var existingPosition = existingPositions.First(p => p.Id == reqPosition.Id.Value);
                    
                    // Check if there are any changes
                    if (existingPosition.Title != reqPosition.Title ||
                        existingPosition.StartDate != reqPosition.StartDate ||
                        existingPosition.EndDate != reqPosition.EndDate ||
                        existingPosition.Description != reqPosition.Description)
                    {
                        existingPosition.Title = reqPosition.Title;
                        existingPosition.StartDate = reqPosition.StartDate;
                        existingPosition.EndDate = reqPosition.EndDate;
                        existingPosition.Description = reqPosition.Description;
                        await _workRepository.UpdatePositionAsync(existingPosition);
                    }
                }
                else
                {
                    // ADD new position (Id is null or not found in existing)
                    var newPosition = new WorkPosition
                    {
                        Id = Guid.NewGuid(),
                        WorkExperienceId = workId,
                        Title = reqPosition.Title,
                        StartDate = reqPosition.StartDate,
                        EndDate = reqPosition.EndDate,
                        Description = reqPosition.Description
                    };
                    await _workRepository.AddPositionAsync(newPosition);
                }
            }

            // Reload work experience with updated positions
            var updated = await _workRepository.GetWorkByIdAsync(workId);

            return new WorkResponse
            {
                Id = updated!.Id,
                CompanyName = updated.CompanyName,
                Description = updated.Description,
                Location = updated.Location,
                StartDate = updated.StartDate,
                EndDate = updated.EndDate,
                IsCurrent = updated.IsCurrent,
                Positions = updated.Positions?.Select(p => new WorkPositionResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Description = p.Description
                }).ToList() ?? new List<WorkPositionResponse>(),
                CreatedAt = updated.CreatedOn,
                UpdatedAt = updated.LastModifiedOn
            };
        }

        public async Task<bool> DeleteWorkAsync(Guid workId)
        {
            var userId = GetCurrentUserId();
            var owns = await _workRepository.UserOwnsWorkAsync(userId, workId);
            if (!owns) return false;
            return await _workRepository.DeleteWorkAsync(workId);
        }

        public async Task<WorkPositionResponse> AddPositionToWorkAsync(Guid workId, CreateWorkPositionRequest request)
        {
            var userId = GetCurrentUserId();
            var work = await _workRepository.GetWorkByIdAsync(workId);
            if (work == null || work.UserId != userId)
                throw new UnauthorizedAccessException("Work experience not found or access denied");

            var position = new WorkPosition
            {
                Id = Guid.NewGuid(),
                WorkExperienceId = workId,
                Title = request.Title,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Description = request.Description
            };

            await _workRepository.AddPositionAsync(position);

            return new WorkPositionResponse
            {
                Id = position.Id,
                Title = position.Title,
                StartDate = position.StartDate,
                EndDate = position.EndDate,
                Description = position.Description
            };
        }

        public async Task<WorkPositionResponse?> UpdatePositionAsync(Guid workId, Guid positionId, UpdateWorkPositionRequest request)
        {
            var userId = GetCurrentUserId();
            var work = await _workRepository.GetWorkByIdAsync(workId);
            if (work == null || work.UserId != userId) return null;

            var position = work.Positions?.FirstOrDefault(p => p.Id == positionId);
            if (position == null) return null;

            position.Title = request.Title;
            position.StartDate = request.StartDate;
            position.EndDate = request.EndDate;
            position.Description = request.Description;

            await _workRepository.UpdatePositionAsync(position);

            return new WorkPositionResponse
            {
                Id = position.Id,
                Title = position.Title,
                StartDate = position.StartDate,
                EndDate = position.EndDate,
                Description = position.Description
            };
        }

        public async Task<bool> DeletePositionAsync(Guid workId, Guid positionId)
        {
            var userId = GetCurrentUserId();
            var work = await _workRepository.GetWorkByIdAsync(workId);
            if (work == null || work.UserId != userId) return false;

            var position = work.Positions?.FirstOrDefault(p => p.Id == positionId);
            if (position == null) return false;

            return await _workRepository.DeletePositionAsync(positionId);
        }
    }
}
