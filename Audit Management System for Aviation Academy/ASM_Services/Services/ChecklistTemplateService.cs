using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ChecklistTemplateDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class ChecklistTemplateService : IChecklistTemplateService
    {
        private readonly IChecklistTemplateRepository _repo;
        private readonly IAuditLogService _logService;

        public ChecklistTemplateService(IChecklistTemplateRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public async Task<IEnumerable<ViewChecklistTemplate>> GetAllChecklistTemplateAsync()
        {
            return await _repo.GetAllChecklistTemplateAsync();
        }

        public async Task<ViewChecklistTemplate?> GetChecklistTemplateByIdAsync(Guid id)
        {
            return await _repo.GetChecklistTemplateByIdAsync(id);
        }

        public async Task<ViewChecklistTemplate> CreateChecklistTemplateAsync(CreateChecklistTemplate dto, Guid userId)
        {
            var created = await _repo.CreateChecklistTemplateAsync(dto, userId);
            await _logService.LogCreateAsync(created, created.TemplateId, userId, "ChecklistTemplate");
            return created;
        }

        public async Task<ViewChecklistTemplate?> UpdateChecklistTemplateAsync(Guid id, UpdateChecklistTemplate dto, Guid userId)
        {
            var before = await _repo.GetChecklistTemplateByIdAsync(id);
            var updated = await _repo.UpdateChecklistTemplateAsync(id, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, id, userId, "ChecklistTemplate");
            }
            return updated;
        }

        public async Task<bool> DeleteChecklistTemplateAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetChecklistTemplateByIdAsync(id);
            var success = await _repo.DeleteChecklistTemplateAsync(id);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, id, userId, "ChecklistTemplate");
            }
            return success;
        }
    }
}
