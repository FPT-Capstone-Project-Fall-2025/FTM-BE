using FTM.Domain.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class UpsertFTMemberRequest
    {
        [Required]
        [JsonConverter(typeof(StringSensitive))]
        public string Fullname { get; set; }

        public int? Gender { get; set; }

        public DateTime? Birthday { get; set; }
        public bool? IsDeath { get; set; }

        [JsonConverter(typeof(StringSensitive))]
        public string DeathDescription { get; set; }
        public DateTime? DeathDate { get; set; }

        [JsonConverter(typeof(StringSensitive))]
        public string BurialAddress { get; set; }
        public Guid? BurialWardId { get; set; }
        public Guid? BurialProvinceId { get; set; }

        public string IdentificationType { get; set; }

        [JsonConverter(typeof(StringSensitive))]
        public string IdentificationNumber { get; set; }
        public Guid? EthnicId { get; set; }
        public Guid? ReligionId { get; set; }
        public int CategoryCode { get; set; }

        [JsonConverter(typeof(StringSensitive))]
        public string Address { get; set; }
        public Guid? WardId { get; set; }
        public Guid? ProvinceId { get; set; }

        [AllowNull]
        [JsonConverter(typeof(EmailSensitive))]
        public string Email { get; set; }

        [AllowNull]
        [JsonConverter(typeof(PhoneSensitive))]
        public string PhoneNumber { get; set; }

        public string Picture { get; set; }

        public string Content { get; set; }

        public string? StoryDescription { get; set; }

        public Guid? FTMemberParentId { get; set; }

        public Guid FTId { get; set; }

        public Guid? RootId { get; set; }
        public Guid? FromFTMemberId { get; set; }
        public Guid? FromFTMemberPartnerId { get; set; }
        public List<FTMemberFileRequest>? FTMemberFiles { get; set; }
       
        //public UpsertFTRelationshipRequest Relationship { get; set; }
    }
}
