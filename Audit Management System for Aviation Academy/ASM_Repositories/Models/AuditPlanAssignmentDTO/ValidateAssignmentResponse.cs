namespace ASM_Repositories.Models.AuditPlanAssignmentDTO
{
    public class ValidateAssignmentResponse
    {
        public bool CanCreate { get; set; }
        public string Reason { get; set; }
        public int CurrentCount { get; set; }
        public int MaxAllowed { get; set; } = 5;
        public bool IsPeriodExpired { get; set; }
    }
}

