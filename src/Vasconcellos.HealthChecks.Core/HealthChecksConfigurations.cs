using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Vasconcellos.HealthChecks.Core.CoreResources;

namespace Vasconcellos.HealthChecks.Core
{
    public static class HealthChecksConfigurations
    {
        private static readonly string _readiness = "readiness";
        private static readonly string _liveness = "liveness";
        private static Dictionary<HealthStatus, int> _resultStatusCodes = new Dictionary<HealthStatus, int>
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK,
            [HealthStatus.Degraded] = StatusCodes.Status200OK,
            [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
        };
        private static readonly IList<string> _addsHealthChecks = new List<string> { };

        public static IServiceCollection AddHealthChecksStartupHosted(this IServiceCollection services, int initialDelaySeconds = 5, bool scheduledLogTask = false)
        {
            StartupHostedService.DelaySeconds = initialDelaySeconds;

            services.AddHostedService<StartupHostedService>();
            services.AddSingleton<StartupHostedServiceHealthCheck>();

            services.AddHealthChecks()
                .AddCheck<StartupHostedServiceHealthCheck>(
                    StartupHostedServiceHealthCheck.Name,
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { _readiness }
                );

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(StartupHostedService.DelaySeconds + 0.2);
                options.Predicate = (check) => check.Tags.Contains(_readiness);
            });

            if (scheduledLogTask)
                services.AddSingleton<IHealthCheckPublisher, ReadinessPublisher>();

            return services;
        }

        public static IServiceCollection AddHealthChecksReadiness(this IServiceCollection services)
        {
            _addsHealthChecks.Add(_readiness);
            return services;
        }

        public static IServiceCollection AddHealthChecksLiveness(this IServiceCollection services)
        {
            _addsHealthChecks.Add(_liveness);
            return services;
        }

        public static void MapHealthChecksEndPoints(this IEndpointRouteBuilder endpointRouteBuilder, Func<HttpContext, HealthReport, Task> responseWriter = default)
        {
            if (responseWriter == default)
                responseWriter = HealthChecksResponseWriter.WriteResponse;

            endpointRouteBuilder.MapHealthChecks("/health", new HealthCheckOptions()
            {
                AllowCachingResponses = false,
                ResultStatusCodes = _resultStatusCodes,
                ResponseWriter = responseWriter
            });

            if (_addsHealthChecks.Contains(_readiness))
            {
                endpointRouteBuilder.MapHealthChecks($"/health/{_readiness}", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains(_readiness),
                    AllowCachingResponses = false,
                    ResultStatusCodes = _resultStatusCodes,
                    ResponseWriter = responseWriter
                });
            }

            if (_addsHealthChecks.Contains(_liveness))
            {
                endpointRouteBuilder.MapHealthChecks($"/health/{_liveness}", new HealthCheckOptions()
                {
                    Predicate = (_) => false,
                    AllowCachingResponses = false,
                    ResultStatusCodes = _resultStatusCodes,
                    ResponseWriter = responseWriter
                });
            }
        }
    }
}