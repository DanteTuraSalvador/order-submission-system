using OrderSubmissionSystem.Infrastructure.Configuration;

namespace OrderSubmissionSystem.Infrastructure.Uploaders
{
    public class LocalFtpUploader : FtpUploaderBase
    {
        public LocalFtpUploader(FtpUploaderSettings settings)
            : base(settings)
        {
        }
    }
}
