using System;

namespace ASM_Repositories.Models.AuditCriteriaMapDTO
{
    public class ViewAuditCriteriaMap
    {
        public Guid AuditId { get; set; }
        public Guid CriteriaId { get; set; }
        public string Status { get; set; }
    }
}
