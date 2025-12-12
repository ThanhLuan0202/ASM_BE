using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AccessGrantDTO
{
    public class ViewAccessGrant
    {
        public Guid GrantId { get; set; }
        public Guid AuditId { get; set; }
        public Guid AuditorId { get; set; }
        public int DeptId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string VerifyCode { get; set; }
        public int TtlMinutes { get; set; }
        public string QrToken { get; set; }
        public string QrUrl { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

