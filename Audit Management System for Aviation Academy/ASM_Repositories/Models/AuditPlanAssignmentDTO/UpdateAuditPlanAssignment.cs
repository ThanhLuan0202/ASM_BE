using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditPlanAssignmentDTO
{
    public class UpdateAuditPlanAssignment
    {
        public Guid? AuditorId { get; set; }
        public Guid? AssignBy { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string Remarks { get; set; }
    }
}
