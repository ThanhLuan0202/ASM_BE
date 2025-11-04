using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditTeamDTO
{
    public class ViewAuditTeam
    {
        public Guid AuditTeamId { get; set; }
        public Guid AuditId { get; set; }
        public Guid UserId { get; set; }
        public string RoleInTeam { get; set; }
        public bool IsLead { get; set; }
        public string Status { get; set; }
    }
}
