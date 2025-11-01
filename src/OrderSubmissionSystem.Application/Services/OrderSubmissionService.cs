using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Domain.Entities;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OrderSubmissionSystem.Application.Services
{
    public class OrderSubmissionService : IOrderSubmissionService
    {
        private readonly IOrderProcessor _orderProcessor;
        private readonly ILogger _logger;

        public OrderSubmissionService(IOrderProcessor orderProcessor)
        {
            _orderProcessor = orderProcessor ?? throw new ArgumentNullException(nameof(orderProcessor));
            _logger = Log.ForContext<OrderSubmissionService>();
        }

        public async Task<OrderSubmissionResult> SubmitOrderAsync(Order order)
        {
            if (order == null)
            {
                _logger.Warning("Attempted to submit a null order payload");
                throw new ArgumentNullException(nameof(order));
            }

            if (!ValidateOrder(order))
            {
                _logger.Warning("Order {OrderId} failed validation", order.OrderId);
                return OrderSubmissionResult.ValidationFailure("Order validation failed");
            }

            var processed = await _orderProcessor.ProcessOrderAsync(order).ConfigureAwait(false);

            if (!processed)
            {
                _logger.Error("Order {OrderId} failed during downstream processing", order.OrderId);
                return OrderSubmissionResult.ProcessingFailure("Order processing failed", order.OrderId);
            }

            _logger.Information("Order {OrderId} processed successfully", order.OrderId);
            return OrderSubmissionResult.SuccessResult(order.OrderId);
        }

        public bool ValidateOrder(Order order)
        {
            if (order == null)
                return false;

            if (string.IsNullOrWhiteSpace(order.OrderId))
                return false;

            if (string.IsNullOrWhiteSpace(order.CustomerId))
                return false;

            if (order.Items == null || !order.Items.Any())
                return false;

            if (order.TotalAmount <= 0)
                return false;

            foreach (var item in order.Items)
            {
                if (string.IsNullOrWhiteSpace(item.ProductId))
                    return false;

                if (item.Quantity <= 0)
                    return false;

                if (item.UnitPrice <= 0)
                    return false;
            }

            return true;
        }
    }
}
