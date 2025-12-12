using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AccessGrantDTO
{
    public class ScanQrTokenResponse
    {
        public bool IsValid { get; set; }
        public Guid? AuditId { get; set; }
        public Guid? AuditorId { get; set; }
        public int? DeptId { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string Reason { get; set; }
    }
}

