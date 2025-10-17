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

        public string CreatedBy { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTimeOffset? LastModifiedOn { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsRoot { get; set; }

        public string Fullname { get; set; }
        public int? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public int StatusCode { get; set; }

        public bool? IsDeath { get; set; }
        public string DeathDescription { get; set; }
        public DateTime? DeathDate { get; set; }
        public string BurialAddress { get; set; }
        public string BurialProvinceCode { get; set; }
        public string BurialWardCode { get; set; }

        public string IdentificationType { get; set; }
        public string IdentificationTypeName { get; set; }
        public string IdentificationNumber { get; set; }
        public string EthnicCode { get; set; }
        public string ReligionCode { get; set; }

        public string Address { get; set; }
        public string ProvinceCode { get; set; }
        public string WardCode { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Picture { get; set; }

        public string Content { get; set; }

        public string StoryDescription { get; set; }

        public Guid? UserId { get; set; }

        //public List<KeyValueModel> Roles { get; set; }

        //public List<MemberPrivacyItemRequest> PrivacyData { get; set; }

        //public Guid? GPMemberParentId { get; set; }
        //public GPDetailsDto GPMemberParent { get; set; }

        //public Guid GPId { get; set; }
        //public GPDetailsDto GP { get; set; }

        //public MasterDataModel Ethnic { get; set; }
        //public MasterDataModel Religion { get; set; }
        //public MasterDataModel City { get; set; }
        //public MasterDataModel Ward { get; set; }
        //public MasterDataModel District { get; set; }

        //public MasterDataModel BurialCity { get; set; }
        //public MasterDataModel BurialWard { get; set; }
        //public MasterDataModel BurialDistrict { get; set; }

        //public List<GPMemberFileDetailsDto> GPMemberFiles { get; set; } = new List<GPMemberFileDetailsDto>();
        //public List<GPRelationshipDetailsDto> GPRelationshipFrom { get; set; } = new List<GPRelationshipDetailsDto>();
        //public List<GPRelationshipDetailsDto> GPRelationshipTo { get; set; } = new List<GPRelationshipDetailsDto>();
    }
}
