using ASM_Repositories.Models.ChecklistItemDTO;
using ASM_Repositories.Models.UsersDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.FindingDTO
{
    public class ViewFindingDetail
    {
        public Guid FindingId { get; set; }

        public Guid AuditId { get; set; }

        public ViewChecklistItem AuditItem { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Severity { get; set; }

        public int? RootCauseId { get; set; }

        public int? DeptId { get; set; }

        public ViewUser CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }

        public DateTime? Deadline { get; set; }

        public ViewUser ReviewerByUser { get; set; }

        public string Source { get; set; }

        public string ExternalAuditorName { get; set; }
    }
}
