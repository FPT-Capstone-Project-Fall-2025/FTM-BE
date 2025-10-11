using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FTM.Domain.Entities.Identity;

namespace FTM.Domain.Entities.Applications
{
	public class WorkExperience : BaseEntity
	{
		// Link directly to the user (detached from Biography)
		[Required]
		public Guid UserId { get; set; }

		[Required]
		[MaxLength(200)]
		public string CompanyName { get; set; } = null!;

		[MaxLength(5000)]
		public string? Description { get; set; }

		[MaxLength(200)]
		public string? Location { get; set; }

		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public bool IsCurrent { get; set; } = false;

		// Navigation
		public virtual ApplicationUser User { get; set; } = null!;
		public virtual ICollection<WorkPosition> Positions { get; set; } = new List<WorkPosition>();
	}
}
