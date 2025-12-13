using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.SensitiveAreaLevelDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class SensitiveAreaLevelRepository : ISensitiveAreaLevelRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public SensitiveAreaLevelRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ViewSensitiveAreaLevel>> GetAllAsync()
        {
            var data = await _context.SensitiveAreaLevels.ToListAsync();
            return _mapper.Map<List<ViewSensitiveAreaLevel>>(data);
        }

        public async Task<ViewSensitiveAreaLevel?> GetByIdAsync(string level)
        {
            var entity = await _context.SensitiveAreaLevels
                .FirstOrDefaultAsync(x => x.Level == level);

            return entity == null ? null : _mapper.Map<ViewSensitiveAreaLevel>(entity);
        }

        public async Task<ViewSensitiveAreaLevel> AddAsync(CreateSensitiveAreaLevel dto)
        {
            var exists = await _context.SensitiveAreaLevels
                .AnyAsync(x => x.Level == dto.Level);

            if (exists)
                throw new ArgumentException("Level already exists.");

            var entity = _mapper.Map<SensitiveAreaLevel>(dto);
            _context.SensitiveAreaLevels.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewSensitiveAreaLevel>(entity);
        }

        public async Task<ViewSensitiveAreaLevel> UpdateAsync(string level, UpdateSensitiveAreaLevel dto)
        {
            var entity = await _context.SensitiveAreaLevels
                .FirstOrDefaultAsync(x => x.Level == level);

            if (entity == null)
                throw new ArgumentException("Level not found.");

            // If changing the Level (primary key), check for duplicates
            if (!string.Equals(level, dto.Level, StringComparison.OrdinalIgnoreCase))
            {
                var duplicate = await _context.SensitiveAreaLevels
                    .AnyAsync(x => x.Level == dto.Level);

                if (duplicate)
                    throw new ArgumentException("Level already exists.");

                // Update all foreign keys in DepartmentSensitiveArea
                var relatedAreas = await _context.DepartmentSensitiveAreas
                    .Where(x => x.Level == level)
                    .ToListAsync();

                foreach (var area in relatedAreas)
                {
                    area.Level = dto.Level;
                }
            }

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewSensitiveAreaLevel>(entity);
        }

        public async Task<bool> DeleteAsync(string level)
        {
            var entity = await _context.SensitiveAreaLevels
                .Include(x => x.DepartmentSensitiveAreas)
                .FirstOrDefaultAsync(x => x.Level == level);

            if (entity == null)
                return false;

            // Prevent delete if in use
            if (entity.DepartmentSensitiveAreas.Any())
                throw new InvalidOperationException("Cannot delete level because it is being used by DepartmentSensitiveArea.");

            _context.SensitiveAreaLevels.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

