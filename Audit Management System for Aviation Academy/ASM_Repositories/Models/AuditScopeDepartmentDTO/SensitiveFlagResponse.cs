using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditScopeDepartmentDTO
{
    public class SensitiveFlagResponse
    {
        public Guid ScopeDeptId { get; set; }
        public Guid AuditId { get; set; }
        public int DeptId { get; set; }
        public bool SensitiveFlag { get; set; }
        public List<string> Areas { get; set; } = new List<string>();
        public string Notes { get; set; }
    }
}

