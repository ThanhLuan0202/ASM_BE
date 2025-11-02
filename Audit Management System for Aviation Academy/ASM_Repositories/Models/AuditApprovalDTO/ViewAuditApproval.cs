using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditApprovalDTO
{
    public class ViewAuditApproval
    {
        public Guid AuditApprovalId { get; set; }
        public Guid AuditId { get; set; }
        public Guid ApproverId { get; set; }
        public string ApprovalLevel { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
