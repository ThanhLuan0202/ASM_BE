using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.UsersDTO
{
    public class CreateUser
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public int? DeptId { get; set; }
        public string Password { get; set; }
    }
}
