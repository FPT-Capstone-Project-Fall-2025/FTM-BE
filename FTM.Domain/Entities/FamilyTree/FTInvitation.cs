using FTM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities.FamilyTree
{
    public class FTInvitation : BaseEntity
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

        public string Token { get; set; } = null!; 

        public DateTime ExpirationDate { get; set; }

        public InvitationStatus Status { get; set; } = InvitationStatus.PENDING;

        public virtual FamilyTree FT { get; set; }
        public virtual FTMember FTMember { get; set; }
    }
}
