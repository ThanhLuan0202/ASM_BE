using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.FindingDTO;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class DepartmentHeadFindingRepository : IDepartmentHeadFindingRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;
        public DepartmentHeadFindingRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<ViewFinding>> GetFindingsByDepartmentAsync(int deptId)
        {
            return await _context.Findings
                .Where(f => f.DeptId == deptId)
                .ProjectTo<ViewFinding>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
