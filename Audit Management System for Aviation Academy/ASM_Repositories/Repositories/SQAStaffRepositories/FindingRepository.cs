using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces.SQAStaffInterfaces;
using ASM_Repositories.Models.FindingDTO;
using ASM_Repositories.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories.SQAStaffRepositories
{
    public class FindingRepository : Repository<Finding>, IFindingRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _DbContext;
        public FindingRepository(AuditManagementSystemForAviationAcademyContext DbContext)
        {
            _DbContext = DbContext;
        }





        public async Task<IEnumerable<Finding>> GetAllFinding()
        {
            var query = await _DbContext.Findings.ToListAsync();

            return query;
        }

      

    }
}
