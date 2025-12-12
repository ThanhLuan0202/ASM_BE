using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditScopeDepartmentDTO;
using ASM_Repositories.Models.DepartmentDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AuditScopeDepartmentRepository : IAuditScopeDepartmentRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AuditScopeDepartmentRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditScopeDepartment>> GetAllAsync()
        {
            var list = await _context.AuditScopeDepartments
                .Where(x => x.Status == "Active")
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAuditScopeDepartment>>(list);
        }

        public async Task<ViewAuditScopeDepartment?> GetByIdAsync(Guid id)
        {
            var entity = await _context.AuditScopeDepartments
                .FirstOrDefaultAsync(x => x.AuditScopeId == id);

            return entity == null ? null : _mapper.Map<ViewAuditScopeDepartment>(entity);
        }

        public async Task<ViewAuditScopeDepartment> AddAsync(CreateAuditScopeDepartment dto)
        {
            try
            {
                bool auditExists = await _context.Audits.AnyAsync(a => a.AuditId == dto.AuditId);
                bool deptExists = await _context.Departments.AnyAsync(d => d.DeptId == dto.DeptId);

                if (!auditExists)
                    throw new ArgumentException($"AuditId '{dto.AuditId}' does not exist.");
                if (!deptExists)
                    throw new ArgumentException($"DeptId '{dto.DeptId}' does not exist.");

                bool duplicate = await _context.AuditScopeDepartments
                    .AnyAsync(x => x.AuditId == dto.AuditId && x.DeptId == dto.DeptId);

                if (duplicate)
                    throw new ArgumentException("This Audit and Department mapping already exists.");

                var entity = _mapper.Map<AuditScopeDepartment>(dto);
                _context.AuditScopeDepartments.Add(entity);
                await _context.SaveChangesAsync();

                return _mapper.Map<ViewAuditScopeDepartment>(entity);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AuditScopeDepartmentRepository.AddAsync] Error: " + ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception("An unexpected error occurred while creating AuditScopeDepartment.", ex);
            }
        }

        public async Task<ViewAuditScopeDepartment?> UpdateAsync(Guid id, UpdateAuditScopeDepartment dto)
        {
            var entity = await _context.AuditScopeDepartments.FirstOrDefaultAsync(x => x.AuditScopeId == id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            _context.AuditScopeDepartments.Update(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewAuditScopeDepartment>(entity);
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.AuditScopeDepartments.FirstOrDefaultAsync(x => x.AuditScopeId == id);
            if (entity == null || entity.Status == "Inactive") return false;

            entity.Status = "Inactive";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AuditScopeDepartment>> GetAuditScopeDepartmentsAsync(Guid auditId)
        => await _context.AuditScopeDepartments.Include(f => f.Dept)
            .Where(f => f.AuditId == auditId).ToListAsync();

        public async Task UpdateScopeDepartmentsAsync(Guid auditId, List<UpdateAuditScopeDepartment>? list)
        {
            if (list == null || !list.Any())
                return; // Không có gì để update, bỏ qua

            // Xóa scope cũ
            var existing = _context.AuditScopeDepartments
                .Where(x => x.AuditId == auditId);
            _context.AuditScopeDepartments.RemoveRange(existing);

            // Thêm scope mới
            foreach (var item in list)
            {
                var entity = _mapper.Map<AuditScopeDepartment>(item);
                entity.AuditId = auditId;
                await _context.AuditScopeDepartments.AddAsync(entity);
            }
        }

        public async Task<IEnumerable<ViewDepartment>> GetDepartmentsByAuditIdAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty");

            var auditScopeDepts = await _context.AuditScopeDepartments
                .Where(asc => asc.AuditId == auditId && asc.Status == "Active")
                .Include(asc => asc.Dept)
                .ToListAsync();

            var departments = auditScopeDepts
                .Where(asc => asc.Dept != null)
                .Select(asc => asc.Dept)
                .Distinct()
                .OrderBy(d => d.Name)
                .ToList();

            return _mapper.Map<IEnumerable<ViewDepartment>>(departments);
        }

        public async Task UpdateStatusToArchivedAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty.");

            var entities = await _context.AuditScopeDepartments
                .Where(a => a.AuditId == auditId)
                .ToListAsync();

            if (!entities.Any())
                throw new InvalidOperationException($"No AuditScopeDepartment found for AuditId '{auditId}'.");

            foreach (var entity in entities)
            {
                entity.Status = "Archived";
                _context.Entry(entity).Property(x => x.Status).IsModified = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<SensitiveFlagResponse> SetSensitiveFlagAsync(Guid scopeDeptId, SetSensitiveFlagRequest request)
        {
            var entity = await _context.AuditScopeDepartments
                .FirstOrDefaultAsync(x => x.AuditScopeId == scopeDeptId);

            if (entity == null)
                throw new ArgumentException($"AuditScopeDepartment with ID '{scopeDeptId}' not found.");

            // Set sensitive flag và notes
            entity.SensitiveFlag = request.SensitiveFlag;
            entity.Notes = request.Notes;

            // Serialize areas thành JSON
            if (request.Areas != null && request.Areas.Any())
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false
                };
                entity.Areas = JsonSerializer.Serialize(request.Areas, jsonOptions);
            }
            else
            {
                entity.Areas = null;
            }

            _context.AuditScopeDepartments.Update(entity);
            await _context.SaveChangesAsync();

            // Deserialize areas để trả về
            List<string> areas = new List<string>();
            if (!string.IsNullOrWhiteSpace(entity.Areas))
            {
                try
                {
                    areas = JsonSerializer.Deserialize<List<string>>(entity.Areas) ?? new List<string>();
                }
                catch
                {
                    areas = new List<string>();
                }
            }

            return new SensitiveFlagResponse
            {
                ScopeDeptId = entity.AuditScopeId,
                AuditId = entity.AuditId,
                DeptId = entity.DeptId,
                SensitiveFlag = entity.SensitiveFlag ?? false,
                Areas = areas,
                Notes = entity.Notes
            };
        }

        public async Task<IEnumerable<SensitiveFlagResponse>> GetSensitiveFlagsByAuditIdAsync(Guid auditId)
        {
            var entities = await _context.AuditScopeDepartments
                .Where(x => x.AuditId == auditId && x.SensitiveFlag == true)
                .ToListAsync();

            var responses = new List<SensitiveFlagResponse>();

            foreach (var entity in entities)
            {
                // Deserialize areas
                List<string> areas = new List<string>();
                if (!string.IsNullOrWhiteSpace(entity.Areas))
                {
                    try
                    {
                        areas = JsonSerializer.Deserialize<List<string>>(entity.Areas) ?? new List<string>();
                    }
                    catch
                    {
                        areas = new List<string>();
                    }
                }

                responses.Add(new SensitiveFlagResponse
                {
                    ScopeDeptId = entity.AuditScopeId,
                    AuditId = entity.AuditId,
                    DeptId = entity.DeptId,
                    SensitiveFlag = entity.SensitiveFlag ?? false,
                    Areas = areas,
                    Notes = entity.Notes
                });
            }

            return responses;
        }

    }

}
