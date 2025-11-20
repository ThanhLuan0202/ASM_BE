using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditCriteriaMapDTO
{
    public class UpdateAuditCriteriaMap
    {
        public Guid AuditId { get; set; }
        public Guid CriteriaId { get; set; }
        public string Status { get; set; }
    }
}
