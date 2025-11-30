using System;

namespace FTM.Domain.DTOs.HonorBoard
{
    public class AcademicHonorDto
    {
        public Guid Id { get; set; }
        public Guid GPMemberId { get; set; }
        public string MemberFullName { get; set; } = null!;
        public string? MemberPhotoUrl { get; set; }
        public Guid FamilyTreeId { get; set; }
        public string AchievementTitle { get; set; } = null!;
        public string InstitutionName { get; set; } = null!;
        public string? DegreeOrCertificate { get; set; }
        public int YearOfAchievement { get; set; }
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsDisplayed { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? LastModifiedOn { get; set; }
    }
}
