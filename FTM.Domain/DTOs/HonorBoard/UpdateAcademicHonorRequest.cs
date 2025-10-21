using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FTM.Domain.DTOs.HonorBoard
{
    public class UpdateAcademicHonorRequest
    {
        [MaxLength(300, ErrorMessage = "Achievement title cannot exceed 300 characters")]
        public string? AchievementTitle { get; set; }

        [MaxLength(300, ErrorMessage = "Institution name cannot exceed 300 characters")]
        public string? InstitutionName { get; set; }

        [MaxLength(200, ErrorMessage = "Degree or certificate cannot exceed 200 characters")]
        public string? DegreeOrCertificate { get; set; }

        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int? YearOfAchievement { get; set; }

        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// New photo or certificate file to upload (optional - if provided, will replace existing)
        /// </summary>
        public IFormFile? Photo { get; set; }

        public bool? IsDisplayed { get; set; }
    }
}
