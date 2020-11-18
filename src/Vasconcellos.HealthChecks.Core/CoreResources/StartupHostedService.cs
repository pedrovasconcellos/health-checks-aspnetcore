using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Vasconcellos.HealthChecks.Core.CoreResources
{
    public class StartupHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly StartupHostedServiceHealthCheck _startupHostedServiceHealthCheck;
        private static int _delaySeconds = 15;
        private static bool _setDelaySecond = true;
        public static int DelaySeconds
        {
            get => _delaySeconds;
            set
            {
                if (_setDelaySecond && _delaySeconds != value && value > 4)
                    _delaySeconds = value;

                if(_setDelaySecond)
                    _setDelaySecond = false;
            }
        }

        public StartupHostedService(ILogger<StartupHostedService> logger,
            StartupHostedServiceHealthCheck startupHostedServiceHealthCheck)
        {
            _logger = logger;
            _startupHostedServiceHealthCheck = startupHostedServiceHealthCheck;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Startup Background Service is starting.");

            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(_delaySeconds));

                _startupHostedServiceHealthCheck.StartupTaskCompleted = true;

                _logger.LogInformation("Startup Background Service has started.");
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Startup Background Service is stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
