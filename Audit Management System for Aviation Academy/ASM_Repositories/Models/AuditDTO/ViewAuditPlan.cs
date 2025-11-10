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

        public ViewUser CreatedByUser { get; set; }
        public List<ViewAuditScopeDepartment> ScopeDepartments { get; set; }
        public List<ViewAuditCriterion> Criteria { get; set; }
        public List<ViewAuditTeam> AuditTeams { get; set; }
        public List<ViewAuditSchedule> Schedules { get; set; }

    }
}
