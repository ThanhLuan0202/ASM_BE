using ASM.API.Helper;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
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
        private static readonly TimeSpan HourlyInterval = TimeSpan.FromHours(1);

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
                    var repo = scope.ServiceProvider.GetRequiredService<IAuditScheduleRepository>();

                    _logger.LogInformation("Running overdue check at {time} UTC", DateTime.UtcNow);

                    var evidenceUpdated = await repo.MarkEvidenceDueOverdueAsync(stoppingToken);
                    var capaUpdated = await repo.MarkCapaDueOverdueAsync(stoppingToken);
                    var draftUpdated = await repo.MarkDraftReportDueOverdueAsync(stoppingToken);
                    var draftDueTomorrow = await repo.GetDraftReportDueTomorrowAssignmentsAsync(stoppingToken);
                    var capaDueTomorrow = await repo.GetCapaDueTomorrowAssignmentsAsync(stoppingToken);
                    var evidenceDueTomorrow = await repo.GetEvidenceDueTomorrowAssignmentsAsync(stoppingToken);
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    _logger.LogInformation(
                        "Overdue update completed. Evidence updated: {evidenceCount}, CAPA updated: {capaCount}, Draft updated: {draftCount}, Draft due tomorrow notifications: {draftDueTomorrowCount}, CAPA due tomorrow notifications: {capaDueTomorrowCount}, Evidence due tomorrow notifications: {evidenceDueTomorrowCount}",
                        evidenceUpdated,
                        capaUpdated,
                        draftUpdated,
                        draftDueTomorrow.Count,
                        capaDueTomorrow.Count,
                        evidenceDueTomorrow.Count);

                    foreach (var (auditId, auditorId, dueDate) in draftDueTomorrow)
                    {
                        var notif = await notificationService.CreateNotificationAsync(new Notification
                        {
                            UserId = auditorId,
                            Title = "Draft report due tomorrow",
                            Message = $"Audit draft report is due on {dueDate:yyyy-MM-dd HH:mm} UTC.",
                            EntityType = "Audit",
                            EntityId = auditId,
                            IsRead = false,
                            Status = "Active",
                        });

                        await _notificationHelper.SendToUserAsync(auditorId.ToString(), notif);
                    }

                    foreach (var (auditId, auditorId, dueDate) in capaDueTomorrow)
                    {
                        var notif = await notificationService.CreateNotificationAsync(new Notification
                        {
                            UserId = auditorId,
                            Title = "CAPA due tomorrow",
                            Message = $"CAPA is due on {dueDate:yyyy-MM-dd HH:mm} UTC.",
                            EntityType = "Audit",
                            EntityId = auditId,
                            IsRead = false,
                            Status = "Active",
                        });

                        await _notificationHelper.SendToUserAsync(auditorId.ToString(), notif);
                    }

                    foreach (var (auditId, auditorId, dueDate) in evidenceDueTomorrow)
                    {
                        var notif = await notificationService.CreateNotificationAsync(new Notification
                        {
                            UserId = auditorId,
                            Title = "Evidence due tomorrow",
                            Message = $"Evidence is due on {dueDate:yyyy-MM-dd HH:mm} UTC.",
                            EntityType = "Audit",
                            EntityId = auditId,
                            IsRead = false,
                            Status = "Active",
                        });

                        await _notificationHelper.SendToUserAsync(auditorId.ToString(), notif);
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

