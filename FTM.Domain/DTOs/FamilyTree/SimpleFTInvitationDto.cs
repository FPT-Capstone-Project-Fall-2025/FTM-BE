using FTM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class SimpleFTInvitationDto
    {
        public Guid FTId { get; set; }

        public string FTName { get; set; }

        public Guid? FTMemberId { get; set; }

        public string? FTMemberName { get; set; }

        public string Email { get; set; } = null!;

        public Guid InviterUserId { get; set; }

        public string InviterName { get; set; }

        public Guid InvitedUserId { get; set; }

        public string InvitedName { get; set; }

        public InvitationStatus Status { get; set; }

        public DateTime ExpirationDate { get; set; }

        public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.Now;
    }
}
