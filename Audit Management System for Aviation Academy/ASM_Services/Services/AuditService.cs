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
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

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
        private readonly IAuditCriteriaMapRepository _auditCriteriaMapRepo;
        private readonly IAuditTeamRepository _auditTeamRepo;
        private readonly IAuditScheduleRepository _auditScheduleRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IUsersRepository _userRepo;

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
            IAttachmentRepository attachmentRepo,
            IAuditCriteriaMapRepository auditCriteriaMapRepo,
            IAuditTeamRepository auditTeamRepo,
            IAuditScheduleRepository auditScheduleRepo,
            INotificationRepository notificationRepo,
            IUsersRepository userRepo)
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
            _auditCriteriaMapRepo = auditCriteriaMapRepo;
            _auditTeamRepo = auditTeamRepo;
            _auditScheduleRepo = auditScheduleRepo;
            _notificationRepo = notificationRepo;
            _userRepo = userRepo;
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

        public async Task<Notification> SubmitToLeadAuditorAsync(Guid auditId, Guid userBy)
        {
            var audit = await _repo.GetAuditByIdAsync(auditId);
            if (audit == null)
                throw new Exception("Audit not found");

            var updated = await _repo.SubmitToLeadAuditorAsync(auditId);
            if (!updated)
                throw new Exception($"Submit failed for audit {auditId}.");

            await NotifyLeadAuditorsAsync(auditId);

            var user = await _userRepo.GetUserShortInfoAsync(userBy);
            if (user == null)
                throw new Exception("User not found");

            var leadId = await _userRepo.GetLeadAuditorIdAsync();
            if (leadId == null)
                throw new Exception("LeadId not found for this Audit");

            var notif = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = leadId.Value,
                Title = "Audit Plan has been created by Auditor",
                Message = $"Audit '{audit.Title}' has been submitted to you by {user.FullName} ({user.RoleName}).\n" +
                        "Please proceed with the next steps.",
                EntityType = "Audit",
                EntityId = auditId,
                IsRead = false,
                Status = "Active",
            });

            return notif;
        }

        public async Task<List<Notification>> ApproveAndForwardToDirectorAsync(Guid auditId, Guid approverId, string comment)
        {
            var audit = await _repo.GetAuditByIdAsync(auditId);
            if (audit == null)
                throw new Exception("Audit not found");

            if (audit.CreatedBy == null)
                throw new Exception("Audit CreatedBy is null");

            await _repo.ApproveAndForwardToDirectorAsync(auditId, approverId, comment);

            var user = await _userRepo.GetUserShortInfoAsync(approverId);
            if (user == null)
                throw new Exception("User not found");

            var directorId = await _userRepo.GetDirectorIdAsync();
            if (directorId == null)
                throw new Exception("Director not found");

            var directorInfo = await _userRepo.GetUserShortInfoAsync(directorId.Value);
            if (directorInfo == null || string.IsNullOrWhiteSpace(directorInfo.Email))
                throw new Exception("Director contact information not found");

            var notif1 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = directorId.Value,
                Title = "Audit Plan Requires Your Review",
                Message = $"The audit plan '{audit.Title}' has completed Lead Auditor approval and has been forwarded to you by {user.FullName} ({user.RoleName}).\n" +
                        "Your review and approval are kindly requested.",
                EntityType = "Audit",
                EntityId = auditId,
                IsRead = false,
                Status = "Active",
            });

            var notif2 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = audit.CreatedBy.Value,
                Title = "Your Audit Plan Has Been Approved",
                Message = $"Your audit plan '{audit.Title}' has been approved by {user.FullName} ({user.RoleName}) and forwarded to Director for review.",
                EntityType = "Audit",
                EntityId = auditId,
                IsRead = false,
                Status = "Active",
            });

            await _emailService.SendAuditPlanForwardedToDirectorAsync(
                directorInfo.Email,
                directorInfo.FullName,
                audit.Title ?? "Audit Plan",
                user.FullName,
                user.RoleName,
                comment);

            return new List<Notification> { notif1, notif2 };
        }

        public async Task<Notification> DeclinedPlanContentAsync(Guid auditId, Guid approverId, string comment)
        {
            var audit = await _repo.GetAuditByIdAsync(auditId);
            if (audit == null)
                throw new Exception("Audit not found");

            if(audit.CreatedBy == null)
                throw new Exception("Audit CreatedBy is null");

            await _repo.DeclinedPlanContentAsync(auditId, approverId, comment);

            var user = await _userRepo.GetUserShortInfoAsync(approverId);
            if (user == null)
                throw new Exception("User not found");

            var notif = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = audit.CreatedBy.Value,
                Title = "Your Audit Plan Has Been Declined",
                Message = $"Your audit plan '{audit.Title}' has been declined by {user.FullName} ({user.RoleName}).\n" +
                        $"Reason: {comment}",
                EntityType = "Audit",
                EntityId = auditId,
                IsRead = false,
                Status = "Active",
            });

            return notif;
        }



        public async Task<List<Notification>> ApprovePlanAsync(Guid auditId, Guid approverId, string comment)
        {
            var audit = await _repo.GetAuditByIdAsync(auditId);
            if (audit == null)
                throw new Exception("Audit not found");

            if (audit.CreatedBy == null)
                throw new Exception("Audit CreatedBy is null");

            await _repo.ApprovePlanAsync(auditId, approverId, comment);

            var user = await _userRepo.GetUserShortInfoAsync(approverId);
            if (user == null)
                throw new Exception("User not found");

            var notifications = new List<Notification>();
            var emailTasks = new List<Task>();
            var auditTitle = string.IsNullOrWhiteSpace(audit.Title) ? "Audit Plan" : audit.Title;

            var notif1 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = audit.CreatedBy.Value,
                Title = "Your Audit Plan Has Been Approved By Director",
                Message = $"Your audit plan '{audit.Title}' has been approved by {user.FullName} ({user.RoleName}).",
                EntityType = "Audit",
                EntityId = auditId,
                IsRead = false,
                Status = "Active",
            });
            notifications.Add(notif1);

            var creatorInfo = await _userRepo.GetUserShortInfoAsync(audit.CreatedBy.Value);
            if (creatorInfo != null && !string.IsNullOrWhiteSpace(creatorInfo.Email))
            {
                emailTasks.Add(_emailService.SendAuditPlanApprovedForCreatorAsync(
                    creatorInfo.Email,
                    creatorInfo.FullName,
                    auditTitle,
                    user.FullName,
                    audit.StartDate,
                    comment));
            }

            var leadId = await _userRepo.GetLeadAuditorIdAsync();
            if (leadId == null)
                throw new Exception("LeadId not found for this Audit");

            var leadInfo = await _userRepo.GetUserShortInfoAsync(leadId.Value);

            var notif2 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = leadId.Value,
                Title = "Your Audit Plan Has Been Approved By Director",
                Message = $"Your audit plan '{audit.Title}' has been approve by {user.FullName} ({user.RoleName}).",
                EntityType = "Audit",
                EntityId = auditId,
                IsRead = false,
                Status = "Active",
            });
            notifications.Add(notif2);

            if (leadInfo != null && !string.IsNullOrWhiteSpace(leadInfo.Email))
            {
                emailTasks.Add(_emailService.SendAuditPlanApprovedForLeadAsync(
                    leadInfo.Email,
                    leadInfo.FullName,
                    auditTitle,
                    user.FullName,
                    audit.StartDate,
                    comment));
            }

            var auditors = await _auditTeamRepo.GetAuditorsByAuditIdAsync(auditId);
            foreach (var auditor in auditors)
            {
                if (string.IsNullOrWhiteSpace(auditor.Email))
                    continue;

                emailTasks.Add(_emailService.SendAuditPlanApprovedForAuditorAsync(
                    auditor.Email,
                    auditor.FullName,
                    auditTitle,
                    leadInfo?.FullName ?? "Lead Auditor",
                    user.FullName,
                    audit.StartDate,
                    comment));
            }

            var auditScopeDepts = await _auditScopeDepartmentRepo.GetAuditScopeDepartmentsAsync(auditId);

            foreach (var scope in auditScopeDepts)
            {
                var auditeeOwnerInfo = await _userRepo.GetAuditeeOwnerInfoByDepartmentIdAsync(scope.DeptId);

                if (auditeeOwnerInfo == null || auditeeOwnerInfo.UserId == Guid.Empty) continue;
                if (!await _userRepo.UserExistsAsync(auditeeOwnerInfo.UserId)) continue;

                var notifDept = await _notificationRepo.CreateNotificationAsync(new Notification
                {
                    UserId = auditeeOwnerInfo.UserId,
                    Title = "New Audit Assigned to Your Department",
                    Message = $"The audit plan '{audit.Title}' has been approved and assigned to your department ({scope.Dept.Name}).\n" +
                              "Please prepare for the upcoming audit activities.",
                    EntityType = "Audit",
                    EntityId = auditId,
                    IsRead = false,
                    Status = "Active"
                });
                notifications.Add(notifDept);

                if (string.IsNullOrWhiteSpace(auditeeOwnerInfo.Email))
                    continue;

                emailTasks.Add(_emailService.SendAuditPlanApprovedForDepartmentHeadAsync(
                    auditeeOwnerInfo.Email,
                    auditeeOwnerInfo.FullName,
                    scope.Dept?.Name ?? $"Department {scope.DeptId}",
                    auditTitle,
                    audit.StartDate,
                    comment));
            }

            if (emailTasks.Any())
            {
                try
                {
                    await Task.WhenAll(emailTasks);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send audit plan approval emails for audit {AuditId}", auditId);
                }
            }

            return notifications;

        }
        public async Task<List<Notification>> RejectPlanContentAsync(Guid auditId, Guid approverId, string comment)
        {
            var audit = await _repo.GetAuditByIdAsync(auditId);
            if (audit == null)
                throw new Exception("Audit not found");

            if (audit.CreatedBy == null)
                throw new Exception("Audit CreatedBy is null");

            await _repo.RejectPlanContentAsync(auditId, approverId, comment);

            var user = await _userRepo.GetUserShortInfoAsync(approverId);
            if (user == null)
                throw new Exception("User not found");

            var notif1 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = audit.CreatedBy.Value,
                Title = "Your Audit Plan Has Been Rejected By Director",
                Message = $"Your audit plan '{audit.Title}' has been rejected by {user.FullName} ({user.RoleName}).\n" +
                        $"Reason: {comment}",
                EntityType = "Audit",
                EntityId = auditId,
                IsRead = false,
                Status = "Active",
            });

            var leadId = await _userRepo.GetLeadAuditorIdAsync();
            if (leadId == null)
                throw new Exception("LeadId not found for this Audit");

            var notif2 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = leadId.Value,
                Title = "Your Audit Plan Has Been Rejected By Director",
                Message = $"Your audit plan '{audit.Title}' has been rejected by {user.FullName} ({user.RoleName}).\n" +
                        $"Reason: {comment}",
                EntityType = "Audit",
                EntityId = auditId,
                IsRead = false,
                Status = "Active",
            });

            // Send email to creator (auditor who created the audit)
            var creatorInfo = await _userRepo.GetUserShortInfoAsync(audit.CreatedBy.Value);
            if (creatorInfo != null && !string.IsNullOrWhiteSpace(creatorInfo.Email))
            {
                try
                {
                    await _emailService.SendAuditPlanRejectedForCreatorAsync(
                        creatorInfo.Email,
                        creatorInfo.FullName,
                        audit.Title,
                        user.FullName,
                        comment);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send rejection email to creator for audit {AuditId}", auditId);
                }
            }

            return new List<Notification> { notif1, notif2 };
        }

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



        public async Task<Notification> SubmitAuditAsync(Guid auditId, string pdfUrl, Guid requestedBy)
        {
            var audit = await _repo.GetAuditByIdAsync(auditId);
            if (audit == null) throw new Exception("Audit not found");

            await _repo.UpdateStatusByAuditIdAsync(auditId, "Submitted");

            var rr = await _reportRequestRepo.CreateReportRequestAsync(auditId, pdfUrl, requestedBy);

            var user = await _userRepo.GetUserShortInfoAsync(requestedBy);
            if (user == null)
                throw new Exception("User not found");

            var leadId = await _userRepo.GetLeadAuditorIdAsync();
            if (leadId == null)
                throw new Exception("LeadId not found for this Audit");

            var notif = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = leadId.Value,
                Title = "Audit Report Submitted – Review Required",
                Message = $"Audit report for '{audit.Title}' has been submitted by {user.FullName} ({user.RoleName}).\n" +
                        "Kindly review and provide your feedback.",
                EntityType = "ReportRequest",
                EntityId = rr.ReportRequestId,
                IsRead = false,
                Status = "Active",
            });

            return notif;
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

        public async Task<Notification> ReportApproveAsync(Guid auditId, Guid userBy, string note)
        {
            await _repo.UpdateStatusByAuditIdAsync(auditId, "InProgress");
            await _reportRequestRepo.UpdateStatusAndNoteByAuditIdAsync(auditId, "Approved", note);

            var user = await _userRepo.GetUserShortInfoAsync(userBy);
            if (user == null)
                throw new Exception("User not found");

            var report = await _reportRequestRepo.GetReportByAuditIdAsync(auditId);
            if (report?.RequestedBy == null)
                throw new Exception("Report requested by unknown user");

            var audit = await _repo.GetAuditByIdAsync(auditId);
            if (audit == null)
                throw new Exception("Audit not found");

            var notif = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = report.RequestedBy.Value,
                Title = "Your Audit Report has been approved by Auditor",
                Message = $"Your audit report for the action '{audit?.Title}' has been reviewed and approved by {user.FullName} ({user.RoleName}).\n" +
                        "You may now proceed to print the report and submit it for further signature or processing.\n" +
                        (!string.IsNullOrWhiteSpace(note) ? $"\nRemarks: {note}" : ""),
                EntityType = "ReportRequest",
                EntityId = report.ReportRequestId,
                IsRead = false,
                Status = "Active",
            });

            return notif;
        }

        public async Task<Notification> ReportRejectedAsync(Guid auditId, Guid userBy, string note)
        {
            await _repo.UpdateStatusByAuditIdAsync(auditId, "Returned");
            await _reportRequestRepo.UpdateStatusAndNoteByAuditIdAsync(auditId, "Rejected", note);

            var user = await _userRepo.GetUserShortInfoAsync(userBy);
            if (user == null)
                throw new Exception("User not found");

            var report = await _reportRequestRepo.GetReportByAuditIdAsync(auditId);
            if (report?.RequestedBy == null)
                throw new Exception("Report requested by unknown user");

            var audit = await _repo.GetAuditByIdAsync(auditId);
            if (audit == null)
                throw new Exception("Audit not found");

            var notif = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = report.RequestedBy.Value,
                Title = "Your Audit Report Has Been Returned for Revision",
                Message = $"Your audit report for the action '{audit.Title}' has been reviewed by {user.FullName} ({user.RoleName}) and requires revision.\n" +
                        "Please update the report according to the feedback provided and resubmit it for further review.\n" +
                        (!string.IsNullOrWhiteSpace(note) ? $"\nRemarks: {note}" : ""),
                EntityType = "ReportRequest",
                EntityId = report.ReportRequestId,
                IsRead = false,
                Status = "Active",
            });

            return notif;
        }

        public async Task<bool> UpdateAuditPlanAsync(Guid auditId, UpdateAuditPlan request)
        {
            var auditPlan = await _repo.GetAuditPlanAsync(auditId);

            if (auditPlan == null)
                return false;

            await _repo.UpdateAuditPlanAsync(auditPlan, request.Audit);

            await _auditScopeDepartmentRepo.UpdateScopeDepartmentsAsync(auditId, request.ScopeDepartments);

            await _auditCriteriaMapRepo.UpdateCriteriaMapAsync(auditId, request.Criteria);

            await _auditTeamRepo.UpdateAuditTeamsAsync(auditId, request.AuditTeams);

            await _auditScheduleRepo.UpdateSchedulesAsync(auditId, request.Schedules);

            await _repo.SaveChangesAsync();

            return true;
        }

    }
}
