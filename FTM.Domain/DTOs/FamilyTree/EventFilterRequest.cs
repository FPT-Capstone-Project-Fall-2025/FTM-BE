using FTM.Domain.Enums;
using System;

namespace FTM.Domain.DTOs.FamilyTree
{
    public class EventFilterRequest
    {
        public Guid FTId { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public Guid? FTMemberId { get; set; }
        public string EventType { get; set; }
        public string SearchTerm { get; set; }
        public bool? IsLunar { get; set; }
        public int? Skip { get; set; }
        public int? Take { get; set; }
    }
}