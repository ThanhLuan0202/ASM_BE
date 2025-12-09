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
        private readonly INotificationService _notificationService;
        private static readonly TimeSpan DailyTargetUtc = new TimeSpan(0, 1, 0);
        private static readonly TimeSpan HourlyInterval = TimeSpan.FromHours(1);

        public AuditScheduleOverdueService(
            IServiceProvider provider,
            ILogger<AuditScheduleOverdueService> logger,
            NotificationHelper notificationHelper,
            INotificationService notificationService)
        {
            _provider = provider;
            _logger = logger;
            _notificationHelper = notificationHelper;
            _notificationService = notificationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AuditScheduleOverdueService started");

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilNextRun(DateTime.UtcNow);
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

                    _logger.LogInformation(
                        "Overdue update completed. Evidence updated: {evidenceCount}, CAPA updated: {capaCount}, Draft updated: {draftCount}, Draft due tomorrow notifications: {draftDueTomorrowCount}",
                        evidenceUpdated,
                        capaUpdated,
                        draftUpdated,
                        draftDueTomorrow.Count);

                    foreach (var (auditId, auditorId, dueDate) in draftDueTomorrow)
                    {
                        var notif = await _notificationService.CreateNotificationAsync(new Notification
                        {
                            UserId = auditorId,
                            Title = "Draft report due tomorrow",
                            Message = $"Audit draft report is due on {dueDate:yyyy-MM-dd HH:mm} UTC.",
                            EntityType = "Audit",
                            EntityId = auditId,
                            IsRead = false,
                            Status = "Active",
                        });

                        await _notificationHelper.SendToUserAsync(
                            auditorId.ToString(),
                            new
                            {
                                Type = "DraftReportDueTomorrow",
                                AuditId = auditId,
                                DueDate = dueDate,
                                NotificationId = notif.NotificationId,
                                Message = "Draft report due tomorrow."
                            });
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

        private static TimeSpan GetDelayUntilNextRun(DateTime nowUtc)
        {
            var nextHourly = nowUtc.Add(HourlyInterval);

            var todayTarget = new DateTime(nowUtc.Year, nowUtc.Month, nowUtc.Day, 0, 1, 0, DateTimeKind.Utc);
            var nextDaily = nowUtc < todayTarget ? todayTarget : todayTarget.AddDays(1);

            var next = nextHourly < nextDaily ? nextHourly : nextDaily;
            return next - nowUtc;
        }

    }
}

