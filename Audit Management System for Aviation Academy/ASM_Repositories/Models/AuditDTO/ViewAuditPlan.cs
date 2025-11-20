using ASM_Repositories.Models.AuditCriteriaMapDTO;
using ASM_Repositories.Models.AuditCriterionDTO;
using ASM_Repositories.Models.AuditScheduleDTO;
using ASM_Repositories.Models.AuditScopeDepartmentDTO;
using ASM_Repositories.Models.AuditTeamDTO;
using ASM_Repositories.Models.UsersDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditDTO
{
    public class ViewAuditPlan
    {
        public ViewAudit Audit { get; set; }
        public ViewUser CreatedBy { get; set; }
        public List<ViewAuditScopeDepartment> ScopeDepartments { get; set; }
        public List<ViewAuditCriteriaMap> Criteria { get; set; }
        public List<ViewAuditTeam> AuditTeams { get; set; }
        public List<ViewAuditSchedule> Schedules { get; set; }

    }
}
