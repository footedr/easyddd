using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EasyDdd.Kernel.Kafka
{
    public class KafkaConsumerHealthCheck : IHealthCheck
    {
        public HealthCheckResult Result { get; set; }
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Result);
        }
    }
}
