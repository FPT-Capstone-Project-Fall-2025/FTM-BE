using System;

namespace FTM.Domain.Specification.HonorBoard
{
    public class AcademicHonorSpecParams : BaseSpecParams
    {
        /// <summary>
        /// Filter by Family Tree ID
        /// </summary>
        public Guid? FamilyTreeId { get; set; }

        /// <summary>
        /// Filter by GPMember ID
        /// </summary>
        public Guid? GPMemberId { get; set; }

        /// <summary>
        /// Filter by year of achievement
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Filter by institution name (partial match)
        /// </summary>
        public string? InstitutionName { get; set; }

        /// <summary>
        /// Filter by display status
        /// </summary>
        public bool? IsDisplayed { get; set; }
    }
}
