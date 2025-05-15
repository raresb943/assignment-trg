using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Node.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Node Service worker started at: {time}", DateTimeOffset.Now);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    // Log heartbeat
                    _logger.LogDebug("Node Service worker heartbeat at: {time}", DateTimeOffset.Now);
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // When stoppingToken is triggered, no need to handle
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Node Service worker");
            }
            finally
            {
                _logger.LogInformation("Node Service worker stopped at: {time}", DateTimeOffset.Now);
            }
        }
    }
}
