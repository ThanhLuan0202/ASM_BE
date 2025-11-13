using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.FindingStatusDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class FindingStatusRepository : IFindingStatusRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public FindingStatusRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ViewFindingStatus>> GetAllAsync()
        {
            var data = await _context.FindingStatuses.ToListAsync();
            return _mapper.Map<List<ViewFindingStatus>>(data);
        }

        public async Task<ViewFindingStatus> GetByIdAsync(string status)
        {
            var entity = await _context.FindingStatuses.FirstOrDefaultAsync(x => x.FindingStatus1 == status);
            return _mapper.Map<ViewFindingStatus>(entity);
        }

        public async Task<ViewFindingStatus> AddAsync(CreateFindingStatus dto)
        {
            bool isExist = await _context.FindingStatuses
                .AnyAsync(x => x.FindingStatus1 == dto.FindingStatus1);

            if (isExist)
                throw new Exception("Status already exists!");

            var entity = _mapper.Map<FindingStatus>(dto);
            _context.FindingStatuses.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewFindingStatus>(entity);
        }

        public async Task<bool> UpdateAsync(string status, UpdateFindingStatus dto)
        {
            var entity = await _context.FindingStatuses
                .FirstOrDefaultAsync(x => x.FindingStatus1 == status);

            if (entity == null) return false;

            bool isExist = await _context.FindingStatuses
                .AnyAsync(x => x.FindingStatus1 == dto.FindingStatus1 && dto.FindingStatus1 != status);

            if (isExist)
                throw new Exception("Status already exists!");

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string status)
        {
            var entity = await _context.FindingStatuses
                .Include(x => x.Findings)  
                .FirstOrDefaultAsync(x => x.FindingStatus1 == status);

            if (entity == null) return false;

            if (entity.Findings.Any())
                throw new Exception("Cannot delete this status because it is being used!");

            _context.FindingStatuses.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

    }


}
