using ASM_Repositories.Models.DepartmentDTO;
using ASM_Repositories.Models.UsersDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.AdminInterfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<ViewUsers>> GetAllUsersAsync();
    }
}
