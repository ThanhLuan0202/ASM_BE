using ASM_Repositories.Models.AuditCriteriaMapDTO;
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
    public class UpdateAuditPlan
    {
        public UpdateAudit? Audit { get; set; }
        public List<UpdateAuditScopeDepartment>? ScopeDepartments { get; set; }
        public List<UpdateAuditCriteriaMap>? Criteria { get; set; }
        public List<UpdateAuditTeam>? AuditTeams { get; set; }
        public List<UpdateAuditSchedule>? Schedules { get; set; }
    }
}
