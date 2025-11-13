using ASM_Repositories.Models.DepartmentDTO;
using ASM_Repositories.Models.FindingDTO;
using ASM_Repositories.Models.RootCauseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditDTO
{
    public class ViewAuditSummary
    {
        public Guid AuditId { get; set; }
        public string Title { get; set; } = "";
        public string Type { get; set; } = "";
        public string Scope { get; set; } = "";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int TotalFindings { get; set; }
        public int OpenFindings { get; set; }
        public int ClosedFindings { get; set; }
        public int OverdueFindings { get; set; }

        public Dictionary<string, int> SeverityBreakdown { get; set; } = new();
        public List<ViewDepartmentCount> ByDepartment { get; set; } = new();
        public List<ViewRootCauseCount> ByRootCause { get; set; } = new();
        public List<ViewFindingByMonth> FindingsByDate { get; set; } = new();
    }
}
