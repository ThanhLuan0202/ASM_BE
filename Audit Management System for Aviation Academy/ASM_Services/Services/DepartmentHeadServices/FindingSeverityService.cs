using ASM_Repositories.Interfaces.DepartmentHeadInterfaces;
using ASM_Repositories.Models.FindingSeverityDTO;
using ASM_Services.Interfaces.DepartmentHeadInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services.DepartmentHeadServices
{
    public class FindingSeverityService : IFindingSeverityService
    {
        private readonly IFindingSeverityRepository _repo;

        public FindingSeverityService(IFindingSeverityRepository repo)
        {
            _repo = repo;
        }

        public Task<List<ViewFindingSeverity>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewFindingSeverity?> GetByIdAsync(string severity) => _repo.GetByIdAsync(severity);
        public Task<ViewFindingSeverity> CreateAsync(CreateFindingSeverity dto) => _repo.AddAsync(dto);
        public Task<ViewFindingSeverity> UpdateAsync(string severity, UpdateFindingSeverity dto) => _repo.UpdateAsync(severity, dto);
        public Task<bool> DeleteAsync(string severity) => _repo.DeleteAsync(severity);
    }
}
