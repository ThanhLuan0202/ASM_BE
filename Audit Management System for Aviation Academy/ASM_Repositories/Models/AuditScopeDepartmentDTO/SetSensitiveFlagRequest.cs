using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditScopeDepartmentDTO
{
    public class SetSensitiveFlagRequest
    {
        public bool SensitiveFlag { get; set; }
        public List<string> Areas { get; set; } = new List<string>();
        public string Notes { get; set; }
    }
}

