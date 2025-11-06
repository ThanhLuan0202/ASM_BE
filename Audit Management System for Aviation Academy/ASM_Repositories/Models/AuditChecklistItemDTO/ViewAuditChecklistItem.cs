using System;

namespace ASM_Repositories.Models.AuditChecklistItemDTO
{
    public class ViewAuditChecklistItem
    {
        public Guid AuditItemId { get; set; }
        public Guid AuditId { get; set; }
        public string QuestionTextSnapshot { get; set; }
        public string Section { get; set; }
        public int? Order { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
    }
}
