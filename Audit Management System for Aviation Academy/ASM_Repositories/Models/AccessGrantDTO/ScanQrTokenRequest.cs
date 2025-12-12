using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AccessGrantDTO
{
    public class ScanQrTokenRequest
    {
        public string QrToken { get; set; }
        public Guid ScannerUserId { get; set; }
    }
}

