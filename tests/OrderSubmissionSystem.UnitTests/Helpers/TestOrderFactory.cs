using System;
using System.Collections.Generic;
using OrderSubmissionSystem.Domain.Entities;

namespace OrderSubmissionSystem.UnitTests.Helpers
{
    internal static class TestOrderFactory
    {
        public static Order CreateOrder(Action<Order> configure = null)
        {
            var order = new Order
            {
                OrderId = "ORDER-123",
                CustomerId = "CUSTOMER-456",
                TotalAmount = 150m,
                OrderDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = "SKU-1", Quantity = 1, UnitPrice = 50m },
                    new OrderItem { ProductId = "SKU-2", Quantity = 2, UnitPrice = 50m }
                }
            };

            configure?.Invoke(order);
            return order;
        }
    }
}
