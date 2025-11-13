using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.FindingDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class DepartmentHeadFindingService : IDepartmentHeadFindingService
    {
        private readonly IDepartmentHeadFindingRepository _repo;
        public DepartmentHeadFindingService(IDepartmentHeadFindingRepository repo)
        {
            _repo = repo;
        }
        public Task<List<ViewFinding>> GetFindingsByDepartmentAsync(int deptId) => _repo.GetFindingsByDepartmentAsync(deptId);
    }
}
