using OrderSubmissionSystem.Api.Monitoring;
using OrderSubmissionSystem.Api.Security;
using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Domain.Entities;
using Serilog;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace OrderSubmissionSystem.Api.Controllers
{
    [ApiKeyAuthorize]
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private readonly IOrderSubmissionService _orderSubmissionService;
        private readonly ILogger _logger;

        public OrdersController(IOrderSubmissionService orderSubmissionService)
        {
            _orderSubmissionService = orderSubmissionService;
            _logger = Log.ForContext<OrdersController>();
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> SubmitOrder(Order order)
        {
            OrderMetrics.OrderRequests.Inc();
            var stopwatch = Stopwatch.StartNew();

            if (order == null)
            {
                stopwatch.Stop();
                OrderMetrics.OrderResults.WithLabels("invalid_payload").Inc();
                _logger.Warning("Order submission rejected because request payload was null");
                return BadRequest("Order cannot be null");
            }

            _logger.Information("Submitting order {OrderId} for customer {CustomerId}", order.OrderId, order.CustomerId);

            var submissionResult = await _orderSubmissionService.SubmitOrderAsync(order);

            stopwatch.Stop();
            OrderMetrics.OrderProcessingDuration.Observe(stopwatch.Elapsed.TotalSeconds);

            if (submissionResult.Success)
            {
                OrderMetrics.OrderResults.WithLabels("success").Inc();
                _logger.Information("Order {OrderId} submitted successfully", order.OrderId);
                return Ok(submissionResult);
            }

            if (submissionResult.Status == OrderSubmissionStatus.ValidationFailed)
            {
                OrderMetrics.OrderResults.WithLabels("validation_failed").Inc();
                _logger.Warning("Order {OrderId} failed validation: {Message}", order.OrderId, submissionResult.Message);
                return Content(HttpStatusCode.BadRequest, submissionResult);
            }

            OrderMetrics.OrderResults.WithLabels("processing_failed").Inc();
            _logger.Error("Order {OrderId} failed during processing: {Message}", order.OrderId, submissionResult.Message);
            return Content(HttpStatusCode.InternalServerError, submissionResult);
        }

        [HttpGet]
        [Route("health")]
        public IHttpActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}
