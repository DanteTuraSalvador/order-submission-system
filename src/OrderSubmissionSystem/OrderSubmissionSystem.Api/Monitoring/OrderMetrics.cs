using Prometheus;

namespace OrderSubmissionSystem.Api.Monitoring
{
    public static class OrderMetrics
    {
        public static readonly Counter OrderRequests = Metrics.CreateCounter(
            "order_submission_requests_total",
            "Total number of order submission requests received");

        public static readonly Counter OrderResults = Metrics.CreateCounter(
            "order_submission_results_total",
            "Outcome of order submissions",
            new CounterConfiguration { LabelNames = new[] { "result" } });

        public static readonly Histogram OrderProcessingDuration = Metrics.CreateHistogram(
            "order_submission_duration_seconds",
            "Order submission processing duration in seconds");
    }
}
