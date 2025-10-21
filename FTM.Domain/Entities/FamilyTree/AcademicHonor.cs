using System;
using System.ComponentModel.DataAnnotations;

namespace FTM.Domain.Entities.FamilyTree
{
    /// <summary>
    /// Represents an academic achievement or honor awarded to a family tree member
    /// </summary>
    public class AcademicHonor : BaseEntity
    {
        /// <summary>
        /// Foreign key to the family tree member who earned this honor
        /// </summary>
        [Required]
        public Guid GPMemberId { get; set; }

        /// <summary>
        /// Foreign key to the family tree this honor belongs to
        /// </summary>
        [Required]
        public Guid FamilyTreeId { get; set; }

        /// <summary>
        /// Title or name of the academic achievement (e.g., "Valedictorian", "PhD in Computer Science")
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string AchievementTitle { get; set; } = null!;

        /// <summary>
        /// Name of the educational institution
        /// </summary>
        [Required]
        [MaxLength(300)]
        public string InstitutionName { get; set; } = null!;

        /// <summary>
        /// Degree, certificate, or qualification obtained
        /// </summary>
        [MaxLength(200)]
        public string? DegreeOrCertificate { get; set; }

        /// <summary>
        /// Year when the achievement was earned
        /// </summary>
        [Required]
        public int YearOfAchievement { get; set; }

        /// <summary>
        /// Detailed description or notes about the achievement
        /// </summary>
        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// URL to photo or certificate image
        /// </summary>
        [MaxLength(500)]
        public string? PhotoUrl { get; set; }

        /// <summary>
        /// Whether this honor is currently displayed on the honor board
        /// </summary>
        public bool IsDisplayed { get; set; } = true;

        // Navigation properties
        public virtual FTMember GPMember { get; set; } = null!;
        public virtual FamilyTree FamilyTree { get; set; } = null!;
    }
}
