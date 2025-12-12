using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.DepartmentSensitiveAreaDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class DepartmentSensitiveAreaService : IDepartmentSensitiveAreaService
    {
        private readonly IDepartmentSensitiveAreaRepository _repository;
        private readonly IAuditLogService _logService;
        private readonly IUsersRepository _userRepository;

        public DepartmentSensitiveAreaService(
            IDepartmentSensitiveAreaRepository repository,
            IAuditLogService logService,
            IUsersRepository userRepository)
        {
            _repository = repository;
            _logService = logService;
            _userRepository = userRepository;
        }

        public Task<IEnumerable<ViewDepartmentSensitiveArea>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public Task<ViewDepartmentSensitiveArea?> GetByIdAsync(Guid id)
        {
            return _repository.GetByIdAsync(id);
        }

        public Task<ViewDepartmentSensitiveArea?> GetByDeptIdAsync(int deptId)
        {
            return _repository.GetByDeptIdAsync(deptId);
        }

        public async Task<ViewDepartmentSensitiveArea> CreateAsync(CreateDepartmentSensitiveArea dto, Guid userId)
        {
            var user = await _userRepository.GetUserShortInfoAsync(userId);
            var createdBy = user?.FullName ?? userId.ToString();

            var created = await _repository.CreateAsync(dto, createdBy);
            await _logService.LogCreateAsync(created, created.Id, userId, "DepartmentSensitiveArea");
            return created;
        }

        public async Task<ViewDepartmentSensitiveArea?> UpdateAsync(Guid id, UpdateDepartmentSensitiveArea dto, Guid userId)
        {
            var before = await _repository.GetByIdAsync(id);
            if (before == null)
                return null;

            var updated = await _repository.UpdateAsync(id, dto);
            if (updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, id, userId, "DepartmentSensitiveArea");
            }
            return updated;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var before = await _repository.GetByIdAsync(id);
            var success = await _repository.DeleteAsync(id);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, id, userId, "DepartmentSensitiveArea");
            }
            return success;
        }
    }
}

