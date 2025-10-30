using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces.SQAStaffInterfaces;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services.SQAStaffServices
{
    public class FindingService : IFindingService
    {
        private readonly IFindingRepository _repo;
        public FindingService(IFindingRepository repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<Finding>> GetAllFinding()
        {
            return await _repo.GetAllFinding();
        }
    }
}
