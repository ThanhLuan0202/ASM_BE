using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditPlanAssignmentDTO
{
    public class CreateAuditPlanAssignment
    {
        public Guid AuditorId { get; set; }
        public Guid AssignBy { get; set; } // Will be set from token in controller
        public DateTime AssignedDate { get; set; }
        public string Remarks { get; set; }
    }
}
