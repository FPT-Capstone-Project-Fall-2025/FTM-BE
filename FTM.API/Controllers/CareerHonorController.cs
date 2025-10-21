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
    public class CareerHonorController : ControllerBase
    {
        private readonly ICareerHonorService _careerHonorService;

        public CareerHonorController(ICareerHonorService careerHonorService)
        {
            _careerHonorService = careerHonorService;
        }

        /// <summary>
        /// Get career honors with pagination and filtering
        /// </summary>
        /// <param name="familyTreeId">Filter by family tree ID</param>
        /// <param name="memberId">Filter by member ID</param>
        /// <param name="year">Filter by year of achievement</param>
        /// <param name="organizationName">Filter by organization name (partial match)</param>
        /// <param name="isDisplayed">Filter by display status</param>
        /// <param name="requestParams">Pagination parameters</param>
        [HttpGet]
        public async Task<IActionResult> GetCareerHonors(
            [FromQuery] Guid? familyTreeId,
            [FromQuery] Guid? memberId,
            [FromQuery] int? year,
            [FromQuery] string? organizationName,
            [FromQuery] bool? isDisplayed,
            [FromQuery] SearchWithPaginationRequest requestParams)
        {
            try
            {
                var specParams = new CareerHonorSpecParams
                {
                    FamilyTreeId = familyTreeId,
                    GPMemberId = memberId,
                    Year = year,
                    OrganizationName = organizationName,
                    IsDisplayed = isDisplayed,
                    Skip = (requestParams.PageIndex - 1) * requestParams.PageSize,
                    Take = requestParams.PageSize
                };

                var result = await _careerHonorService.GetCareerHonorsAsync(specParams);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get career honor by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCareerHonorById(Guid id)
        {
            try
            {
                var result = await _careerHonorService.GetCareerHonorByIdAsync(id);
                if (result == null)
                    return NotFound(new ApiError("Career honor not found"));
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Create a new career honor
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCareerHonor([FromForm] CreateCareerHonorRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiError("Invalid request data"));

                var result = await _careerHonorService.CreateCareerHonorAsync(request);
                return CreatedAtAction(nameof(GetCareerHonorById), new { id = result.Id }, new ApiSuccess(result));
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
        /// Update an existing career honor
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCareerHonor(Guid id, [FromForm] UpdateCareerHonorRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiError("Invalid request data"));

                var result = await _careerHonorService.UpdateCareerHonorAsync(id, request);
                if (result == null)
                    return NotFound(new ApiError("Career honor not found"));
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
        /// Delete a career honor (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCareerHonor(Guid id)
        {
            try
            {
                var result = await _careerHonorService.DeleteCareerHonorAsync(id);
                if (!result)
                    return NotFound(new ApiError("Career honor not found"));
                return Ok(new ApiSuccess("Career honor deleted successfully"));
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
