using System;
using System.Collections.Generic;

namespace ASM_Repositories.Models.AuditDTO
{
    public class ValidateDepartmentResponse
    {
        public bool IsValid { get; set; }
        public List<ConflictingAuditInfo> ConflictingAudits { get; set; } = new List<ConflictingAuditInfo>();
        public List<int> ConflictingDepartments { get; set; } = new List<int>();
    }

    public class ConflictingAuditInfo
    {
        public Guid AuditId { get; set; }
        public string Title { get; set; }
        public List<int> Departments { get; set; } = new List<int>();
    }
}

