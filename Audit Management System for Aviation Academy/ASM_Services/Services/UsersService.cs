using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.UsersDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repo;

        public UsersService(IUsersRepository repo)
        {
            _repo = repo;
        }
        public Task<IEnumerable<ViewUser>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewUser> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<ViewUser> CreateAsync(CreateUser dto) => _repo.CreateAsync(dto);
        public Task<ViewUser> UpdateAsync(Guid id, UpdateUser dto) => _repo.UpdateAsync(id, dto);
        public Task<bool> DeleteAsync(Guid id) => _repo.DeleteAsync(id);
        public Task<IEnumerable<ViewUser>> GetByDeptIdAsync(int deptId) => _repo.GetByDeptIdAsync(deptId);
    }
}