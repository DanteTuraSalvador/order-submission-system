using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Domain.Entities;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OrderSubmissionSystem.Infrastructure.Processors
{
    public class FtpOrderProcessor : IOrderProcessor
    {
        private readonly string _ftpUrl;
        private readonly string _ftpUsername;
        private readonly string _ftpPassword;

        public FtpOrderProcessor()
        {
            _ftpUrl = ConfigurationManager.AppSettings["FtpUrl"]
                ?? throw new InvalidOperationException("FtpUrl not configured");
            _ftpUsername = ConfigurationManager.AppSettings["FtpUsername"] ?? "";
            _ftpPassword = ConfigurationManager.AppSettings["FtpPassword"] ?? "";
        }

        public async Task<bool> ProcessOrderAsync(Order order)
        {
            try
            {
                var xmlContent = SerializeOrderToXml(order);
                var fileName = $"Order_{order.OrderId}_{DateTime.UtcNow:yyyyMMddHHmmss}.xml";
                var ftpFullUrl = $"{_ftpUrl.TrimEnd('/')}/{fileName}";

                var request = (FtpWebRequest)WebRequest.Create(ftpFullUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);

                var bytes = Encoding.UTF8.GetBytes(xmlContent);
                request.ContentLength = bytes.Length;

                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    await requestStream.WriteAsync(bytes, 0, bytes.Length);
                }

                using (var response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    return response.StatusCode == FtpStatusCode.ClosingData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing order via FTP: {ex.Message}");
                return false;
            }
        }

        private string SerializeOrderToXml(Order order)
        {
            var serializer = new XmlSerializer(typeof(Order));
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, order);
                return stringWriter.ToString();
            }
        }
    }
}