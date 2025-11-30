using System;
using System.Collections.Generic;

namespace FTM.Domain.Models.Applications
{
    // Education DTOs
    public class CreateEducationRequest
    {
        public string InstitutionName { get; set; } = null!;
        public string? Major { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
    }

    public class UpdateEducationRequest : CreateEducationRequest { }

    public class EducationResponse
    {
        public Guid Id { get; set; }
        public string InstitutionName { get; set; } = null!;
        public string? Major { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    // Work DTOs
    public class CreateWorkRequest
    {
        public string CompanyName { get; set; } = null!;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public List<CreateWorkPositionRequest>? Positions { get; set; }
    }

    public class UpdateWorkRequest
    {
        public string CompanyName { get; set; } = null!;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public List<UpdateWorkPositionRequest>? Positions { get; set; }
    }

    public class WorkResponse
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public List<WorkPositionResponse> Positions { get; set; } = new List<WorkPositionResponse>();
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public class CreateWorkPositionRequest
    {
        public string Title { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateWorkPositionRequest
    {
        public Guid? Id { get; set; } // Optional: null = new position, has value = update existing
        public string Title { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }

    public class WorkPositionResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }
}
