using System;
using System.ComponentModel.DataAnnotations;
using FTM.Domain.Entities.Identity;

namespace FTM.Domain.Entities.Applications
{
    public class Education : BaseEntity
    {
    // Link directly to the user (detached from Biography)
    [Required]
    public Guid UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string InstitutionName { get; set; } = null!;

        [MaxLength(200)]
        public string? Major { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; } = false;

        [MaxLength(5000)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        // Navigation
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
