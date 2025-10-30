using ASM_Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.SQAStaffInterfaces
{
    public interface IFindingService
    {
        Task<IEnumerable<Finding>> GetAllFinding();

    }
}
