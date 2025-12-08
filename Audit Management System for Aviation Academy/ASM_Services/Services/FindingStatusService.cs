using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.FindingStatusDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class FindingStatusService : IFindingStatusService
    {
        private readonly IFindingStatusRepository _repo;
        private readonly IAuditLogService _logService;

        public FindingStatusService(IFindingStatusRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<List<ViewFindingStatus>> GetAllAsync() => _repo.GetAllAsync();

        public Task<ViewFindingStatus> GetByIdAsync(string status) => _repo.GetByIdAsync(status);

        public async Task<ViewFindingStatus> CreateAsync(CreateFindingStatus dto, Guid userId)
        {
            var created = await _repo.AddAsync(dto);
            var entityId = Guid.TryParse(dto.FindingStatus1, out Guid parsedId) ? parsedId : Guid.NewGuid();
            await _logService.LogCreateAsync(created, entityId, userId, "FindingStatus");
            return created;
        }

        public async Task<bool> UpdateAsync(string status, UpdateFindingStatus dto, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(status);
            var updated = await _repo.UpdateAsync(status, dto);
            
            if (updated && existing != null)
            {
                var after = await _repo.GetByIdAsync(status);
                if (after != null)
                {
                    var entityId = Guid.TryParse(status, out Guid parsedId) ? parsedId : Guid.NewGuid();
                    await _logService.LogUpdateAsync(existing, after, entityId, userId, "FindingStatus");
                }
            }
            
            return updated;
        }

        public async Task<bool> DeleteAsync(string status, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(status);
            var deleted = await _repo.DeleteAsync(status);
            
            if (deleted && existing != null)
            {
                var entityId = Guid.TryParse(status, out Guid parsedId) ? parsedId : Guid.NewGuid();
                await _logService.LogDeleteAsync(existing, entityId, userId, "FindingStatus");
            }
            
            return deleted;
        }
    }

}
