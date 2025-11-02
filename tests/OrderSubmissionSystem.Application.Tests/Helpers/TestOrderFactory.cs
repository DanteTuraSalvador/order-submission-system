using System;
using System.Collections.Generic;
using OrderSubmissionSystem.Domain.Entities;

namespace OrderSubmissionSystem.Application.Tests.Helpers
{
    internal static class TestOrderFactory
    {
        public static Order CreateSampleOrder()
        {
            return new Order
            {
                OrderId = "ORDER-123",
                CustomerId = "CUSTOMER-456",
                TotalAmount = 150.75m,
                OrderDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        ProductId = "SKU-1",
                        Quantity = 2,
                        UnitPrice = 50.25m
                    },
                    new OrderItem
                    {
                        ProductId = "SKU-2",
                        Quantity = 1,
                        UnitPrice = 50.25m
                    }
                }
            };
        }
    }
}
