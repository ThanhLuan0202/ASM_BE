using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AccessGrantDTO
{
    public class VerifyCodeResponse
    {
        public bool IsValid { get; set; }
        public string Reason { get; set; }
    }
}

