using ASM_Repositories.Models.AuditChecklistItemDTO;
using ASM_Repositories.Models.AuditCriteriaMapDTO;
using ASM_Repositories.Models.AuditScheduleDTO;
using ASM_Repositories.Models.AuditScopeDepartmentDTO;
using ASM_Repositories.Models.AuditTeamDTO;
using System;
using System.Collections.Generic;

namespace ASM_Repositories.Models.AuditDTO
{
    /// <summary>
    /// DTO để update audit cùng với tất cả các entities liên quan trong một lần
    /// </summary>
    public class UpdateAuditComplete
    {
        /// <summary>
        /// Thông tin audit cần update (optional - nếu null thì không update audit)
        /// </summary>
        public UpdateAudit? Audit { get; set; }

        /// <summary>
        /// Danh sách AuditCriteriaMap cần update (optional - nếu null hoặc empty thì không update)
        /// </summary>
        public List<UpdateAuditCriteriaMap>? CriteriaMaps { get; set; }

        /// <summary>
        /// Danh sách AuditScopeDepartment cần update (optional - nếu null hoặc empty thì không update)
        /// </summary>
        public List<UpdateAuditScopeDepartment>? ScopeDepartments { get; set; }

        /// <summary>
        /// Danh sách AuditTeam cần update (optional - nếu null hoặc empty thì không update)
        /// </summary>
        public List<UpdateAuditTeam>? AuditTeams { get; set; }

        /// <summary>
        /// Danh sách AuditSchedule cần update (optional - nếu null hoặc empty thì không update)
        /// </summary>
        public List<UpdateAuditSchedule>? Schedules { get; set; }

        /// <summary>
        /// Danh sách AuditChecklistItem cần update (optional - nếu null hoặc empty thì không update)
        /// </summary>
        public List<UpdateAuditChecklistItem>? ChecklistItems { get; set; }
    }
}


