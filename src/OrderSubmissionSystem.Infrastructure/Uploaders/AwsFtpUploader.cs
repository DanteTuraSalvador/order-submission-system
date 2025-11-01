using System.Net;
using OrderSubmissionSystem.Infrastructure.Configuration;

namespace OrderSubmissionSystem.Infrastructure.Uploaders
{
    public class AwsFtpUploader : FtpUploaderBase
    {
        public AwsFtpUploader(FtpUploaderSettings settings)
            : base(settings)
        {
        }

        protected override FtpWebRequest CreateRequest(string ftpUrl)
        {
            var request = base.CreateRequest(ftpUrl);
            request.UsePassive = false;
            return request;
        }
    }
}
