using System;
using System.Collections.Generic;
using FTM.Domain.Enums;

namespace FTM.Domain.Entities.Events
{
    public class FTFamilyEvent : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public EventType EventType { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string? Location { get; set; }
        public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.None;
        public Guid FTId { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? ReferenceEventId { get; set; }
        public string? Address { get; set; }
        public string? LocationName { get; set; }
        public bool IsAllDay { get; set; } = false;
        public DateTimeOffset? RecurrenceEndTime { get; set; }
        public bool IsLunar { get; set; } = false;
        
        // Event Scope
        public Guid? TargetMemberId { get; set; }  // null = Event của gia phả, có giá trị = Event cá nhân của member
        public bool IsPublic { get; set; } = true;  // true = Share với tất cả, false = Chỉ share với người được tag (chỉ áp dụng khi TargetMemberId = null)

        // Navigation properties
        public virtual FamilyTree.FamilyTree FT { get; set; }
        public virtual FamilyTree.FTMember? TargetMember { get; set; }
        public virtual ICollection<FTFamilyEventMember> EventMembers { get; set; } = new List<FTFamilyEventMember>();
        public virtual ICollection<FTFamilyEventFT> EventFTs { get; set; } = new List<FTFamilyEventFT>();
    }
}
