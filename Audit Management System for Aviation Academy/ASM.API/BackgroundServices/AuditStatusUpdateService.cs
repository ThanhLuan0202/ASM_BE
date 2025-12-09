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
    public class AuditStatusUpdateService : BackgroundService
    {
        private readonly ILogger<AuditStatusUpdateService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private static readonly TimeSpan DailyTargetUtc = new TimeSpan(0, 1, 0); 
        private static readonly TimeSpan HourlyInterval = TimeSpan.FromHours(1);

        public AuditStatusUpdateService(
            ILogger<AuditStatusUpdateService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilNextRun(DateTime.UtcNow);
                var nextRun = DateTime.UtcNow.Add(delay);

                _logger.LogInformation("Next audit status update scheduled at {nextRun} UTC (in {delay})", nextRun, delay);

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
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var auditRepository = scope.ServiceProvider.GetRequiredService<IAuditRepository>();
                        
                        var updatedCount = await auditRepository.UpdateAuditsToInProgressByStartDateAsync();
                        
                        if (updatedCount > 0)
                        {
                            _logger.LogInformation("Updated {count} audit(s) to InProgress status.", updatedCount);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating audit status to InProgress.");
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

