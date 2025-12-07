using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.FindingDTO
{
    public class ViewFinding
    {
        public Guid FindingId { get; set; }

        public Guid AuditId { get; set; }

        public Guid? AuditItemId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Severity { get; set; }

        public int? RootCauseId { get; set; }

        public int? DeptId { get; set; }

        public Guid WitnessId { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }

        public DateTime? Deadline { get; set; }

        public Guid? ReviewerId { get; set; }

        public string Source { get; set; }

        public string ExternalAuditorName { get; set; }
    }
}
