using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ChecklistItemNoFindingDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class ChecklistItemNoFindingRepository : IChecklistItemNoFindingRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public ChecklistItemNoFindingRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewChecklistItemNoFinding>> GetAllAsync()
        {
            var list = await _context.ChecklistItemNoFindings
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewChecklistItemNoFinding>>(list);
        }

        public async Task<ViewChecklistItemNoFinding?> GetByIdAsync(int id)
        {
            var entity = await _context.ChecklistItemNoFindings
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return entity == null ? null : _mapper.Map<ViewChecklistItemNoFinding>(entity);
        }

        public async Task<ViewChecklistItemNoFinding> CreateAsync(CreateChecklistItemNoFinding dto)
        {
            try
            {
                var entity = _mapper.Map<ChecklistItemNoFinding>(dto);
                entity.CreatedDate = DateTime.UtcNow;

                _context.ChecklistItemNoFindings.Add(entity);
                await _context.SaveChangesAsync();

                return _mapper.Map<ViewChecklistItemNoFinding>(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ChecklistItemNoFindingRepository.CreateAsync] Error: " + ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception("An unexpected error occurred while creating the checklist item no finding.", ex);
            }
        }

        public async Task<ViewChecklistItemNoFinding?> UpdateAsync(int id, UpdateChecklistItemNoFinding dto)
        {
            try
            {
                var existing = await _context.ChecklistItemNoFindings
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (existing == null)
                    return null;

                _mapper.Map(dto, existing);
                _context.ChecklistItemNoFindings.Update(existing);
                await _context.SaveChangesAsync();

                return _mapper.Map<ViewChecklistItemNoFinding>(existing);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ChecklistItemNoFindingRepository.UpdateAsync] Error: " + ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception("An unexpected error occurred while updating the checklist item no finding.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ChecklistItemNoFindings
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null)
                return false;

            _context.ChecklistItemNoFindings.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task UpdateStatusToArchivedAsync(Guid auditId)
        {
            await Task.CompletedTask;
        }
    }
}
