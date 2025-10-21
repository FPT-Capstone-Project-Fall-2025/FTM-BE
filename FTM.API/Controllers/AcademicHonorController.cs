using System;
using System.Threading.Tasks;
using FTM.API.Helpers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.HonorBoard;
using FTM.Domain.Specification.HonorBoard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AcademicHonorController : ControllerBase
    {
        private readonly IAcademicHonorService _academicHonorService;

        public AcademicHonorController(IAcademicHonorService academicHonorService)
        {
            _academicHonorService = academicHonorService;
        }

        /// <summary>
        /// Get academic honors with pagination and filtering
        /// </summary>
        /// <param name="familyTreeId">Filter by family tree ID</param>
        /// <param name="memberId">Filter by member ID</param>
        /// <param name="year">Filter by year of achievement</param>
        /// <param name="institutionName">Filter by institution name (partial match)</param>
        /// <param name="isDisplayed">Filter by display status</param>
        /// <param name="requestParams">Pagination parameters</param>
        [HttpGet]
        public async Task<IActionResult> GetAcademicHonors(
            [FromQuery] Guid? familyTreeId,
            [FromQuery] Guid? memberId,
            [FromQuery] int? year,
            [FromQuery] string? institutionName,
            [FromQuery] bool? isDisplayed,
            [FromQuery] SearchWithPaginationRequest requestParams)
        {
            try
            {
                var specParams = new AcademicHonorSpecParams
                {
                    FamilyTreeId = familyTreeId,
                    GPMemberId = memberId,
                    Year = year,
                    InstitutionName = institutionName,
                    IsDisplayed = isDisplayed,
                    Skip = (requestParams.PageIndex - 1) * requestParams.PageSize,
                    Take = requestParams.PageSize
                };

                var result = await _academicHonorService.GetAcademicHonorsAsync(specParams);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get academic honor by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAcademicHonorById(Guid id)
        {
            try
            {
                var result = await _academicHonorService.GetAcademicHonorByIdAsync(id);
                if (result == null)
                    return NotFound(new ApiError("Academic honor not found"));
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Create a new academic honor
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAcademicHonor([FromForm] CreateAcademicHonorRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiError("Invalid request data"));

                var result = await _academicHonorService.CreateAcademicHonorAsync(request);
                return CreatedAtAction(nameof(GetAcademicHonorById), new { id = result.Id }, new ApiSuccess(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Update an existing academic honor
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAcademicHonor(Guid id, [FromForm] UpdateAcademicHonorRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiError("Invalid request data"));

                var result = await _academicHonorService.UpdateAcademicHonorAsync(id, request);
                if (result == null)
                    return NotFound(new ApiError("Academic honor not found"));
                return Ok(new ApiSuccess(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Delete an academic honor (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAcademicHonor(Guid id)
        {
            try
            {
                var result = await _academicHonorService.DeleteAcademicHonorAsync(id);
                if (!result)
                    return NotFound(new ApiError("Academic honor not found"));
                return Ok(new ApiSuccess("Academic honor deleted successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }
    }
}
