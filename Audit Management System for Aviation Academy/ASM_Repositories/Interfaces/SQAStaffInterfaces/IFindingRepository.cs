using ASM_Repositories.Entities;
using ASM_Repositories.Models.FindingDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces.SQAStaffInterfaces
{
    public interface IFindingRepository
    {
        Task<IEnumerable<Finding>> GetAllFinding();
        //Task<ViewFinding> CreateFinding(CreateFinding createFinding, string userId);
    }
}
