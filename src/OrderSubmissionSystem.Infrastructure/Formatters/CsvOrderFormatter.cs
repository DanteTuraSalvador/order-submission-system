using System;
using System.Linq;
using System.Text;
using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Domain.Entities;

namespace OrderSubmissionSystem.Infrastructure.Formatters
{
    public class CsvOrderFormatter : IOrderFileFormatter
    {
        public OrderFilePayload Format(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var builder = new StringBuilder();
            builder.AppendLine("OrderId,CustomerId,TotalAmount,OrderDate");
            builder.AppendLine($"{Escape(order.OrderId)},{Escape(order.CustomerId)},{order.TotalAmount},{order.OrderDate:O}");
            builder.AppendLine();
            builder.AppendLine("ProductId,Quantity,UnitPrice,LineTotal");

            foreach (var item in order.Items ?? Enumerable.Empty<OrderItem>())
            {
                builder.AppendLine($"{Escape(item.ProductId)},{item.Quantity},{item.UnitPrice},{item.GetLineTotal()}");
            }

            return new OrderFilePayload
            {
                FileName = $"Order_{order.OrderId}.csv",
                ContentType = "text/csv",
                Content = Encoding.UTF8.GetBytes(builder.ToString())
            };
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}
