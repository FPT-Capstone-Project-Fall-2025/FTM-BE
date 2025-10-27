using System;
using System.Collections.Generic;
using FTM.Domain.Enums;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class FTFamilyEventDto
    {
        public Guid Id { get; set; }
    public string? Name { get; set; }
        public EventType EventType { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public string? Location { get; set; }
        public RecurrenceType RecurrenceType { get; set; }
        public Guid FTId { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? ReferenceEventId { get; set; }
        public string? Address { get; set; }
        public string? LocationName { get; set; }
        public bool IsAllDay { get; set; }
        public DateTimeOffset? RecurrenceEndTime { get; set; }
        public bool IsLunar { get; set; }
        public Guid? TargetMemberId { get; set; }
        public string? TargetMemberName { get; set; }
        public bool IsPublic { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastModifiedOn { get; set; }
        
        public List<FTFamilyEventMemberDto> EventMembers { get; set; } = new List<FTFamilyEventMemberDto>();
    }

    public class FTFamilyEventMemberDto
    {
        public Guid Id { get; set; }
        public Guid FTMemberId { get; set; }
    public string? MemberName { get; set; }
        public string? MemberPicture { get; set; }
        public Guid? UserId { get; set; }
    }

    public class AddMemberToEventRequest
    {
        public Guid EventId { get; set; }
        public Guid MemberId { get; set; }
    }
}
