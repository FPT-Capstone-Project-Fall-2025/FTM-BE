using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class UpsertFamilyTreeRequest
    {
        [StringLength(255, ErrorMessage = "Tên gia phả không được vượt quá 255 ký tự")]
        public string? Name { get; set; }
        public Guid? OwnerId { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        public IFormFile? File { get; set; }
        public int? GPModeCode { get; set; }
    }
}