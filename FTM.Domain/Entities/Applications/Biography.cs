using FTM.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace FTM.Domain.Entities.Applications
{
    public class Biography : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = null!; // "Description" hoặc "Event"
        
        [MaxLength(200)]
        public string? Title { get; set; } // Chỉ dùng cho Event
        
        [MaxLength(5000)]
        public string? Description { get; set; }
        
        public DateTime? EventDate { get; set; } // Chỉ dùng cho Event
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual ApplicationUser User { get; set; } = null!;
    }
}