using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using OrderSubmissionSystem.Infrastructure.Formatters;
using OrderSubmissionSystem.UnitTests.Helpers;
using Xunit;

namespace OrderSubmissionSystem.UnitTests.Formatters
{
    public class OrderFormatterTests
    {
        [Fact]
        public void CsvFormatter_ProducesExpectedPayload()
        {
            var order = TestOrderFactory.CreateOrder();
            var formatter = new CsvOrderFormatter();

            var payload = formatter.Format(order);
            var csv = Encoding.UTF8.GetString(payload.Content);
            var lines = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Assert.Equal("Order_ORDER-123.csv", payload.FileName);
            Assert.Equal("text/csv", payload.ContentType);
            Assert.Contains(order.OrderId, lines[1]);
            Assert.Contains(order.CustomerId, lines[1]);
        }

        [Fact]
        public void XmlFormatter_ProducesSerializablePayload()
        {
            var order = TestOrderFactory.CreateOrder();
            var formatter = new XmlOrderFormatter();

            var payload = formatter.Format(order);
            var xml = Encoding.UTF8.GetString(payload.Content);
            var document = XDocument.Parse(xml);

            Assert.Equal("Order_ORDER-123.xml", payload.FileName);
            Assert.Equal("application/xml", payload.ContentType);
            Assert.Equal("Order", document.Root?.Name.LocalName);
        }

        [Fact]
        public void JsonFormatter_ProducesExpectedStructure()
        {
            var order = TestOrderFactory.CreateOrder();
            var formatter = new JsonOrderFormatter();

            var payload = formatter.Format(order);
            var json = Encoding.UTF8.GetString(payload.Content);

            Assert.Equal("Order_ORDER-123.json", payload.FileName);
            Assert.Equal("application/json", payload.ContentType);
            Assert.Contains(order.CustomerId, json);
        }

        [Fact]
        public void ExcelFormatter_WrapsOrderInHtmlTable()
        {
            var order = TestOrderFactory.CreateOrder();
            var formatter = new ExcelOrderFormatter();

            var payload = formatter.Format(order);
            var html = Encoding.UTF8.GetString(payload.Content);

            Assert.Equal("Order_ORDER-123.xls", payload.FileName);
            Assert.Equal("application/vnd.ms-excel", payload.ContentType);
            Assert.Contains("<table", html);
            Assert.Contains(order.CustomerId, html);
        }
    }
}
