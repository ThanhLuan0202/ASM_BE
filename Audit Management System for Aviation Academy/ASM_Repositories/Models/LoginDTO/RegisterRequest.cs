using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.LoginDTO
{
    public class RegisterRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string RoleName { get; set; } = null!; // e.g., Admin/User/Auditor
        public int? DeptId { get; set; }
    }
}



