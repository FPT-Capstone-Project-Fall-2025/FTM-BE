using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class UpdateFTFamilyEventRequest
    {
        public string? Name { get; set; }
        public int? EventType { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string? Location { get; set; }
        public int? RecurrenceType { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; } // File upload for event image
        public Guid? ReferenceEventId { get; set; }
        public string? Address { get; set; }
        public string? LocationName { get; set; }
        public bool? IsAllDay { get; set; }
        public DateTimeOffset? RecurrenceEndTime { get; set; }
        public bool? IsLunar { get; set; }
        public Guid? TargetMemberId { get; set; }
        public bool? IsPublic { get; set; }
        public List<Guid>? MemberIds { get; set; }
    }
}