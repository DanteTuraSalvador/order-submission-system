using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Domain.Entities;

namespace OrderSubmissionSystem.Infrastructure.Formatters
{
    public class XmlOrderFormatter : IOrderFileFormatter
    {
        public OrderFilePayload Format(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var serializer = new XmlSerializer(typeof(Order));
            using (var stringWriter = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
            {
                serializer.Serialize(xmlWriter, order);
                var xmlContent = stringWriter.ToString();
                return new OrderFilePayload
                {
                    FileName = $"Order_{order.OrderId}.xml",
                    ContentType = "application/xml",
                    Content = Encoding.UTF8.GetBytes(xmlContent)
                };
            }
        }
    }
}
