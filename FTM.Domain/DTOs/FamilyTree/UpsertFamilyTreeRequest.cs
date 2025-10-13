using System;
using System.ComponentModel.DataAnnotations;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class UpsertFamilyTreeRequest
    {
        [Required(ErrorMessage = "Tên gia phả là bắt buộc")]
        [StringLength(255, ErrorMessage = "Tên gia phả không được vượt quá 255 ký tự")]
        public string Name { get; set; }
        public Guid? OwnerId { get; set; }

        [StringLength(255, ErrorMessage = "Tên chủ sở hữu không được vượt quá 255 ký tự")]
        public string Owner { get; set; }


        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string Description { get; set; }

        public string Picture { get; set; }

        public int? GPModeCode { get; set; }
    }
}