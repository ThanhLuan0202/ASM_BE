using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditTeamDTO
{
    public class UpdateAuditTeam
    {
        /// <summary>
        /// UserId của user trong team (bắt buộc khi tạo mới trong complete-update)
        /// </summary>
        public Guid? UserId { get; set; }
        
        public string RoleInTeam { get; set; }
        public bool? IsLead { get; set; }
        public string Status { get; set; }
    }
}
