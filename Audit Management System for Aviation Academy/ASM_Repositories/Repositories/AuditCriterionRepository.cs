using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditCriterionDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AuditCriterionRepository : IAuditCriterionRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AuditCriterionRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditCriterion>> GetAllAsync()
        {
            var list = await _context.AuditCriteria
                .Where(c => c.Status == "Active")
                .ToListAsync();
            return _mapper.Map<IEnumerable<ViewAuditCriterion>>(list);
        }

        public async Task<ViewAuditCriterion?> GetByIdAsync(Guid id)
        {
            var entity = await _context.AuditCriteria.FirstOrDefaultAsync(c => c.CriteriaId == id);
            return entity == null ? null : _mapper.Map<ViewAuditCriterion>(entity);
        }

        public async Task<ViewAuditCriterion> CreateAsync(CreateAuditCriterion dto)
        {
            try
            {
                if (!string.IsNullOrEmpty(dto.ReferenceCode) &&
                    await _context.AuditCriteria.AnyAsync(c => c.ReferenceCode == dto.ReferenceCode))
                {
                    throw new ArgumentException($"ReferenceCode '{dto.ReferenceCode}' already exists.");
                }

                var entity = _mapper.Map<AuditCriterion>(dto);
                _context.AuditCriteria.Add(entity);
                await _context.SaveChangesAsync();

                return _mapper.Map<ViewAuditCriterion>(entity);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AuditCriterionRepository.AddAsync] Error: " + ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception("An unexpected error occurred while creating AuditCriterion.", ex);
            }
        }

        public async Task<ViewAuditCriterion?> UpdateAsync(Guid id, UpdateAuditCriterion dto)
        {
            var entity = await _context.AuditCriteria.FirstOrDefaultAsync(c => c.CriteriaId == id);
            if (entity == null) return null;

            if (!string.IsNullOrEmpty(dto.ReferenceCode) &&
                await _context.AuditCriteria.AnyAsync(c => c.ReferenceCode == dto.ReferenceCode && c.CriteriaId != id))
            {
                throw new ArgumentException($"ReferenceCode '{dto.ReferenceCode}' already exists.");
            }

            _mapper.Map(dto, entity);
            _context.AuditCriteria.Update(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewAuditCriterion>(entity);
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.AuditCriteria.FindAsync(id);
            if (entity == null || entity.Status == "Inactive")
                return false;

            entity.Status = "Inactive";
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
