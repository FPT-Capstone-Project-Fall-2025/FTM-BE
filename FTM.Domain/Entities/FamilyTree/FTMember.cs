using FTM.Domain.Entities.Applications;
using FTM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Entities.FamilyTree
{
    public class FTMember : BaseEntity
    {
        public Guid? UserId { get; set; }
        public Guid FTId { get; set; }
        public FTMRole FTRole { get; set; }
        public string Fullname { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public int StatusCode { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Picture { get; set; }
        public string? Content { get; set; }
        public Guid? EthnicId { get; set; }
        public Guid? ReligionId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? ProvinceId { get; set; }
        public string? StoryDescription { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? IdentificationType { get; set; }

        public bool? IsDeath { get; set; }
        public string? DeathDescription { get; set; }
        public DateTime? DeathDate { get; set; }
        public string? BurialAddress { get; set; }
        public Guid? BurialWardId { get; set; }
        public Guid? BurialProvinceId { get; set; }
        public string? PrivacyData { get; set; }

        public bool IsRoot { get; set; } = false;
        public bool IsDivorced { get; set; } = false;

        public virtual MEthnic Ethnic { get; set; }
        public virtual MReligion Religion { get; set; }
        public virtual MWard Ward { get; set; }
        public virtual Mprovince Province { get; set; }

        public virtual MWard BurialWard { get; set; }
        public virtual Mprovince BurialProvince { get; set; }

        public virtual FamilyTree FT { get; set; }

        public virtual ICollection<FTInvitation> FTInvitations { get; set; } = new List<FTInvitation>();
        public virtual ICollection<FTMemberFile> FTMemberFiles { get; set; } = new List<FTMemberFile>();
        public virtual ICollection<FTRelationship> FTRelationshipFrom { get; set; } = new List<FTRelationship>();
        public virtual ICollection<FTRelationship> FTRelationshipFromPartner { get; set; } = new List<FTRelationship>();
        public virtual ICollection<FTRelationship> FTRelationshipTo { get; set; } = new List<FTRelationship>();
        public virtual ICollection<FTAuthorization> FTAuthorizations { get; set; } = new List<FTAuthorization>();
    }
}
