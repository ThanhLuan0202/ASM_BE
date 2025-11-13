using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.FindingSeverityDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class FindingSeverityRepository : IFindingSeverityRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public FindingSeverityRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ViewFindingSeverity>> GetAllAsync()
        {
            var data = await _context.FindingSeverities.ToListAsync();
            return _mapper.Map<List<ViewFindingSeverity>>(data);
        }

        public async Task<ViewFindingSeverity?> GetByIdAsync(string severity)
        {
            var entity = await _context.FindingSeverities
                .FirstOrDefaultAsync(x => x.Severity == severity);

            return _mapper.Map<ViewFindingSeverity?>(entity);
        }

        public async Task<ViewFindingSeverity> AddAsync(CreateFindingSeverity dto)
        {
            var exists = await _context.FindingSeverities
                .AnyAsync(x => x.Severity == dto.Severity);

            if (exists)
                throw new ArgumentException("Severity already exists.");

            var entity = _mapper.Map<FindingSeverity>(dto);
            _context.FindingSeverities.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewFindingSeverity>(entity);
        }

        public async Task<ViewFindingSeverity> UpdateAsync(string severity, UpdateFindingSeverity dto)
        {
            var entity = await _context.FindingSeverities
                .FirstOrDefaultAsync(x => x.Severity == severity);

            if (entity == null)
                throw new ArgumentException("Severity not found.");

            if (!string.Equals(severity, dto.Severity, StringComparison.OrdinalIgnoreCase))
            {
                var duplicate = await _context.FindingSeverities
                    .AnyAsync(x => x.Severity == dto.Severity);

                if (duplicate)
                    throw new ArgumentException("Severity already exists.");
            }

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewFindingSeverity>(entity);
        }

        public async Task<bool> DeleteAsync(string severity)
        {
            var entity = await _context.FindingSeverities
                .Include(x => x.ChecklistItems)
                .Include(x => x.Findings)
                .FirstOrDefaultAsync(x => x.Severity == severity);

            if (entity == null)
                return false;

            // Prevent delete if in use
            if (entity.ChecklistItems.Any() || entity.Findings.Any())
                throw new InvalidOperationException("Cannot delete severity because it is being used.");

            _context.FindingSeverities.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
