using System;
using System.ComponentModel.DataAnnotations;

namespace FTM.Domain.Entities.Applications
{
    public class WorkPosition : BaseEntity
    {
        [Required]
        public Guid WorkExperienceId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        // Navigation
        public virtual WorkExperience WorkExperience { get; set; } = null!;
    }
}
