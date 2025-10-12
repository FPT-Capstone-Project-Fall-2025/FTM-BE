using System;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FTMemberDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid FTId { get; set; }
        public string Fullname { get; set; }
        public int Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public int StatusCode { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Picture { get; set; }
        public string Content { get; set; }
        public Guid? EthnicId { get; set; }
        public Guid? ReligionId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? ProvinceId { get; set; }
        public string StoryDescription { get; set; }
        public string IdentificationNumber { get; set; }
        public string IdentificationType { get; set; }
        public bool? IsDeath { get; set; }
        public string DeathDescription { get; set; }
        public DateTime? DeathDate { get; set; }
        public string BurialAddress { get; set; }
        public Guid? BurialWardId { get; set; }
        public Guid? BurialProvinceId { get; set; }
        public string PrivacyData { get; set; }
        public bool IsRoot { get; set; }
    }
}