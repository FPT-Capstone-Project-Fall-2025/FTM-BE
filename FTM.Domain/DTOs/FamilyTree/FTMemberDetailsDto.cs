using FTM.Domain.Entities.Applications;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FTMemberDetailsDto
    {
        public Guid Id { get; set; }
        public Guid FTId { get; set; }
        public FTMRole FTRole { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }  

        public string LastModifiedBy { get; set; }

        public DateTimeOffset? LastModifiedOn { get; set; }

        public bool IsActive { get; set; }
        public bool IsRoot { get; set; }

        public string? PrivacyData { get; set; }

        public string Fullname { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public int StatusCode { get; set; }

        public bool? IsDeath { get; set; }
        public string? DeathDescription { get; set; }
        public DateTime? DeathDate { get; set; }
        public string? BurialAddress { get; set; }
        public Guid? BurialWardId { get; set; }
        public Guid? BurialProvinceId { get; set; }

        public string? IdentificationType { get; set; }
        public string? IdentificationNumber { get; set; }
        public Guid? EthnicId { get; set; }
        public Guid? ReligionId { get; set; }

        public string? Address { get; set; }
        public Guid? WardId { get; set; }
        public Guid? ProvinceId { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Picture { get; set; }

        public string? Content { get; set; }

        public string? StoryDescription { get; set; }

        public Guid? UserId { get; set; }

        public  MEthnicDto? Ethnic { get; set; }
        public  MReligionDto? Religion { get; set; }
        public  MWardDto? Ward { get; set; }
        public  MprovinceDto? Province { get; set; }

        public MWardDto? BurialWard { get; set; }
        public MprovinceDto? BurialProvince { get; set; }
        public  List<FTMemberFileDto> FTMemberFiles { get; set; } = new List<FTMemberFileDto>();
    }
}
