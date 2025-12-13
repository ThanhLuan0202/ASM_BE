using System;

namespace ASM_Repositories.Models.RootCauseDTO
{
    public class ViewRootCause
    {
        public int RootCauseId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public int? DeptId { get; set; }
        public string DepartmentName { get; set; }
        public Guid? FindingId { get; set; }
    }
}
