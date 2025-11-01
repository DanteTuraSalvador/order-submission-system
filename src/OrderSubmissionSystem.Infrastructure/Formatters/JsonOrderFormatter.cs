using System;
using System.Text;
using Newtonsoft.Json;
using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Domain.Entities;

namespace OrderSubmissionSystem.Infrastructure.Formatters
{
    public class JsonOrderFormatter : IOrderFileFormatter
    {
        public OrderFilePayload Format(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var json = JsonConvert.SerializeObject(order, Formatting.Indented);

            return new OrderFilePayload
            {
                FileName = $"Order_{order.OrderId}.json",
                ContentType = "application/json",
                Content = Encoding.UTF8.GetBytes(json)
            };
        }
    }
}
