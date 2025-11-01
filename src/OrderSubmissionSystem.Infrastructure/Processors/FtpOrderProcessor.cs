using System;
using System.Threading.Tasks;
using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Domain.Entities;
using Serilog;

namespace OrderSubmissionSystem.Infrastructure.Processors
{
    public class FtpOrderProcessor : IOrderProcessor
    {
        private readonly IOrderFileFormatter _formatter;
        private readonly IFtpUploader _uploader;
        private readonly ILogger _logger;

        public FtpOrderProcessor(IOrderFileFormatter formatter, IFtpUploader uploader)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _uploader = uploader ?? throw new ArgumentNullException(nameof(uploader));
            _logger = Log.ForContext<FtpOrderProcessor>();
        }

        public async Task<bool> ProcessOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            try
            {
                _logger.Information("Preparing order {OrderId} for FTP upload using {Formatter} and {Uploader}",
                    order.OrderId,
                    _formatter.GetType().Name,
                    _uploader.GetType().Name);

                var payload = _formatter.Format(order);
                var uploadResult = await _uploader.UploadAsync(payload).ConfigureAwait(false);

                if (!uploadResult)
                {
                    _logger.Error("FTP upload reported failure for order {OrderId}", order.OrderId);
                }
                else
                {
                    _logger.Information("Order {OrderId} uploaded to FTP successfully", order.OrderId);
                }

                return uploadResult;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing order {OrderId} via FTP", order.OrderId);
                return false;
            }
        }
    }
}
