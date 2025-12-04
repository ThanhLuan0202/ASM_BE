using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditChecklistTemplateMapDTO
{
    public class CreateAuditChecklistTemplateMap
    {
        public Guid AuditId { get; set; }
        public Guid TemplateId { get; set; }
        public Guid? AssignedBy { get; set; }
        public string Status { get; set; }
    }
}
