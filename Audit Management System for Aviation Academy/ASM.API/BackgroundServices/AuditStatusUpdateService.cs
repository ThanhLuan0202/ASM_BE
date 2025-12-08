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
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1); 

        public AuditStatusUpdateService(
            ILogger<AuditStatusUpdateService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AuditStatusUpdateService BackgroundService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var auditRepository = scope.ServiceProvider.GetRequiredService<IAuditRepository>();
                        
                        var updatedAuditIds = await auditRepository.UpdateAuditsToInProgressByStartDateAsync();
                        
                        if (updatedAuditIds.Any())
                        {
                            var auditIdsString = string.Join(", ", updatedAuditIds);
                            _logger.LogInformation($"Updated {updatedAuditIds.Count} audit(s) to InProgress status. Audit IDs: {auditIdsString}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating audit status to InProgress.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("AuditStatusUpdateService is stopping.");
        }
    }
}

