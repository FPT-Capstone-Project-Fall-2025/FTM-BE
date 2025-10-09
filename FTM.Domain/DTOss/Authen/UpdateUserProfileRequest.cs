using System;
using System.ComponentModel.DataAnnotations;

namespace FTM.Domain.DTOs.Authen
{
    public class UpdateUserProfileRequest
    {
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string? Name { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        public string? PhoneNumber { get; set; }

        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự")]
        public string? Address { get; set; }

        [StringLength(50, ErrorMessage = "Biệt danh không được vượt quá 50 ký tự")]
        public string? Nickname { get; set; }

        public DateTime? Birthday { get; set; }

        [StringLength(100, ErrorMessage = "Nghề nghiệp không được vượt quá 100 ký tự")]
        public string? Job { get; set; }

        [Range(0, 1, ErrorMessage = "Giới tính phải là: 0 (Nam), 1 (Nữ)")]
        public int? Gender { get; set; }

        public Guid? ProvinceId { get; set; }

        public Guid? WardId { get; set; }
    }
}