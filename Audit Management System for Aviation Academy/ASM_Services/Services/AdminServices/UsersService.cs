using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Interfaces.AdminInterfaces.AdminRepositories;
using ASM_Repositories.Models.UsersDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services.AdminServices
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repo;

        public UsersService(IUsersRepository repo)
        {
            _repo = repo;
        }
        public Task<IEnumerable<ViewUsers>> GetAllUsersAsync() => _repo.GetAllUsersAsync();
        
    }
}
