using Prometheus;

namespace OrderSubmissionSystem.Api.Monitoring
{
    public static class AuthMetrics
    {
        public static readonly Counter ApiKeyValidation = Metrics.CreateCounter(
            "api_key_validation_total",
            "API key validation attempts",
            new CounterConfiguration { LabelNames = new[] { "result" } });
    }
}
