using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Models;
using OrderSubmissionSystem.Infrastructure.Configuration;

namespace OrderSubmissionSystem.Infrastructure.Uploaders
{
    public abstract class FtpUploaderBase : IFtpUploader
    {
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        private readonly FtpUploaderSettings _settings;

        protected FtpUploaderBase(FtpUploaderSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task<bool> UploadAsync(OrderFilePayload payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            var safeFileName = SanitizeFileName(payload.FileName);
            if (!string.Equals(payload.FileName, safeFileName, StringComparison.Ordinal))
            {
                payload.FileName = safeFileName;
            }

            var ftpUrl = BuildDestinationUrl(safeFileName);
            var request = CreateRequest(ftpUrl);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = BuildCredentials();
            request.UseBinary = true;
            request.ContentLength = payload.Content.Length;

            using (var requestStream = await request.GetRequestStreamAsync().ConfigureAwait(false))
            {
                await requestStream.WriteAsync(payload.Content, 0, payload.Content.Length).ConfigureAwait(false);
            }

            using (var response = (FtpWebResponse)await request.GetResponseAsync().ConfigureAwait(false))
            {
                return response.StatusCode == FtpStatusCode.ClosingData;
            }
        }

        protected virtual FtpWebRequest CreateRequest(string ftpUrl)
        {
            return (FtpWebRequest)WebRequest.Create(ftpUrl);
        }

        protected virtual ICredentials BuildCredentials()
        {
            return new NetworkCredential(_settings.Username, _settings.Password);
        }

        protected virtual string BuildDestinationUrl(string fileName)
        {
            return $"{_settings.Url.TrimEnd('/')}/{fileName}";
        }

        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return $"order_{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            }

            var builder = new StringBuilder(fileName.Length);
            foreach (var character in fileName)
            {
                if (Array.IndexOf(InvalidFileNameChars, character) >= 0 || character == '/' || character == '\\')
                {
                    builder.Append('_');
                }
                else
                {
                    builder.Append(character);
                }
            }

            var result = builder.ToString().Trim();
            if (string.IsNullOrWhiteSpace(result) || result == "." || result == "..")
            {
                result = $"order_{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            }

            return result;
        }
    }
}
