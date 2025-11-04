using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditScopeDepartmentDTO
{
    public class CreateAuditScopeDepartment
    {
        public Guid AuditId { get; set; }
        public int DeptId { get; set; }
    }
}
