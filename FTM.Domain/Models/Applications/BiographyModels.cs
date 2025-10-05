using System.ComponentModel.DataAnnotations;

namespace FTM.Domain.Models.Applications
{
    // Biography Description Models
    public class UpdateBiographyDescriptionRequest
    {
        [MaxLength(5000, ErrorMessage = "Mô tả tiểu sử không được vượt quá 5000 ký tự")]
        public string? Description { get; set; }
    }

    public class BiographyDescriptionResponse
    {
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // Biography Events Models
    public class CreateBiographyEventRequest
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [MaxLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = null!;
        
        [MaxLength(5000, ErrorMessage = "Mô tả không được vượt quá 5000 ký tự")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Ngày sự kiện là bắt buộc")]
        public DateTime EventDate { get; set; }
    }

    public class UpdateBiographyEventRequest
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [MaxLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = null!;
        
        [MaxLength(5000, ErrorMessage = "Mô tả không được vượt quá 5000 ký tự")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Ngày sự kiện là bắt buộc")]
        public DateTime EventDate { get; set; }
    }

    public class BiographyEventResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}