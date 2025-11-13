using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.FindingDTO;
using ASM_Repositories.Repositories;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class FindingService : IFindingService
    {
        private readonly IFindingRepository _repo;
        
        public FindingService(IFindingRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ViewFinding>> GetAllFindingAsync()
        {
            return await _repo.GetAllFindingAsync();
        }

        public async Task<ViewFinding?> GetFindingByIdAsync(Guid id)
        {
            return await _repo.GetFindingByIdAsync(id);
        }

        public async Task<ViewFinding> CreateFindingAsync(CreateFinding dto, Guid? createdByUserId)
        {
            return await _repo.CreateFindingAsync(dto, createdByUserId);
        }

        public async Task<ViewFinding?> UpdateFindingAsync(Guid id, UpdateFinding dto)
        {
            return await _repo.UpdateFindingAsync(id, dto);
        }

        public async Task<bool> DeleteFindingAsync(Guid id)
        {
            return await _repo.DeleteFindingAsync(id);
        }

        public Task<List<Finding>> GetFindingsAsync(Guid auditId) => _repo.GetFindingsAsync(auditId);

        public async Task<List<(DateTime Date, int Count)>> GetFindingsByMonthAsync(Guid auditId)
        {
            var data = await _repo.GetFindingsByMonthAsync(auditId);

            // Nếu không có dữ liệu, trả 12 tháng = 0
            if (data == null || data.Count == 0)
            {
                return Enumerable.Range(1, 12)
                    .Select(m => (new DateTime(DateTime.Now.Year, m, 1), 0))
                    .ToList();
            }

            // Đảm bảo dùng 12 tháng trong năm của dữ liệu
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

        public async Task<List<(string Department, int Count)>> GetDepartmentFindingsInCurrentMonthAsync(Guid auditId)
        {
            var result = await _repo.GetDepartmentFindingsInCurrentMonthAsync(auditId);
            return result;
        }

    }
}
