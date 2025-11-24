using System;

namespace ASM_Repositories.Models.AuditTeamDTO
{
    public class AuditorInfoDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}

