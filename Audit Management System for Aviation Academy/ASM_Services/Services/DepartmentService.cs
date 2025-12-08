using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Services.Interfaces.AdminInterfaces.AdminServices;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repo;
        private readonly IAuditLogService _logService;

        public DepartmentService(IDepartmentRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewDepartment>> GetAllDepartmentsAsync() => _repo.GetAllDepartmentsAsync();
        public Task<ViewDepartment?> GetDepartmentByIdAsync(int id) => _repo.GetDepartmentByIdAsync(id);
        public async Task<ViewDepartment> CreateDepartmentAsync(CreateDepartment dto, Guid userId)
        {
            var created = await _repo.CreateDepartmentAsync(dto);
            var entityId = Guid.NewGuid(); // dept key int; use surrogate Guid for log context
            await _logService.LogCreateAsync(created, entityId, userId, "Department");
            return created;
        }
        public async Task<ViewDepartment?> UpdateDepartmentAsync(int id, UpdateDepartment dto, Guid userId)
        {
            var before = await _repo.GetDepartmentByIdAsync(id);
            var updated = await _repo.UpdateDepartmentAsync(id, dto);
            if (before != null && updated != null)
            {
                var entityId = Guid.NewGuid();
                await _logService.LogUpdateAsync(before, updated, entityId, userId, "Department");
            }
            return updated;
        }
        public async Task<bool> DeleteDepartmentAsync(int id, Guid userId)
        {
            var before = await _repo.GetDepartmentByIdAsync(id);
            var success = await _repo.DeleteDepartmentAsync(id);
            if (success && before != null)
            {
                var entityId = Guid.NewGuid();
                await _logService.LogDeleteAsync(before, entityId, userId, "Department");
            }
            return success;
        }
    }

}
