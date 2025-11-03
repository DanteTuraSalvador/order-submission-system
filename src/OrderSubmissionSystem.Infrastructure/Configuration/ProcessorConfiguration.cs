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

        public static FtpUploaderSettings GetFtpUploaderSettings(FtpUploaderType uploaderType)
        {
            string urlKey;
            string usernameKey;
            string passwordKey;

            switch (uploaderType)
            {
                case FtpUploaderType.Azure:
                    urlKey = "AzureFtpUrl";
                    usernameKey = "AzureFtpUsername";
                    passwordKey = "AzureFtpPassword";
                    break;

                case FtpUploaderType.Aws:
                    urlKey = "AwsFtpUrl";
                    usernameKey = "AwsFtpUsername";
                    passwordKey = "AwsFtpPassword";
                    break;

                case FtpUploaderType.Local:
                default:
                    urlKey = "FtpUrl";
                    usernameKey = "FtpUsername";
                    passwordKey = "FtpPassword";
                    break;
            }

            var url = SecureConfigurationHelper.GetSetting(urlKey);
            if (string.IsNullOrWhiteSpace(url))
            {
                url = SecureConfigurationHelper.GetRequiredSetting("FtpUrl");
            }

            var username = SecureConfigurationHelper.GetSetting(usernameKey);
            if (string.IsNullOrWhiteSpace(username))
            {
                username = SecureConfigurationHelper.GetRequiredSetting("FtpUsername");
            }

            var password = SecureConfigurationHelper.GetSetting(passwordKey);
            if (string.IsNullOrWhiteSpace(password))
            {
                password = SecureConfigurationHelper.GetRequiredSetting("FtpPassword");
            }

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
