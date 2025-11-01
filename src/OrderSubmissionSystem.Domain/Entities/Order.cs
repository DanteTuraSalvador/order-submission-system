using System;
using System.Collections.Generic;

namespace OrderSubmissionSystem.Domain.Entities
{
    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
            OrderDate = DateTime.UtcNow;
        }
    }
}