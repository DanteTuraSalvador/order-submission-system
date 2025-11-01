using System.Net;
using OrderSubmissionSystem.Infrastructure.Configuration;

namespace OrderSubmissionSystem.Infrastructure.Uploaders
{
    public class AzureFtpUploader : FtpUploaderBase
    {
        public AzureFtpUploader(FtpUploaderSettings settings)
            : base(settings)
        {
        }

        protected override FtpWebRequest CreateRequest(string ftpUrl)
        {
            var request = base.CreateRequest(ftpUrl);
            request.EnableSsl = true;
            request.UsePassive = true;
            return request;
        }
    }
}
