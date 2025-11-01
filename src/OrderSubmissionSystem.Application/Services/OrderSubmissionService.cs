using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OrderSubmissionSystem.Application.Services
{
    public class OrderSubmissionService : IOrderSubmissionService
    {
        private readonly IOrderProcessor _orderProcessor;

        public OrderSubmissionService(IOrderProcessor orderProcessor)
        {
            _orderProcessor = orderProcessor ?? throw new ArgumentNullException(nameof(orderProcessor));
        }

        public async Task<OrderSubmissionResult> SubmitOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (!ValidateOrder(order))
                return OrderSubmissionResult.ValidationFailure("Order validation failed");

            var processed = await _orderProcessor.ProcessOrderAsync(order);

            if (!processed)
                return OrderSubmissionResult.ProcessingFailure("Order processing failed", order.OrderId);

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
