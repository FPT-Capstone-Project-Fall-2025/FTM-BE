using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class GuestInvitationDto
    {
        [Required(ErrorMessage = "FTId is required")]
        public Guid FTId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string InvitedUserEmail { get; set; }
    }
}
