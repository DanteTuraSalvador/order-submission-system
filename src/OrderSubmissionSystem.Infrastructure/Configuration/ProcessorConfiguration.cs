using OrderSubmissionSystem.Domain.Enums;
using System;
using System.Configuration;

namespace OrderSubmissionSystem.Infrastructure.Configuration
{
    public static class ProcessorConfiguration
    {
        public static ProcessorType GetProcessorType()
        {
            var processorTypeSetting = GetSetting("ProcessorType");

            if (!string.IsNullOrWhiteSpace(processorTypeSetting) &&
                 Enum.TryParse(processorTypeSetting, true, out ProcessorType processorType))
            {
                return processorType;
            }

            return ProcessorType.Sql;
        }

        public static OrderFileFormat GetFileFormat()
        {
            var formatSetting = GetSetting("OrderFileFormat");

            if (!string.IsNullOrWhiteSpace(formatSetting) &&
                Enum.TryParse(formatSetting, true, out OrderFileFormat format))
            {
                return format;
            }

            return OrderFileFormat.Xml;
        }

        public static FtpUploaderType GetFtpUploaderType()
        {
            var uploaderSetting = GetSetting("FtpUploaderType");

            if (!string.IsNullOrWhiteSpace(uploaderSetting) &&
                Enum.TryParse(uploaderSetting, true, out FtpUploaderType uploaderType))
            {
                return uploaderType;
            }

            return FtpUploaderType.Local;
        }

        public static FtpUploaderSettings GetFtpUploaderSettings(FtpUploaderType type)
        {
            string prefix = type.ToString();

            string url = GetSetting($"{prefix}FtpUrl")
                         ?? GetSetting("FtpUrl");

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new InvalidOperationException($"{prefix} FTP URL not configured");
            }

            string username = GetSetting($"{prefix}FtpUsername")
                              ?? GetSetting("FtpUsername")
                              ?? string.Empty;

            string password = GetSetting($"{prefix}FtpPassword")
                              ?? GetSetting("FtpPassword")
                              ?? string.Empty;

            return new FtpUploaderSettings
            {
                Url = url,
                Username = username,
                Password = password
            };
        }

        private static string GetSetting(string key)
        {
            return Environment.GetEnvironmentVariable(key) ?? ConfigurationManager.AppSettings[key];
        }
    }
}
