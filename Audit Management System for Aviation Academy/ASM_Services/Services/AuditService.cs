using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditDTO;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Repositories.Models.FindingDTO;
using ASM_Repositories.Models.RootCauseDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _repo;
        private readonly IFindingRepository _findingRepo;
        private readonly IDepartmentRepository _departmentRepo;
        private readonly IRootCauseRepository _rootCauseRepo;
        private readonly IAuditDocumentRepository _auditDocumentRepo;
        private readonly IReportRequestRepository _reportRequestRepo;
        private readonly IAuditScopeDepartmentRepository _auditScopeDepartmentRepo;
        private readonly IMapper _mapper ;
        public AuditService(IAuditRepository repo, IFindingRepository findingRepo, IDepartmentRepository departmentRepo, IRootCauseRepository rootCauseRepo, IAuditDocumentRepository auditDocumentRepo, IReportRequestRepository reportRequestRepo, IAuditScopeDepartmentRepository auditScopeDepartmentRepo, IMapper mapper)
        {
            _repo = repo;
            _findingRepo = findingRepo;
            _departmentRepo = departmentRepo;
            _rootCauseRepo = rootCauseRepo;
            _auditDocumentRepo = auditDocumentRepo;
            _reportRequestRepo = reportRequestRepo;
            _auditScopeDepartmentRepo = auditScopeDepartmentRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAudit>> GetAllAuditAsync()
        {
            return await _repo.GetAllAuditAsync();
        }

        public async Task<ViewAudit?> GetAuditByIdAsync(Guid id)
        {
            return await _repo.GetAuditByIdAsync(id);
        }

        public async Task<ViewAudit> CreateAuditAsync(CreateAudit dto, Guid? createdByUserId)
        {
            return await _repo.CreateAuditAsync(dto, createdByUserId);
        }

        public async Task<ViewAudit?> UpdateAuditAsync(Guid id, UpdateAudit dto)
        {
            return await _repo.UpdateAuditAsync(id, dto);
        }

        public async Task<bool> DeleteAuditAsync(Guid id)
        {
            return await _repo.DeleteAuditAsync(id);
        }

        public Task<List<ViewAuditPlan>> GetAuditPlansAsync() => _repo.GetAllAuditPlansAsync();

        public Task<ViewAuditPlan?> GetAuditPlanDetailsAsync(Guid auditId) => _repo.GetAuditPlanByIdAsync(auditId);

        public Task<bool> UpdateStatusAsync(Guid auditId, string status) => _repo.UpdateStatusAsync(auditId, status);

        public Task<bool> SubmitToLeadAuditorAsync(Guid auditId)
            => _repo.SubmitToLeadAuditorAsync(auditId);

        public Task<bool> RejectPlanContentAsync(Guid auditId, Guid approverId, string comment)
            => _repo.RejectPlanContentAsync(auditId, approverId, comment);

        public Task<bool> ApproveAndForwardToDirectorAsync(Guid auditId, Guid approverId, string comment)
            => _repo.ApproveAndForwardToDirectorAsync(auditId, approverId, comment);

        public Task<bool> ApprovePlanAsync(Guid auditId, Guid approverId, string comment)
            => _repo.ApprovePlanAsync(auditId, approverId, comment);

        public async Task<ViewAuditSummary?> GetAuditSummaryAsync(Guid auditId)
        {
            var audit = await _repo.GetAuditByIdAsync(auditId);
            if (audit == null) return null;

            var findings = await _findingRepo.GetFindingsAsync(auditId);
            var scopeDepts = await _auditScopeDepartmentRepo.GetAuditScopeDepartmentsAsync(auditId);

            var now = DateTime.UtcNow;

            var findingsThisMonth = findings
                .Where(f => f.CreatedAt.Month == now.Month && f.CreatedAt.Year == now.Year)
                .ToList();

            var total = findingsThisMonth.Count;
            var open = findingsThisMonth.Count(f => f.Status == "Open");
            var closed = findingsThisMonth.Count(f => f.Status == "Closed");
            var overdue = findingsThisMonth.Count(f => f.Deadline != null && f.Deadline < now && f.Status != "Closed");

            var severityGroups = findingsThisMonth
                .GroupBy(f => f.Severity ?? "N/A")
                .Select(g => new { Severity = g.Key, Count = g.Count() })
                .ToList();

            var findingDeptIds = findingsThisMonth
                .Where(f => f.DeptId != null)
                .Select(f => f.DeptId.Value)
                .ToList();

            var scopeDeptIds = scopeDepts.Select(s => s.DeptId).ToList();

            var allDeptIds = scopeDeptIds
                .Union(findingDeptIds)
                .Distinct()
                .ToList();

            var deptNames = await _departmentRepo.GetDepartmentsAsync(allDeptIds);

            var byDepartment = allDeptIds
                .Select(id => new ViewDepartmentCount
                {
                    DeptName = deptNames.ContainsKey(id) ? deptNames[id] : "Unknown",
                    Count = findingsThisMonth.Count(f => f.DeptId == id)
                })
                .GroupBy(x => x.DeptName)
                .Select(g => new ViewDepartmentCount
                {
                    DeptName = g.Key,
                    Count = g.Sum(x => x.Count)
                })
                .ToList();

            // RootCause summary
            var byRootGroups = findingsThisMonth
                .GroupBy(f => f.RootCauseId)
                .Select(g => new { RootId = g.Key, Count = g.Count() })
                .ToList();

            var rootIds = byRootGroups
                .Where(x => x.RootId.HasValue)
                .Select(x => x.RootId.Value)
                .ToList();

            var rootNames = await _rootCauseRepo.GetRootCausesAsync(rootIds);

            var byRootCause = byRootGroups
                .Select(x => new ViewRootCauseCount
                {
                    RootCause = x.RootId.HasValue && rootNames.ContainsKey(x.RootId.Value)
                        ? rootNames[x.RootId.Value]
                        : "Unknown",
                    Count = x.Count
                })
                .ToList();

            // Mapping Findings
            var mappedFindings = _mapper.Map<List<ViewFindingDetail>>(findingsThisMonth);

            var findingsByMonth = new ViewFindingByMonth
            {
                Month = now.Month,
                Total = total,
                Open = open,
                Closed = closed,
                Overdue = overdue,
                Findings = mappedFindings
            };

            return new ViewAuditSummary
            {
                AuditId = audit.AuditId,
                Title = audit.Title,
                Type = audit.Type,
                Scope = audit.Scope,
                Status = audit.Status,
                StartDate = audit.StartDate,
                EndDate = audit.EndDate,

                TotalFindings = total,
                OpenFindings = open,
                ClosedFindings = closed,
                OverdueFindings = overdue,

                SeverityBreakdown = severityGroups.ToDictionary(x => x.Severity, x => x.Count),
                ByDepartment = byDepartment,
                ByRootCause = byRootCause,
                FindingsByMonth = new List<ViewFindingByMonth> { findingsByMonth }
            };
        }



        public async Task SubmitAuditAsync(Guid auditId, string pdfUrl, Guid requestedBy)
        {
            var audit = await _repo.GetAuditByIdAsync(auditId);
            if (audit == null) throw new Exception("Audit not found");

            // Cập nhật trạng thái audit
            audit.Status = "Submitted";

            // Tạo AuditDocument
            var doc = new AuditDocument
            {
                DocId = Guid.NewGuid(),
                AuditId = audit.AuditId,
                DocumentType = "Submitted Report",
                Title = $"{audit.Title} - Submitted Report",
                Status = "Pending",
                BlobPath = string.Empty,
                IsFinalVersion = false,
                ContentType = string.Empty,
                SizeBytes = string.Empty.Length
            };
            await _auditDocumentRepo.AddAuditDocumentAsync(doc);

            // Tạo ReportRequest
            var rr = new ReportRequest
            {
                ReportRequestId = Guid.NewGuid(),
                RequestedBy = requestedBy,
                Parameters = $"{{\"auditId\":\"{auditId}\"}}",
                Status = "Pending",
                FilePath = pdfUrl,
                RequestedAt = DateTime.UtcNow

            };
            await _reportRequestRepo.AddReportRequestAsync(rr);

            // Lưu tất cả thay đổi
            await _repo.SaveChangesAsync();
        }
    }
}
