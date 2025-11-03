using System.Collections.Generic;
using OrderSubmissionSystem.Domain.Enums;
using OrderSubmissionSystem.Infrastructure.Configuration;
using OrderSubmissionSystem.UnitTests.Helpers;
using Xunit;

namespace OrderSubmissionSystem.UnitTests.Configuration
{
    public class ProcessorConfigurationTests
    {
        [Fact]
        public void GetProcessorType_ReturnsConfiguredValue()
        {
            using (new EnvironmentVariableScope(new Dictionary<string, string>
            {
                ["ProcessorType"] = nameof(ProcessorType.Ftp)
            }))
            {
                Assert.Equal(ProcessorType.Ftp, ProcessorConfiguration.GetProcessorType());
            }
        }

        [Fact]
        public void GetProcessorType_DefaultsToSqlWhenMissing()
        {
            using (new EnvironmentVariableScope(new Dictionary<string, string>
            {
                ["ProcessorType"] = null
            }))
            {
                Assert.Equal(ProcessorType.Sql, ProcessorConfiguration.GetProcessorType());
            }
        }

        [Fact]
        public void GetFileFormat_ReturnsConfiguredValue()
        {
            using (new EnvironmentVariableScope(new Dictionary<string, string>
            {
                ["OrderFileFormat"] = nameof(OrderFileFormat.Json)
            }))
            {
                Assert.Equal(OrderFileFormat.Json, ProcessorConfiguration.GetFileFormat());
            }
        }

        [Fact]
        public void GetFtpUploaderSettings_UsesFallbackWhenSpecificMissing()
        {
            using (new EnvironmentVariableScope(new Dictionary<string, string>
            {
                ["AzureFtpUrl"] = null,
                ["AzureFtpUsername"] = null,
                ["AzureFtpPassword"] = null,
                ["FtpUrl"] = "ftp://fallback.example.com/orders",
                ["FtpUsername"] = "fallback-user",
                ["FtpPassword"] = "fallback-pass"
            }))
            {
                var settings = ProcessorConfiguration.GetFtpUploaderSettings(FtpUploaderType.Azure);
                Assert.Equal("ftp://fallback.example.com/orders", settings.Url);
                Assert.Equal("fallback-user", settings.Username);
                Assert.Equal("fallback-pass", settings.Password);
            }
        }
    }
}
