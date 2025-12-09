using ASM.API.Helper;
using ASM_Repositories.Entities;
using ASM_Repositories.Helper;
using ASM_Repositories.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ASM.API.BackgroundServices
{
    public class AuditScheduleOverdueService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<AuditScheduleOverdueService> _logger;
        private readonly NotificationHelper _notificationHelper;
        private static readonly TimeSpan DailyTargetUtc = new TimeSpan(0, 1, 0);
        private static readonly TimeSpan HourlyInterval = TimeSpan.FromHours(12);

        public AuditScheduleOverdueService(
            IServiceProvider provider,
            ILogger<AuditScheduleOverdueService> logger,
            NotificationHelper notificationHelper)
        {
            _provider = provider;
            _logger = logger;
            _notificationHelper = notificationHelper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AuditScheduleOverdueService started");

            var firstRun = true;
            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = firstRun
                    ? TimeSpan.Zero
                    : ScheduleRunHelper.GetDelayUntilNextRunUtc(DateTime.UtcNow, DailyTargetUtc, HourlyInterval);
                firstRun = false;
                var nextRun = DateTime.UtcNow.Add(delay);

                _logger.LogInformation("Next overdue check scheduled at {nextRun} UTC (in {delay})", nextRun, delay);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                try
                {
                    using var scope = _provider.CreateScope();
                    var scheduleService = scope.ServiceProvider.GetRequiredService<IAuditScheduleService>();
                    var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                    var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();

                    var nowUtc = DateTime.UtcNow;
                    var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, TimeZoneHelper.TimeZone);
                    _logger.LogInformation("Running overdue check at {timeUtc} UTC ({timeLocal} local)", nowUtc, nowLocal);

                    var evidenceAuditIds = await scheduleService.MarkEvidenceDueOverdueAsync(stoppingToken);
                    var capaAuditIds = await scheduleService.MarkCapaDueOverdueAsync(stoppingToken);
                    var draftAuditIds = await scheduleService.MarkDraftReportDueOverdueAsync(stoppingToken);
                    
                    _logger.LogInformation("Overdue check results - Evidence: {evidenceCount}, CAPA: {capaCount}, Draft: {draftCount}", 
                        evidenceAuditIds.Count, capaAuditIds.Count, draftAuditIds.Count);
                    
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                    var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneHelper.TimeZone);
                    var todayStartLocal = localNow.Date;
                    var todayStartUtc = TimeZoneInfo.ConvertTimeToUtc(todayStartLocal, TimeZoneHelper.TimeZone);

                    // Lấy Lead Auditor ID
                    var leadAuditorId = await usersService.GetLeadAuditorIdAsync();
                    if (leadAuditorId == null)
                    {
                        _logger.LogWarning("No lead auditor found in system when sending overdue notifications");
                        return;
                    }

                    foreach (var auditId in draftAuditIds)
                    {
                        try
                        {
                            var audit = await auditService.GetAuditByIdAsync(auditId);
                            var auditTitle = audit?.Title ?? "Unknown Audit";
                            
                            var title = "Draft report is overdue";
                            var exists = await notificationService.NotificationExistsAsync(
                                title,
                                leadAuditorId.Value,
                                auditId,
                                "Audit",
                                todayStartUtc);

                            if (!exists)
                            {
                                var notif = await notificationService.CreateNotificationAsync(new Notification
                                {
                                    UserId = leadAuditorId.Value,
                                    Title = title,
                                    Message = $"Draft report for audit '{auditTitle}' is now overdue.",
                                    EntityType = "Audit",
                                    EntityId = auditId,
                                    IsRead = false,
                                    Status = "Active",
                                });

                                await _notificationHelper.SendToUserAsync(leadAuditorId.Value.ToString(), notif);
                                _logger.LogInformation("Notification sent for Draft report overdue - Audit: {auditId}, Lead Auditor: {userId}", 
                                    auditId, leadAuditorId.Value);
                            }
                            else
                            {
                                _logger.LogInformation("Notification already exists for Draft report overdue - Audit: {auditId}, Lead Auditor: {userId}", 
                                    auditId, leadAuditorId.Value);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error sending Draft report overdue notification for audit {auditId}", auditId);
                        }
                    }

                    foreach (var auditId in capaAuditIds)
                    {
                        try
                        {
                            var audit = await auditService.GetAuditByIdAsync(auditId);
                            var auditTitle = audit?.Title ?? "Unknown Audit";
                            
                            var title = "CAPA is overdue";
                            var exists = await notificationService.NotificationExistsAsync(
                                title,
                                leadAuditorId.Value,
                                auditId,
                                "Audit",
                                todayStartUtc);

                            if (!exists)
                            {
                                var notif = await notificationService.CreateNotificationAsync(new Notification
                                {
                                    UserId = leadAuditorId.Value,
                                    Title = title,
                                    Message = $"CAPA for audit '{auditTitle}' is now overdue.",
                                    EntityType = "Audit",
                                    EntityId = auditId,
                                    IsRead = false,
                                    Status = "Active",
                                });

                                await _notificationHelper.SendToUserAsync(leadAuditorId.Value.ToString(), notif);
                                _logger.LogInformation("Notification sent for CAPA overdue - Audit: {auditId}, Lead Auditor: {userId}", 
                                    auditId, leadAuditorId.Value);
                            }
                            else
                            {
                                _logger.LogInformation("Notification already exists for CAPA overdue - Audit: {auditId}, Lead Auditor: {userId}", 
                                    auditId, leadAuditorId.Value);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error sending CAPA overdue notification for audit {auditId}", auditId);
                        }
                    }

                    foreach (var auditId in evidenceAuditIds)
                    {
                        try
                        {
                            var audit = await auditService.GetAuditByIdAsync(auditId);
                            var auditTitle = audit?.Title ?? "Unknown Audit";
                            
                            var title = "Evidence is overdue";
                            var exists = await notificationService.NotificationExistsAsync(
                                title,
                                leadAuditorId.Value,
                                auditId,
                                "Audit",
                                todayStartUtc);

                            if (!exists)
                            {
                                var notif = await notificationService.CreateNotificationAsync(new Notification
                                {
                                    UserId = leadAuditorId.Value,
                                    Title = title,
                                    Message = $"Evidence for audit '{auditTitle}' is now overdue.",
                                    EntityType = "Audit",
                                    EntityId = auditId,
                                    IsRead = false,
                                    Status = "Active",
                                });

                                await _notificationHelper.SendToUserAsync(leadAuditorId.Value.ToString(), notif);
                                _logger.LogInformation("Notification sent for Evidence overdue - Audit: {auditId}, Lead Auditor: {userId}", 
                                    auditId, leadAuditorId.Value);
                            }
                            else
                            {
                                _logger.LogInformation("Notification already exists for Evidence overdue - Audit: {auditId}, Lead Auditor: {userId}", 
                                    auditId, leadAuditorId.Value);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error sending Evidence overdue notification for audit {auditId}", auditId);
                        }
                    }

                    var draftDueTomorrow = await scheduleService.GetDraftReportDueTomorrowAssignmentsAsync(stoppingToken);
                    var capaDueTomorrow = await scheduleService.GetCapaDueTomorrowAssignmentsAsync(stoppingToken);
                    var evidenceDueTomorrow = await scheduleService.GetEvidenceDueTomorrowAssignmentsAsync(stoppingToken);

                    _logger.LogInformation(
                        "Overdue update completed. Evidence updated: {evidenceCount}, CAPA updated: {capaCount}, Draft updated: {draftCount}, Draft due tomorrow notifications: {draftDueTomorrowCount}, CAPA due tomorrow notifications: {capaDueTomorrowCount}, Evidence due tomorrow notifications: {evidenceDueTomorrowCount}",
                        evidenceAuditIds.Count,
                        capaAuditIds.Count,
                        draftAuditIds.Count,
                        draftDueTomorrow.Count,
                        capaDueTomorrow.Count,
                        evidenceDueTomorrow.Count);

                    var draftGroupedByAudit = draftDueTomorrow.GroupBy(x => x.AuditId);

                    foreach (var group in draftGroupedByAudit)
                    {
                        var auditId = group.Key;
                        var audit = await auditService.GetAuditByIdAsync(auditId);
                        var auditTitle = audit?.Title ?? "Unknown Audit";
                        var title = "Draft report due tomorrow";
                        
                        var dueDate = group.First().DueDate;
                        var auditorIds = group.Select(x => x.AuditorId).Distinct().ToList();

                        foreach (var auditorId in auditorIds)
                        {
                            var exists = await notificationService.NotificationExistsAsync(
                                title, 
                                auditorId, 
                                auditId, 
                                "Audit", 
                                todayStartUtc);
                            
                            if (!exists)
                            {
                                var notif = await notificationService.CreateNotificationAsync(new Notification
                                {
                                    UserId = auditorId,
                                    Title = title,
                                    Message = $"Audit draft report for audit '{auditTitle}' is due on {dueDate:yyyy-MM-dd HH:mm} UTC.",
                                    EntityType = "Audit",
                                    EntityId = auditId,
                                    IsRead = false,
                                    Status = "Active",
                                });

                                await _notificationHelper.SendToUserAsync(auditorId.ToString(), notif);
                            }
                        }

                        if (leadAuditorId.HasValue && !auditorIds.Contains(leadAuditorId.Value))
                        {
                            var leadTitle = "Draft report due tomorrow";
                            var leadExists = await notificationService.NotificationExistsAsync(
                                leadTitle,
                                leadAuditorId.Value,
                                auditId,
                                "Audit",
                                todayStartUtc);

                            if (!leadExists)
                            {
                                var leadNotif = await notificationService.CreateNotificationAsync(new Notification
                                {
                                    UserId = leadAuditorId.Value,
                                    Title = leadTitle,
                                    Message = $"Draft report for audit '{auditTitle}' is due on {dueDate:yyyy-MM-dd HH:mm} UTC.",
                                    EntityType = "Audit",
                                    EntityId = auditId,
                                    IsRead = false,
                                    Status = "Active",
                                });

                                await _notificationHelper.SendToUserAsync(leadAuditorId.Value.ToString(), leadNotif);
                            }
                        }
                    }

                    var capaGroupedByAudit = capaDueTomorrow.GroupBy(x => x.AuditId);

                    foreach (var group in capaGroupedByAudit)
                    {
                        var auditId = group.Key;
                        var audit = await auditService.GetAuditByIdAsync(auditId);
                        var auditTitle = audit?.Title ?? "Unknown Audit";
                        var title = "CAPA due tomorrow";
                        
                        var dueDate = group.First().DueDate;
                        var auditorIds = group.Select(x => x.AuditorId).Distinct().ToList();

                        foreach (var auditorId in auditorIds)
                        {
                            var exists = await notificationService.NotificationExistsAsync(
                                title, 
                                auditorId, 
                                auditId, 
                                "Audit", 
                                todayStartUtc);
                            
                            if (!exists)
                            {
                                var notif = await notificationService.CreateNotificationAsync(new Notification
                                {
                                    UserId = auditorId,
                                    Title = title,
                                    Message = $"CAPA for audit '{auditTitle}' is due on {dueDate:yyyy-MM-dd HH:mm} UTC.",
                                    EntityType = "Audit",
                                    EntityId = auditId,
                                    IsRead = false,
                                    Status = "Active",
                                });

                                await _notificationHelper.SendToUserAsync(auditorId.ToString(), notif);
                            }
                        }

                        if (leadAuditorId.HasValue && !auditorIds.Contains(leadAuditorId.Value))
                        {
                            var leadTitle = "CAPA due tomorrow";
                            var leadExists = await notificationService.NotificationExistsAsync(
                                leadTitle,
                                leadAuditorId.Value,
                                auditId,
                                "Audit",
                                todayStartUtc);

                            if (!leadExists)
                            {
                                var leadNotif = await notificationService.CreateNotificationAsync(new Notification
                                {
                                    UserId = leadAuditorId.Value,
                                    Title = leadTitle,
                                    Message = $"CAPA for audit '{auditTitle}' is due on {dueDate:yyyy-MM-dd HH:mm} UTC.",
                                    EntityType = "Audit",
                                    EntityId = auditId,
                                    IsRead = false,
                                    Status = "Active",
                                });

                                await _notificationHelper.SendToUserAsync(leadAuditorId.Value.ToString(), leadNotif);
                            }
                        }
                    }

                    var evidenceGroupedByAudit = evidenceDueTomorrow.GroupBy(x => x.AuditId);

                    foreach (var group in evidenceGroupedByAudit)
                    {
                        var auditId = group.Key;
                        var audit = await auditService.GetAuditByIdAsync(auditId);
                        var auditTitle = audit?.Title ?? "Unknown Audit";
                        var title = "Evidence due tomorrow";
                        
                        var dueDate = group.First().DueDate;
                        var auditorIds = group.Select(x => x.AuditorId).Distinct().ToList();

                        foreach (var auditorId in auditorIds)
                        {
                            var exists = await notificationService.NotificationExistsAsync(
                                title, 
                                auditorId, 
                                auditId, 
                                "Audit", 
                                todayStartUtc);
                            
                            if (!exists)
                            {
                                var notif = await notificationService.CreateNotificationAsync(new Notification
                                {
                                    UserId = auditorId,
                                    Title = title,
                                    Message = $"Evidence for audit '{auditTitle}' is due on {dueDate:yyyy-MM-dd HH:mm} UTC.",
                                    EntityType = "Audit",
                                    EntityId = auditId,
                                    IsRead = false,
                                    Status = "Active",
                                });

                                await _notificationHelper.SendToUserAsync(auditorId.ToString(), notif);
                            }
                        }

                        if (leadAuditorId.HasValue && !auditorIds.Contains(leadAuditorId.Value))
                        {
                            var leadTitle = "Evidence due tomorrow";
                            var leadExists = await notificationService.NotificationExistsAsync(
                                leadTitle,
                                leadAuditorId.Value,
                                auditId,
                                "Audit",
                                todayStartUtc);

                            if (!leadExists)
                            {
                                var leadNotif = await notificationService.CreateNotificationAsync(new Notification
                                {
                                    UserId = leadAuditorId.Value,
                                    Title = leadTitle,
                                    Message = $"Evidence for audit '{auditTitle}' is due on {dueDate:yyyy-MM-dd HH:mm} UTC.",
                                    EntityType = "Audit",
                                    EntityId = auditId,
                                    IsRead = false,
                                    Status = "Active",
                                });

                                await _notificationHelper.SendToUserAsync(leadAuditorId.Value.ToString(), leadNotif);
                            }
                        }
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("AuditScheduleOverdueService is stopping...");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while processing overdue schedules");
                }
            }
        }

    }
}

