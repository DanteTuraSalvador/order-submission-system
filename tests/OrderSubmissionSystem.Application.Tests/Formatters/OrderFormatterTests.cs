using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using OrderSubmissionSystem.Application.Tests.Helpers;
using OrderSubmissionSystem.Infrastructure.Formatters;

namespace OrderSubmissionSystem.Application.Tests.Formatters
{
    [TestClass]
    public class OrderFormatterTests
    {
        [TestMethod]
        public void CsvFormatter_ProducesExpectedPayload()
        {
            var order = TestOrderFactory.CreateSampleOrder();
            var formatter = new CsvOrderFormatter();

            var payload = formatter.Format(order);
            var csv = Encoding.UTF8.GetString(payload.Content);
            var lines = csv.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            Assert.AreEqual("Order_ORDER-123.csv", payload.FileName);
            Assert.AreEqual("text/csv", payload.ContentType);
            Assert.AreEqual("OrderId,CustomerId,TotalAmount,OrderDate", lines[0]);
            Assert.IsTrue(lines[1].Contains(order.OrderId));
            Assert.IsTrue(lines[1].Contains(order.CustomerId));
        }

        [TestMethod]
        public void XmlFormatter_ProducesSerializablePayload()
        {
            var order = TestOrderFactory.CreateSampleOrder();
            var formatter = new XmlOrderFormatter();

            var payload = formatter.Format(order);
            var xml = Encoding.UTF8.GetString(payload.Content);
            var document = XDocument.Parse(xml);

            Assert.AreEqual("Order_ORDER-123.xml", payload.FileName);
            Assert.AreEqual("application/xml", payload.ContentType);
            Assert.AreEqual("Order", document.Root?.Name.LocalName);
        }

        [TestMethod]
        public void JsonFormatter_ProducesExpectedStructure()
        {
            var order = TestOrderFactory.CreateSampleOrder();
            var formatter = new JsonOrderFormatter();

            var payload = formatter.Format(order);
            var json = Encoding.UTF8.GetString(payload.Content);
            var obj = JObject.Parse(json);

            Assert.AreEqual("Order_ORDER-123.json", payload.FileName);
            Assert.AreEqual("application/json", payload.ContentType);
            Assert.AreEqual(order.OrderId, obj["OrderId"]?.Value<string>());
            Assert.AreEqual(order.Items.Count, obj["Items"]?.Count());
        }

        [TestMethod]
        public void ExcelFormatter_WrapsOrderInHtmlTable()
        {
            var order = TestOrderFactory.CreateSampleOrder();
            var formatter = new ExcelOrderFormatter();

            var payload = formatter.Format(order);
            var html = Encoding.UTF8.GetString(payload.Content);

            Assert.AreEqual("Order_ORDER-123.xls", payload.FileName);
            Assert.AreEqual("application/vnd.ms-excel", payload.ContentType);
            StringAssert.Contains(html, "<table");
            StringAssert.Contains(html, order.CustomerId);
        }
    }
}
