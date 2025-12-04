using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditChecklistTemplateMapDTO
{
    public class UpdateAuditChecklistTemplateMap
    {
        public DateTime AssignedAt { get; set; }
        public Guid? AssignedBy { get; set; }
        public string Status { get; set; }
    }
}
