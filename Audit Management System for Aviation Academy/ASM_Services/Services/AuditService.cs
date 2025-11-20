using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditDTO;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Repositories.Models.FindingDTO;
using ASM_Repositories.Models.RootCauseDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using ASM_Services.Models.Email;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
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
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuditService> _logger;
        private readonly IAttachmentRepository _attachmentRepo;

        public AuditService(
            IAuditRepository repo,
            IFindingRepository findingRepo,
            IDepartmentRepository departmentRepo,
            IRootCauseRepository rootCauseRepo,
            IAuditDocumentRepository auditDocumentRepo,
            IReportRequestRepository reportRequestRepo,
            IAuditScopeDepartmentRepository auditScopeDepartmentRepo,
            IMapper mapper,
            IEmailService emailService,
            ILogger<AuditService> logger,
            IAttachmentRepository attachmentRepo)
        {
            _repo = repo;
            _findingRepo = findingRepo;
            _departmentRepo = departmentRepo;
            _rootCauseRepo = rootCauseRepo;
            _auditDocumentRepo = auditDocumentRepo;
            _reportRequestRepo = reportRequestRepo;
            _auditScopeDepartmentRepo = auditScopeDepartmentRepo;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
            _attachmentRepo = attachmentRepo;
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

        public async Task<bool> SubmitToLeadAuditorAsync(Guid auditId)
        {
            var updated = await _repo.SubmitToLeadAuditorAsync(auditId);
            if (!updated)
            {
                return false;
            }

            await NotifyLeadAuditorsAsync(auditId);
            return true;
        }

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

            var findingIds = findingsThisMonth.Select(f => f.FindingId).ToList();
            var attachments = await _attachmentRepo.GetAttachmentsByFindingIdAsync(findingIds);

            // Mapping Findings
            var mappedFindings = _mapper.Map<List<ViewFindingDetail>>(findingsThisMonth);

            foreach (var f in mappedFindings)
            {
                f.Attachments = attachments.Where(a => a.EntityId == f.FindingId).ToList();
            }

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

            await _repo.UpdateStatusByAuditIdAsync(auditId, "Submitted");
            /*
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
            */
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

            await _repo.SaveChangesAsync();
        }

        private async Task NotifyLeadAuditorsAsync(Guid auditId)
        {
            try
            {
                var audit = await _repo.GetAuditByIdAsync(auditId);
                if (audit == null)
                {
                    _logger.LogWarning("Audit {AuditId} no longer exists when attempting to notify lead auditors.", auditId);
                    return;
                }

                var contacts = await _repo.GetLeadAuditorAsync(auditId);




                await _emailService.SendForLeadAuditor(contacts.Email, audit.Title, contacts.FullName, audit.StartDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send Lead Auditor notification for audit {AuditId}", auditId);
            }
        }

        private static string BuildLeadAuditorEmailBody(ViewAudit audit)
        {
            var title = WebUtility.HtmlEncode(audit.Title ?? "Audit Plan");
            var scope = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(audit.Scope) ? "Updating soon" : audit.Scope);
            var objective = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(audit.Objective) ? "Updating soon" : audit.Objective);
            var start = audit.StartDate?.ToString("dd MMM yyyy") ?? "Updating soon";
            var end = audit.EndDate?.ToString("dd MMM yyyy") ?? "Updating soon";

            var sb = new StringBuilder();
            sb.AppendLine("<div style=\"font-family:'Segoe UI',Arial,sans-serif;font-size:14px;color:#1a1a1a;line-height:1.6;\">");
            sb.AppendLine("<h2 style=\"color:#0F5FFF;margin-bottom:8px;\">Audit submission pending your review</h2>");
            sb.AppendLine($"<p>The audit plan <strong>{title}</strong> has just been submitted and is waiting for your leadership review.</p>");
            sb.AppendLine("<div style=\"background-color:#f5f7fb;border-radius:8px;padding:16px;margin:18px 0;\">");
            sb.AppendLine("<table style=\"width:100%;border-collapse:collapse;font-size:14px;\">");
            sb.AppendLine($"<tr><td style=\"padding:6px 0;color:#6b7280;width:140px;\">Audit Window</td><td style=\"padding:6px 0;color:#111827;\"><strong>{start}</strong> ➜ <strong>{end}</strong></td></tr>");
            sb.AppendLine($"<tr><td style=\"padding:6px 0;color:#6b7280;\">Scope</td><td style=\"padding:6px 0;color:#111827;\">{scope}</td></tr>");
            sb.AppendLine($"<tr><td style=\"padding:6px 0;color:#6b7280;\">Objective</td><td style=\"padding:6px 0;color:#111827;\">{objective}</td></tr>");
            sb.AppendLine($"<tr><td style=\"padding:6px 0;color:#6b7280;\">Current Status</td><td style=\"padding:6px 0;color:#111827;\"><span style=\"display:inline-block;padding:4px 10px;border-radius:999px;background:#dbeafe;color:#1d4ed8;font-weight:600;\">Pending Review</span></td></tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");
            sb.AppendLine("<p style=\"margin-bottom:18px;\">Please sign in to the Audit Management System to review the plan, leave comments, and provide your approval.</p>");
            sb.AppendLine("<p style=\"margin:0;color:#6b7280;font-size:13px;\">Thank you for keeping our assurance roadmap on track.</p>");
            sb.AppendLine("<p style=\"margin-top:6px;font-weight:600;\">Audit Management System</p>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        private static string BuildLeadAuditorPlainText(ViewAudit audit)
        {
            var start = audit.StartDate?.ToString("dd MMM yyyy") ?? "Updating soon";
            var end = audit.EndDate?.ToString("dd MMM yyyy") ?? "Updating soon";
            var title = audit.Title ?? "Audit Plan";
            var scope = string.IsNullOrWhiteSpace(audit.Scope) ? "Updating soon" : audit.Scope;
            var objective = string.IsNullOrWhiteSpace(audit.Objective) ? "Updating soon" : audit.Objective;

            var sb = new StringBuilder();
            sb.AppendLine($"Audit \"{title}\" is pending Lead Auditor review.");
            sb.AppendLine($"Window: {start} - {end}");
            sb.AppendLine($"Scope: {scope}");
            sb.AppendLine($"Objective: {objective}");
            sb.AppendLine();
            sb.AppendLine("Please sign in to the Audit Management System to review and respond.");
            return sb.ToString();
        }

        public async Task UpdateReportStatusAsync(Guid auditId, string statusAudit, string statusDoc)
        {
            var audit = await _repo.UpdateStatusByAuditIdAsync(auditId, statusAudit);
            var doc = await _auditDocumentRepo.UpdateStatusByAuditIdAsync(auditId, statusDoc);
            var rr = await _reportRequestRepo.UpdateStatusByAuditIdAsync(auditId, statusDoc);
        }

        public async Task UpdateReportStatusAndNoteAsync(Guid auditId, string statusAudit, string statusDoc, string note)
        {
            var audit = await _repo.UpdateStatusByAuditIdAsync(auditId, statusAudit);
            var doc = await _auditDocumentRepo.UpdateStatusByAuditIdAsync(auditId, statusDoc);
            var rr = await _reportRequestRepo.UpdateStatusAndNoteByAuditIdAsync(auditId, statusDoc, note);
        }
    }
}
