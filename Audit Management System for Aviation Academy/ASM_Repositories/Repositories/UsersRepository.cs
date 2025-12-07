using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Repositories.Models.UsersDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
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
        public async Task<IEnumerable<ViewUser>> GetAllAsync()
        {
            var users = await _context.UserAccounts.ToListAsync();
            return _mapper.Map<IEnumerable<ViewUser>>(users);
        }
        public async Task<ViewUser> GetByIdAsync(Guid id)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.UserId == id);
            return _mapper.Map<ViewUser>(user);
        }

        public async Task<ViewUser> CreateAsync(CreateUser dto)
        {
            if (await _context.UserAccounts.AnyAsync(u => u.Email == dto.Email))
                throw new Exception("Email already exists.");

            if (!await _context.Roles.AnyAsync(r => r.RoleName == dto.RoleName))
                throw new Exception($"Role '{dto.RoleName}' does not exist.");

            if (dto.DeptId.HasValue)
            {
                bool deptExists = await _context.Departments.AnyAsync(d => d.DeptId == dto.DeptId.Value);
                if (!deptExists)
                    throw new Exception($"Department with ID {dto.DeptId} does not exist.");
            }

            byte[] salt = new byte[32];
            RandomNumberGenerator.Fill(salt);
            byte[] hash;
            using (var hmac = new HMACSHA512(salt))
            {
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
            }

            var user = _mapper.Map<UserAccount>(dto);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            await _context.UserAccounts.AddAsync(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewUser>(user);
        }




        public async Task<ViewUser> UpdateAsync(Guid id, UpdateUser dto)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                throw new Exception("User not found.");

            if (!user.IsActive && dto.DeptId.HasValue && dto.DeptId.Value != user.DeptId)
                throw new Exception("Cannot change department for an inactive user.");

            if (dto.DeptId.HasValue && dto.DeptId.Value != user.DeptId)
            {
                bool deptExists = await _context.Departments.AnyAsync(d => d.DeptId == dto.DeptId.Value);
                if (!deptExists)
                    throw new Exception($"Department with ID {dto.DeptId} does not exist.");
            }

            if (!string.IsNullOrEmpty(dto.RoleName) && dto.RoleName != user.RoleName)
            {
                bool roleExists = await _context.Roles.AnyAsync(r => r.RoleName == dto.RoleName);
                if (!roleExists)
                    throw new Exception($"Role '{dto.RoleName}' does not exist.");
            }

            _mapper.Map(dto, user);
            _context.UserAccounts.Update(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewUser>(user);
        }




        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null) return false;

            user.Status = "Inactive";
            user.IsActive = false;

            _context.UserAccounts.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ViewUserShortInfo> GetUserShortInfoAsync(Guid userId)
        {
            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.UserId == userId);

            return _mapper.Map<ViewUserShortInfo>(user);
        }

        public async Task<IEnumerable<ViewUser>> GetByDeptIdAsync(int deptId)
        {
            if (deptId <= 0)
                throw new ArgumentException("DeptId must be greater than zero");

            var users = await _context.UserAccounts
                .Where(u => u.DeptId == deptId)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewUser>>(users);
        }

        public async Task<Guid?> GetDirectorIdAsync()
        {
            return await _context.UserAccounts
                .Where(u =>
                    u.RoleName == "Director" &&
                    u.IsActive &&
                    u.Status != null &&
                    u.Status.Equals("Active"))
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();
        }

        public async Task<Guid?> GetAuditeeOwnerByDepartmentIdAsync(int deptId)
        {
            return await _context.UserAccounts
                .Where(u =>
                    u.DeptId == deptId &&
                    u.RoleName == "AuditeeOwner" &&
                    u.IsActive &&
                    u.Status != null &&
                    u.Status.Equals("Active"))
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();
        }

        public async Task<ViewUserShortInfo?> GetAuditeeOwnerInfoByDepartmentIdAsync(int deptId)
        {
            var user = await _context.UserAccounts
                .Where(u =>
                    u.DeptId == deptId &&
                    u.RoleName == "AuditeeOwner" &&
                    u.IsActive &&
                    u.Status != null &&
                    u.Status.Equals("Active"))
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            return _mapper.Map<ViewUserShortInfo>(user);
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            return await _context.UserAccounts
                .AnyAsync(u => u.UserId == userId && u.IsActive && u.Status == "Active");
        }

        public async Task<Guid?> GetLeadAuditorIdAsync()
        {
            var lead = await _context.UserAccounts
                .Where(t => t.RoleName == "LeadAuditor" &&
                    t.IsActive &&
                    t.Status != null &&
                    t.Status.Equals("Active"))
                .Select(t => t.UserId)
                .FirstOrDefaultAsync();

            return lead == Guid.Empty ? null : lead;
        }

        public async Task<IEnumerable<Entities.UserAccount>> GetUsersByRolesAsync(string[] roleNames)
        {
            var users = await _context.UserAccounts
                .Where(u => roleNames.Contains(u.RoleName) 
                    && u.IsActive 
                    && u.Status == "Active")
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return users;
        }
    }
}
