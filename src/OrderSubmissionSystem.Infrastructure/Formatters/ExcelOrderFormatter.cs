using System;
using System.Linq;
using System.Text;
using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Domain.Entities;

namespace OrderSubmissionSystem.Infrastructure.Formatters
{
    public class ExcelOrderFormatter : IOrderFileFormatter
    {
        public OrderFilePayload Format(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var builder = new StringBuilder();
            builder.AppendLine("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8'/></head><body>");
            builder.AppendLine("<table border='1'>");
            builder.AppendLine("<tr><th colspan='4'>Order Summary</th></tr>");
            builder.AppendLine($"<tr><td>Order Id</td><td>{Escape(order.OrderId)}</td><td>Customer Id</td><td>{Escape(order.CustomerId)}</td></tr>");
            builder.AppendLine($"<tr><td>Total Amount</td><td>{order.TotalAmount}</td><td>Order Date</td><td>{order.OrderDate:O}</td></tr>");
            builder.AppendLine("</table><br/>");

            builder.AppendLine("<table border='1'>");
            builder.AppendLine("<tr><th>Product Id</th><th>Quantity</th><th>Unit Price</th><th>Line Total</th></tr>");
            foreach (var item in order.Items ?? Enumerable.Empty<OrderItem>())
            {
                builder.AppendLine($"<tr><td>{Escape(item.ProductId)}</td><td>{item.Quantity}</td><td>{item.UnitPrice}</td><td>{item.GetLineTotal()}</td></tr>");
            }
            builder.AppendLine("</table>");
            builder.AppendLine("</body></html>");

            return new OrderFilePayload
            {
                FileName = $"Order_{order.OrderId}.xls",
                ContentType = "application/vnd.ms-excel",
                Content = Encoding.UTF8.GetBytes(builder.ToString())
            };
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}
