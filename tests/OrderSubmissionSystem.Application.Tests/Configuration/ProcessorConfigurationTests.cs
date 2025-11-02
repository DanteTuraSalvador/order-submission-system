using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderSubmissionSystem.Application.Tests.Helpers;
using OrderSubmissionSystem.Domain.Enums;
using OrderSubmissionSystem.Infrastructure.Configuration;

namespace OrderSubmissionSystem.Application.Tests.Configuration
{
    [TestClass]
    public class ProcessorConfigurationTests
    {
        [TestMethod]
        public void GetProcessorType_ReturnsConfiguredValue()
        {
            using (new EnvironmentVariableScope(new Dictionary<string, string>
            {
                ["ProcessorType"] = nameof(ProcessorType.Ftp)
            }))
            {
                var result = ProcessorConfiguration.GetProcessorType();
                Assert.AreEqual(ProcessorType.Ftp, result);
            }
        }

        [TestMethod]
        public void GetProcessorType_DefaultsToSqlWhenMissing()
        {
            using (new EnvironmentVariableScope(new Dictionary<string, string>
            {
                ["ProcessorType"] = null
            }))
            {
                var result = ProcessorConfiguration.GetProcessorType();
                Assert.AreEqual(ProcessorType.Sql, result);
            }
        }

        [TestMethod]
        public void GetFileFormat_ReturnsConfiguredValue()
        {
            using (new EnvironmentVariableScope(new Dictionary<string, string>
            {
                ["OrderFileFormat"] = nameof(OrderFileFormat.Json)
            }))
            {
                var result = ProcessorConfiguration.GetFileFormat();
                Assert.AreEqual(OrderFileFormat.Json, result);
            }
        }

        [TestMethod]
        public void GetFileFormat_DefaultsToXmlWhenMissing()
        {
            using (new EnvironmentVariableScope(new Dictionary<string, string>
            {
                ["OrderFileFormat"] = null
            }))
            {
                var result = ProcessorConfiguration.GetFileFormat();
                Assert.AreEqual(OrderFileFormat.Xml, result);
            }
        }

        [TestMethod]
        public void GetFtpUploaderSettings_UsesSpecificCredentialsWhenAvailable()
        {
            using (new EnvironmentVariableScope(new Dictionary<string, string>
            {
                ["AzureFtpUrl"] = "ftp://azure.example.com/orders",
                ["AzureFtpUsername"] = "azure-user",
                ["AzureFtpPassword"] = "azure-pass",
                ["FtpUrl"] = "ftp://fallback.example.com/orders",
                ["FtpUsername"] = "fallback-user",
                ["FtpPassword"] = "fallback-pass"
            }))
            {
                var settings = ProcessorConfiguration.GetFtpUploaderSettings(FtpUploaderType.Azure);
                Assert.AreEqual("ftp://azure.example.com/orders", settings.Url);
                Assert.AreEqual("azure-user", settings.Username);
                Assert.AreEqual("azure-pass", settings.Password);
            }
        }

        [TestMethod]
        public void GetFtpUploaderSettings_FallsBackToGenericWhenSpecificMissing()
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
                Assert.AreEqual("ftp://fallback.example.com/orders", settings.Url);
                Assert.AreEqual("fallback-user", settings.Username);
                Assert.AreEqual("fallback-pass", settings.Password);
            }
        }
    }
}
