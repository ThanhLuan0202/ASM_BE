using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditPlanAssignmentDTO
{
    public class UpdateAuditPlanAssignment
    {
        public int? AuditorId { get; set; }
        public int? AssignBy { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string Remarks { get; set; }
    }
}
