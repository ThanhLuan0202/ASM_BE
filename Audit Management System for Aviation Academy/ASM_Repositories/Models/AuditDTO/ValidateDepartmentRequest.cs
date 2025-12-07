using System;
using System.Collections.Generic;

namespace ASM_Repositories.Models.AuditDTO
{
    public class ValidateDepartmentRequest
    {
        public Guid? AuditId { get; set; } // null nếu là audit mới
        public List<int> DepartmentIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

