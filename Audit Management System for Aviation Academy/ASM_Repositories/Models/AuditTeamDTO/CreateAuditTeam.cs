using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditTeamDTO
{
    public class CreateAuditTeam
    {
        public Guid AuditId { get; set; }
        public Guid UserId { get; set; }
        public string RoleInTeam { get; set; }
        public bool IsLead { get; set; }
    }
}
