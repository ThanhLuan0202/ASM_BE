using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Models.AuditStatusDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories.AdminRepositories
{
    public class AuditStatusRepository : IAuditStatusRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AuditStatusRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditStatus>> GetAllAsync()
        {
            var data = await _context.AuditStatuses.ToListAsync();
            return _mapper.Map<IEnumerable<ViewAuditStatus>>(data);
        }

        public async Task<ViewAuditStatus?> GetByIdAsync(string auditStatus)
        {
            var entity = await _context.AuditStatuses
                .FirstOrDefaultAsync(x => x.AuditStatus1 == auditStatus);
            return entity == null ? null : _mapper.Map<ViewAuditStatus>(entity);
        }

        public async Task<ViewAuditStatus> CreateAsync(CreateAuditStatus dto)
        {
            bool isExist = await _context.AuditStatuses
                .AnyAsync(x => x.AuditStatus1 == dto.AuditStatus1);

            if (isExist)
                throw new InvalidOperationException("AuditStatus already exists!");

            var entity = _mapper.Map<AuditStatus>(dto);
            _context.AuditStatuses.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewAuditStatus>(entity);
        }

        public async Task<ViewAuditStatus?> UpdateAsync(string auditStatus, UpdateAuditStatus dto)
        {
            var entity = await _context.AuditStatuses
                .FirstOrDefaultAsync(x => x.AuditStatus1 == auditStatus);

            if (entity == null) return null;

            // Check if new status name already exists (excluding current one)
            bool isExist = await _context.AuditStatuses
                .AnyAsync(x => x.AuditStatus1 == dto.AuditStatus1 && dto.AuditStatus1 != auditStatus);

            if (isExist)
                throw new InvalidOperationException("AuditStatus already exists!");

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewAuditStatus>(entity);
        }

        public async Task<bool> DeleteAsync(string auditStatus)
        {
            var entity = await _context.AuditStatuses
                .Include(x => x.Audits)
                .FirstOrDefaultAsync(x => x.AuditStatus1 == auditStatus);

            if (entity == null) return false;

            // Check if status is being used
            if (entity.Audits.Any())
                throw new InvalidOperationException("Cannot delete this AuditStatus because it is being used by one or more Audits!");

            _context.AuditStatuses.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

