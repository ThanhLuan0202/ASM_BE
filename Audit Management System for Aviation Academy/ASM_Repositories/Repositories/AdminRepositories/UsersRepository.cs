using ASM_Repositories.DBContext;
using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Repositories.Models.UsersDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories.AdminRepositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public UsersRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ViewUsers>> GetAllUsersAsync()
        {
            var users = await _context.UserAccounts.ToListAsync();
            return _mapper.Map<IEnumerable<ViewUsers>>(users);
        }
    }
}
