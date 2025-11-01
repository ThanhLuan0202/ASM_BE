using ASM_Repositories.Interfaces.SQAStaffInterfaces;
using ASM_Repositories.Models.ChecklistTemplateDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services.SQAStaffServices
{
    public class ChecklistTemplateService : IChecklistTemplateService
    {
        private readonly IChecklistTemplateRepository _repo;

        public ChecklistTemplateService(IChecklistTemplateRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ViewChecklistTemplate>> GetAllChecklistTemplateAsync()
        {
            return await _repo.GetAllChecklistTemplateAsync();
        }

        public async Task<ViewChecklistTemplate?> GetChecklistTemplateByIdAsync(Guid id)
        {
            return await _repo.GetChecklistTemplateByIdAsync(id);
        }

        public async Task<ViewChecklistTemplate> CreateChecklistTemplateAsync(CreateChecklistTemplate dto, Guid? createdByUserId)
        {
            return await _repo.CreateChecklistTemplateAsync(dto, createdByUserId);
        }

        public async Task<ViewChecklistTemplate?> UpdateChecklistTemplateAsync(Guid id, UpdateChecklistTemplate dto)
        {
            return await _repo.UpdateChecklistTemplateAsync(id, dto);
        }

        public async Task<bool> DeleteChecklistTemplateAsync(Guid id)
        {
            return await _repo.DeleteChecklistTemplateAsync(id);
        }
    }
}
