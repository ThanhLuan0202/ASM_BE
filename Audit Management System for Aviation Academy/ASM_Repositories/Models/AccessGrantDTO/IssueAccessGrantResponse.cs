using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AccessGrantDTO
{
    public class IssueAccessGrantResponse
    {
        public Guid GrantId { get; set; }
        public string QrToken { get; set; }
        public string QrUrl { get; set; }
        public string VerifyCode { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string Status { get; set; }
    }
}

