using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace FTM.Domain.Models.Authen
{
    public class UpdateAvatarRequest
    {
        [Required(ErrorMessage = "File ảnh là bắt buộc")]
        public IFormFile Avatar { get; set; }
    }

    public class UpdateAvatarResponse
    {
        public string AvatarUrl { get; set; }
        public string Message { get; set; }
    }
}