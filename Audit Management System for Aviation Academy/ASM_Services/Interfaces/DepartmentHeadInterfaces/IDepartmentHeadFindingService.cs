using ASM_Repositories.Models.FindingDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.DepartmentHeadInterfaces
{
    public interface IDepartmentHeadFindingService
    {
        Task<List<ViewFinding>> GetFindingsByDepartmentAsync(int deptId);
    }
}
