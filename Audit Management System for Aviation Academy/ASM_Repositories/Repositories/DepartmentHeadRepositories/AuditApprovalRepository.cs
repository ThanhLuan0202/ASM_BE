using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces.DepartmentHeadInterfaces;
using ASM_Repositories.Models.AuditApprovalDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories.DepartmentHeadRepositories
{
    public class AuditApprovalRepository : IAuditApprovalRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AuditApprovalRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditApproval>> GetAllAsync()
        {
            var list = await _context.AuditApprovals.ToListAsync();
            return _mapper.Map<IEnumerable<ViewAuditApproval>>(list);
        }

        public async Task<ViewAuditApproval?> GetByIdAsync(Guid id)
        {
            var entity = await _context.AuditApprovals.FindAsync(id);
            return _mapper.Map<ViewAuditApproval?>(entity);
        }

        public async Task<ViewAuditApproval> AddAsync(CreateAuditApproval dto)
        {
            try
            {
                if (!await _context.Audits.AnyAsync(a => a.AuditId == dto.AuditId))
                    throw new ArgumentException($"AuditId '{dto.AuditId}' does not exist.");

                if (!await _context.UserAccounts.AnyAsync(u => u.UserId == dto.ApproverId))
                    throw new ArgumentException($"ApproverId '{dto.ApproverId}' does not exist.");

                var exists = await _context.AuditApprovals.AnyAsync(a =>
                    a.AuditId == dto.AuditId);

                if (exists)
                    throw new ArgumentException($"An approval record for this AuditId already exists.");

                var entity = _mapper.Map<AuditApproval>(dto);

                _context.AuditApprovals.Add(entity);
                await _context.SaveChangesAsync();

                return _mapper.Map<ViewAuditApproval>(entity);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ [AuditApprovalRepository.AddAsync] Error: " + ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception("An unexpected error occurred while creating the audit approval.", ex);
            }
        }


        public async Task<ViewAuditApproval> UpdateAsync(Guid id, UpdateAuditApproval dto)
        {
            var existing = await _context.AuditApprovals.FirstOrDefaultAsync(a => a.AuditApprovalId == id);
            if (existing == null)
                throw new ArgumentException("AuditApproval not found.");

            _mapper.Map(dto, existing);
            _context.AuditApprovals.Update(existing);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewAuditApproval>(existing);
        }

        public async Task<bool> SoftDeleteAsync(Guid id, Guid userId)
        {
            var entity = await _context.AuditApprovals.FindAsync(id);
            if (entity == null)
                return false;

            if (entity.ApproverId != userId)
                throw new UnauthorizedAccessException("You are not authorized to delete this approval.");

            if (entity.Status == "Approved")
                throw new InvalidOperationException("Approved approvals cannot be deleted.");

            entity.Status = "Inactive";
            await _context.SaveChangesAsync();

            return true;
        }

    }

}
