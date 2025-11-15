using System;
using System.Collections.Generic;

namespace ASM_Repositories.Models.AuditChecklistItemDTO
{
    public class ViewAuditChecklistItemBySection
    {
        public string Section { get; set; }
        public IEnumerable<ViewAuditChecklistItem> Items { get; set; }
    }
}

