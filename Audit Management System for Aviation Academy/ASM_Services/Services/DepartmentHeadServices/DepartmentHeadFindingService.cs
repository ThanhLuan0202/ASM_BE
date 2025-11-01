using ASM_Repositories.Interfaces.DepartmentHeadInterfaces;
using ASM_Repositories.Models.FindingDTO;
using ASM_Services.Interfaces.DepartmentHeadInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services.DepartmentHeadServices
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
