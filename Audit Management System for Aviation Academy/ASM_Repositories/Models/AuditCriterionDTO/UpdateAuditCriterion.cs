using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditCriterionDTO
{
    public class UpdateAuditCriterion
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReferenceCode { get; set; }
        public string PublishedBy { get; set; }
    }
}
