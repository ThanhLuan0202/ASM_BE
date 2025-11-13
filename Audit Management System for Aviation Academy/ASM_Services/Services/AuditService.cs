using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditDTO;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Repositories.Models.RootCauseDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
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
        public AuditService(IAuditRepository repo, IFindingRepository findingRepo, IDepartmentRepository departmentRepo, IRootCauseRepository rootCauseRepo, IAuditDocumentRepository auditDocumentRepo, IReportRequestRepository reportRequestRepo)
        {
            _repo = repo;
            _findingRepo = findingRepo;
            _departmentRepo = departmentRepo;
            _rootCauseRepo = rootCauseRepo;
            _auditDocumentRepo = auditDocumentRepo;
            _reportRequestRepo = reportRequestRepo;
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
            var now = DateTime.UtcNow;

            var total = findings.Count;
            var open = findings.Count(f => f.Status == "Open");
            var closed = findings.Count(f => f.Status == "Closed");
            var overdue = findings.Count(f => f.Deadline != null && f.Deadline < now && f.Status != "Closed");

            var severityGroups = findings.GroupBy(f => f.Severity ?? "N/A")
                .Select(g => new { Severity = g.Key, Count = g.Count() }).ToList();

            var byDeptGroups = findings.GroupBy(f => f.DeptId)
                .Select(g => new { DeptID = g.Key, Count = g.Count() }).ToList();
            var deptIds = byDeptGroups.Select(x => x.DeptID).Where(x => x != null).Cast<int>().ToList();
            var deptNames = await _departmentRepo.GetDepartmentsAsync(deptIds);

            var byRootGroups = findings.GroupBy(f => f.RootCauseId)
                .Select(g => new { RootId = g.Key, Count = g.Count() }).ToList();
            var rootIds = byRootGroups.Select(x => x.RootId).Where(x => x != null).Cast<int>().ToList();
            var rootNames = await _rootCauseRepo.GetRootCausesAsync(rootIds);

            return new ViewAuditSummary
            {
                AuditId = audit.AuditId,
                Title = audit.Title,
                Type = audit.Type,
                Scope = audit.Scope,
                StartDate = audit.StartDate,
                EndDate = audit.EndDate,
                TotalFindings = total,
                OpenFindings = open,
                ClosedFindings = closed,
                OverdueFindings = overdue,
                SeverityBreakdown = severityGroups.ToDictionary(x => x.Severity, x => x.Count),
                ByDepartment = byDeptGroups.Select(x => new ViewDepartmentCount
                {
                    DeptName = x.DeptID != null && deptNames.ContainsKey(x.DeptID.Value) ? deptNames[x.DeptID.Value] : "Unknown",
                    Count = x.Count
                }).ToList(),
                ByRootCause = byRootGroups.Select(x => new ViewRootCauseCount
                {
                    RootCause = x.RootId != null && rootNames.ContainsKey(x.RootId.Value) ? rootNames[x.RootId.Value] : "Unknown",
                    Count = x.Count
                }).ToList()
            };
        }
        public async Task SubmitAuditAsync(Guid auditId)
        {
            var audit = await _repo.GetAuditByIdAsync(auditId);
            if (audit == null) throw new Exception("Audit not found");
            audit.Status = "Submitted";

            var doc = new AuditDocument
            {
                AuditId = audit.AuditId,
                DocumentType = "Submitted_Report",
                Title = $"{audit.Title} - Submitted Report",
                BlobPath = "",
                UploadedAt = DateTime.UtcNow,
                IsFinalVersion = true
            };
            await _auditDocumentRepo.AddAuditDocumentAsync(doc);

            var rr = new ReportRequest
            {
                ReportRequestId = Guid.NewGuid(),
                Parameters = $"{{\"auditId\":\"{auditId}\"}}",
                Status = "Submitted",
                RequestedAt = DateTime.UtcNow
            };
            await _reportRequestRepo.AddReportRequestAsync(rr);

            await _repo.SaveChangesAsync();
        }
    }
}
