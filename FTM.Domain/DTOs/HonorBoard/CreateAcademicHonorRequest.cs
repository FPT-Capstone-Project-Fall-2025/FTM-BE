using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FTM.Domain.DTOs.HonorBoard
{
    public class CreateAcademicHonorRequest
    {
        [Required(ErrorMessage = "GPMemberId is required")]
        public Guid GPMemberId { get; set; }

        [Required(ErrorMessage = "FamilyTreeId is required")]
        public Guid FamilyTreeId { get; set; }

        [Required(ErrorMessage = "Achievement title is required")]
        [MaxLength(300, ErrorMessage = "Achievement title cannot exceed 300 characters")]
        public string AchievementTitle { get; set; } = null!;

        [Required(ErrorMessage = "Institution name is required")]
        [MaxLength(300, ErrorMessage = "Institution name cannot exceed 300 characters")]
        public string InstitutionName { get; set; } = null!;

        [MaxLength(200, ErrorMessage = "Degree or certificate cannot exceed 200 characters")]
        public string? DegreeOrCertificate { get; set; }

        [Required(ErrorMessage = "Year of achievement is required")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int YearOfAchievement { get; set; }

        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Photo or certificate file to upload
        /// </summary>
        public IFormFile? Photo { get; set; }

        public bool IsDisplayed { get; set; } = true;
    }
}
