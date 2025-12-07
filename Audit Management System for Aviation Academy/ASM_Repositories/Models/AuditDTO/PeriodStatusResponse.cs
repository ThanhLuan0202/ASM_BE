using System;

namespace ASM_Repositories.Models.AuditDTO
{
    public class PeriodStatusResponse
    {
        public bool IsExpired { get; set; }
        public bool IsActive { get; set; }
        public bool CanAssignNewPlans { get; set; }
        public int CurrentAuditCount { get; set; }
        public int MaxAuditsAllowed { get; set; } = 5;
        public int RemainingSlots { get; set; }
    }
}

