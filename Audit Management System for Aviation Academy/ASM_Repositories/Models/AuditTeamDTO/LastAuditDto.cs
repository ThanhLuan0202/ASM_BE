using System;

namespace ASM_Repositories.Models.AuditTeamDTO
{
    public class LastAuditDto
    {
        public Guid AuditId { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
