using ASM_Repositories.Interfaces;
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
        private static readonly TimeSpan DailyTargetUtc = new TimeSpan(0, 1, 0);
        private static readonly TimeSpan HourlyInterval = TimeSpan.FromHours(1);

        public AuditScheduleOverdueService(IServiceProvider provider, ILogger<AuditScheduleOverdueService> logger)
        {
            _provider = provider;
            _logger = logger;
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

                    var updated = await repo.MarkEvidenceDueOverdueAsync(stoppingToken);

                    _logger.LogInformation(
                        "Overdue update completed. Updated rows: {count}",
                        updated);
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

