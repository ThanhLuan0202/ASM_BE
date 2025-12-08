using System;

namespace ASM_Repositories.Models.AuditTeamDTO
{
    public class AvailableTeamMemberDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public LastAuditDto LastAudit { get; set; }
    }
}
