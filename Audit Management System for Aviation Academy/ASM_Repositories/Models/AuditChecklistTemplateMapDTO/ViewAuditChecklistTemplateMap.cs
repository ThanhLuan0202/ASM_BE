using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditChecklistTemplateMapDTO
{
    public class ViewAuditChecklistTemplateMap
    {
        public Guid AuditId { get; set; }
        public Guid TemplateId { get; set; }
        public DateTime AssignedAt { get; set; }
        public Guid? AssignedBy { get; set; }
        public string Status { get; set; }
    }
}
