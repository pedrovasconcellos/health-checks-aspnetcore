using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Vasconcellos.HealthChecks.PostgreSQL
{
    /// <summary>
    /// Notes: use nuget [Npgsql] in the same version within the project that calls this feature.
    /// Otherwise there will be a runtime error when calling this method [CheckHealthAsync] through the method [AddHealthChecksPostgreSQL]
    /// </summary>
    public class PostgreSQLServiceHealthCheck : IHealthCheck
    {
        public static readonly string Name = "postgresql_dependency_check";
        private static string _connectionString;

        public PostgreSQLServiceHealthCheck(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_connectionString))
                return Task.FromResult(HealthCheckResult.Unhealthy($"{nameof(_connectionString)}={_connectionString}."));

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        var sql = "SELECT version()";
                        using (var cmd = new NpgsqlCommand(sql, connection))
                        {
                            var version = cmd.ExecuteScalar()?.ToString();
                            return Task.FromResult(HealthCheckResult.Healthy($"PostgreSQL is working."));
                        }
                    }
                }
                catch (Exception exception)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy($"PostgreSQL error: {exception.Message}."));
                }
                finally
                {
                    connection.Close();
                }
            }
            return Task.FromResult(HealthCheckResult.Unhealthy($"PostgreSQL not working."));
        }
    }

    public static class PostgreSQLServiceHealthCheckExtension
    {
        public static IServiceCollection AddHealthChecksPostgreSQL(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(s => new PostgreSQLServiceHealthCheck(connectionString));

            services.AddHealthChecks()
                .AddCheck<PostgreSQLServiceHealthCheck>(
                    PostgreSQLServiceHealthCheck.Name,
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "readiness" }
                );

            return services;
        }
    }
}
