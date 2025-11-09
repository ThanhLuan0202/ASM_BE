using System;

namespace ASM_Repositories.Models.AuditScheduleDTO
{
    public class ViewAuditSchedule
    {
        public Guid ScheduleId { get; set; }

        public Guid AuditId { get; set; }

        public string MilestoneName { get; set; }

        public DateTime DueDate { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }
    }
}

