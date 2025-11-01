using System;

namespace ASM_Repositories.Models.AuditDTO
{
    public class ViewAudit
    {
        public Guid AuditId { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string Scope { get; set; }

        public Guid? TemplateId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Status { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsPublished { get; set; }

        public string Objective { get; set; }
    }
}
