using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditLogDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AuditLogRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditLog>> GetAllAsync()
        {
            var data = await _context.AuditLogs
                .Include(x => x.PerformedByNavigation)
                .OrderByDescending(x => x.PerformedAt)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ViewAuditLog>>(data);
        }

        public async Task<ViewAuditLog?> GetByIdAsync(Guid logId)
        {
            var entity = await _context.AuditLogs
                .Include(x => x.PerformedByNavigation)
                .FirstOrDefaultAsync(x => x.LogId == logId);
            return entity == null ? null : _mapper.Map<ViewAuditLog>(entity);
        }

        public async Task<ViewAuditLog> CreateAsync(CreateAuditLog dto)
        {
            if (dto.PerformedBy.HasValue)
            {
                var userExists = await _context.UserAccounts.AnyAsync(u => u.UserId == dto.PerformedBy.Value);
                if (!userExists)
                    throw new InvalidOperationException($"User with ID {dto.PerformedBy.Value} does not exist.");
            }

            var entity = _mapper.Map<AuditLog>(dto);
            entity.LogId = Guid.NewGuid();
            entity.PerformedAt = DateTime.UtcNow;

            _context.AuditLogs.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.AuditLogs
                .Include(x => x.PerformedByNavigation)
                .FirstOrDefaultAsync(x => x.LogId == entity.LogId);

            return _mapper.Map<ViewAuditLog>(created);
        }

        public async Task<ViewAuditLog?> UpdateAsync(Guid logId, UpdateAuditLog dto)
        {
            var entity = await _context.AuditLogs
                .FirstOrDefaultAsync(x => x.LogId == logId);

            if (entity == null) return null;

            if (dto.PerformedBy.HasValue)
            {
                var userExists = await _context.UserAccounts.AnyAsync(u => u.UserId == dto.PerformedBy.Value);
                if (!userExists)
                    throw new InvalidOperationException($"User with ID {dto.PerformedBy.Value} does not exist.");
            }

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            var updated = await _context.AuditLogs
                .Include(x => x.PerformedByNavigation)
                .FirstOrDefaultAsync(x => x.LogId == logId);

            return _mapper.Map<ViewAuditLog>(updated);
        }

        public async Task<bool> DeleteAsync(Guid logId)
        {
            var entity = await _context.AuditLogs.FindAsync(logId);
            if (entity == null) return false;

     
            _context.AuditLogs.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

