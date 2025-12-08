using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.FindingDTO;
using ASM_Repositories.Repositories;
using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class FindingService : IFindingService
    {
        private readonly IFindingRepository _repo;
        private readonly IAuditLogService _logService;
        
        public FindingService(IFindingRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public async Task<IEnumerable<ViewFinding>> GetAllFindingAsync()
        {
            return await _repo.GetAllFindingAsync();
        }

        public async Task<ViewFinding?> GetFindingByIdAsync(Guid id)
        {
            return await _repo.GetFindingByIdAsync(id);
        }

        public async Task<ViewFinding> CreateFindingAsync(CreateFinding dto, Guid userId)
        {
            var created = await _repo.CreateFindingAsync(dto, userId);
            await _logService.LogCreateAsync(created, created.FindingId, userId, "Finding");
            return created;
        }

        public async Task<ViewFinding?> UpdateFindingAsync(Guid id, UpdateFinding dto, Guid userId)
        {
            var existing = await _repo.GetFindingByIdAsync(id);
            var updated = await _repo.UpdateFindingAsync(id, dto);
            
            if (updated != null && existing != null)
            {
                await _logService.LogUpdateAsync(existing, updated, id, userId, "Finding");
            }
            
            return updated;
        }

        public async Task<bool> DeleteFindingAsync(Guid id, Guid userId)
        {
            var existing = await _repo.GetFindingByIdAsync(id);
            var deleted = await _repo.DeleteFindingAsync(id);
            
            if (deleted && existing != null)
            {
                await _logService.LogDeleteAsync(existing, id, userId, "Finding");
            }
            
            return deleted;
        }

        public Task<List<Finding>> GetFindingsAsync(Guid auditId) => _repo.GetFindingsAsync(auditId);

        public async Task<List<(DateTime Date, int Count)>> GetFindingsByMonthAsync(Guid auditId)
        {
            var data = await _repo.GetFindingsByMonthAsync(auditId);

            if (data == null || data.Count == 0)
            {
                return Enumerable.Range(1, 12)
                    .Select(m => (new DateTime(DateTime.Now.Year, m, 1), 0))
                    .ToList();
            }

            int year = data.First().Date.Year;
            var fullRange = Enumerable.Range(1, 12)
                .Select(m =>
                {
                    var monthStart = new DateTime(year, m, 1);
                    var cnt = data.FirstOrDefault(x => x.Date.Month == m)?.Count ?? 0;
                    return (Date: monthStart, Count: cnt);
                })
                .ToList();

            return fullRange;
        }

        public async Task<List<(string Department, int Count)>> GetDepartmentFindingsInAuditAsync(Guid auditId)
        {
            var result = await _repo.GetDepartmentFindingsInAuditAsync(auditId);
            return result;
        }

        public Task<IEnumerable<ViewFinding>> GetFindingsByDepartmentAsync(int departmentId)
            => _repo.GetByDepartmentIdAsync(departmentId);

        public async Task<IEnumerable<ViewFinding>> GetFindingsByAuditItemIdAsync(Guid auditItemId)
        {
            return await _repo.GetByAuditItemIdAsync(auditItemId);
        }

        public async Task<ViewFinding?> SetReceivedAsync(Guid findingId, Guid userId)
        {
            var existing = await _repo.GetFindingByIdAsync(findingId);
            var updated = await _repo.SetReceivedAsync(findingId);
            
            if (updated != null && existing != null)
            {
                await _logService.LogUpdateAsync(existing, updated, findingId, userId, "Finding");
            }
            
            return updated;
        }

        public async Task<IEnumerable<ViewFinding>> GetFindingsByAuditIdAsync(Guid auditId)
        {
            return await _repo.GetFindingsByAuditIdAsync(auditId);
        }

        public async Task<IEnumerable<ViewFinding>> GetFindingsByCreatedByAsync(Guid createdBy)
        {
            return await _repo.GetFindingsByCreatedByAsync(createdBy);
        }

    }
}
