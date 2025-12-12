using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AccessGrantDTO
{
    public class VerifyCodeRequest
    {
        public string QrToken { get; set; }
        public Guid ScannerUserId { get; set; }
        public string VerifyCode { get; set; }
    }
}

