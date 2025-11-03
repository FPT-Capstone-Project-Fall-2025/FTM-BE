using FTM.Domain.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class UpdateFTMemberRequest
    {
        [Required]
        [FromForm(Name = "ftMemberId")]
        public Guid ftMemberId { get; set; }

        [FromForm(Name = "userId")]
        public Guid? UserId { get; set; }

        [JsonConverter(typeof(StringSensitive))]
        [FromForm(Name = "fullname")]
        public string? Fullname { get; set; }

        [FromForm(Name = "gender")]
        public int? Gender { get; set; }

        [FromForm(Name = "birthday")]
        public DateTime? Birthday { get; set; }

        [FromForm(Name = "isDeath")]
        public bool? IsDeath { get; set; }

        [FromForm(Name = "isDivorced")]
        public bool? IsDivorced { get; set; }

        [JsonConverter(typeof(StringSensitive))]
        [FromForm(Name = "deathDescription")]
        public string? DeathDescription { get; set; }

        [FromForm(Name = "deathDate")]
        public DateTime? DeathDate { get; set; }

        [JsonConverter(typeof(StringSensitive))]
        [FromForm(Name = "burialAddress")]
        public string? BurialAddress { get; set; }

        [FromForm(Name = "burialWardId")]
        public Guid? BurialWardId { get; set; }

        [FromForm(Name = "burialProvinceId")]
        public Guid? BurialProvinceId { get; set; }

        [FromForm(Name = "identificationType")]
        public string? IdentificationType { get; set; }

        [JsonConverter(typeof(StringSensitive))]
        [FromForm(Name = "identificationNumber")]
        public string? IdentificationNumber { get; set; }

        [FromForm(Name = "ethnicId")]
        public Guid? EthnicId { get; set; }

        [FromForm(Name = "religionId")]
        public Guid? ReligionId { get; set; }

        [JsonConverter(typeof(StringSensitive))]
        [FromForm(Name = "address")]
        public string? Address { get; set; }

        [FromForm(Name = "wardId")]
        public Guid? WardId { get; set; }

        [FromForm(Name = "provinceId")]
        public Guid? ProvinceId { get; set; }

        [AllowNull]
        [JsonConverter(typeof(EmailSensitive))]
        [EmailAddress]
        [FromForm(Name = "email")]
        public string? Email { get; set; }

        [AllowNull]
        [JsonConverter(typeof(PhoneSensitive))]
        [Phone]
        [FromForm(Name = "phoneNumber")]
        public string? PhoneNumber { get; set; }

        [FromForm(Name = "content")]
        public string? Content { get; set; }

        [FromForm(Name = "storyDescription")]
        public string? StoryDescription { get; set; }

        [FromForm(Name = "fTMemberFiles")]
        public List<FTMemberFileRequest>? FTMemberFiles { get; set; }

        [FromForm(Name = "avatar")]
        public IFormFile? File { get; set; }
    }
}
