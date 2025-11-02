using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderSubmissionSystem.Application.Interfaces;
using OrderSubmissionSystem.Application.Tests.Helpers;
using OrderSubmissionSystem.Application.Services;
using OrderSubmissionSystem.Domain.Enums;
using OrderSubmissionSystem.Infrastructure.Configuration;
using OrderSubmissionSystem.Infrastructure.Formatters;
using OrderSubmissionSystem.Infrastructure.Processors;
using OrderSubmissionSystem.Infrastructure.Uploaders;
using OrderSubmissionSystem.Api;
using OrderSubmissionSystem.Api.Security;
using Unity;

namespace OrderSubmissionSystem.Application.Tests.Security
{
    [TestClass]
    public class UnityConfigTests
    {
        [TestMethod]
        public void BuildContainer_ResolvesFtpProcessorBasedOnConfiguration()
        {
            var tempFile = CreateApiKeysFile("local-test");

            try
            {
                using (new EnvironmentVariableScope(new Dictionary<string, string>
                {
                    ["ProcessorType"] = nameof(ProcessorType.Ftp),
                    ["OrderFileFormat"] = nameof(OrderFileFormat.Json),
                    ["FtpUploaderType"] = nameof(FtpUploaderType.Azure),
                    ["AzureFtpUrl"] = "ftp://azure.example.com/orders",
                    ["AzureFtpUsername"] = "azure-user",
                    ["AzureFtpPassword"] = "azure-pass",
                    ["FtpUrl"] = "ftp://fallback.example.com/orders",
                    ["FtpUsername"] = "fallback-user",
                    ["FtpPassword"] = "fallback-pass",
                    ["ApiKeysFile"] = tempFile
                }))
                {
                    using (var container = UnityConfig.BuildContainer())
                    {
                        var service = container.Resolve<IOrderSubmissionService>();
                        var processor = GetPrivateFieldValue<IOrderProcessor>(service, "_orderProcessor");

                        Assert.IsInstanceOfType(processor, typeof(FtpOrderProcessor));
                        var formatter = GetPrivateFieldValue<object>(processor, "_formatter");
                        Assert.IsInstanceOfType(formatter, typeof(JsonOrderFormatter));
                        var uploader = GetPrivateFieldValue<object>(processor, "_uploader");
                        Assert.IsInstanceOfType(uploader, typeof(AzureFtpUploader));
                    }
                }
            }
            finally
            {
                DeleteFileIfExists(tempFile);
            }
        }

        [TestMethod]
        public void BuildContainer_ResolvesSqlProcessorWhenConfigured()
        {
            var tempFile = CreateApiKeysFile("local-test");

            try
            {
                using (new EnvironmentVariableScope(new Dictionary<string, string>
                {
                    ["ProcessorType"] = nameof(ProcessorType.Sql),
                    ["ApiKeysFile"] = tempFile,
                    ["FtpUrl"] = "ftp://fallback.example.com/orders",
                    ["FtpUsername"] = "fallback-user",
                    ["FtpPassword"] = "fallback-pass"
                }))
                {
                    using (var container = UnityConfig.BuildContainer())
                    {
                        var service = container.Resolve<IOrderSubmissionService>();
                        var processor = GetPrivateFieldValue<IOrderProcessor>(service, "_orderProcessor");
                        Assert.IsInstanceOfType(processor, typeof(SqlOrderProcessor));
                    }
                }
            }
            finally
            {
                DeleteFileIfExists(tempFile);
            }
        }

        [TestMethod]
        public void FileApiKeyStore_ReloadsKeysWhenFileChanges()
        {
            var tempFile = CreateApiKeysFile("initial-key");

            using (var store = new FileApiKeyStore(tempFile))
            {
                Assert.IsTrue(store.IsValid("initial-key"));
                Assert.IsFalse(store.IsValid("updated-key"));

                File.WriteAllText(tempFile, "{\n  \"keys\": [\n    { \"value\": \"updated-key\", \"enabled\": true }\n  ]\n}");

                var updated = SpinWait.SpinUntil(() => store.IsValid("updated-key"), TimeSpan.FromSeconds(2));
                Assert.IsTrue(updated, "API key store did not observe file change in time.");
                Assert.IsFalse(store.IsValid("initial-key"));
            }

            DeleteFileIfExists(tempFile);
        }

        private static T GetPrivateFieldValue<T>(object instance, string fieldName)
        {
            var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field, $"Expected field '{fieldName}' to exist on type '{instance.GetType()}'.");
            return (T)field.GetValue(instance);
        }

        private static string CreateApiKeysFile(string keyValue)
        {
            var path = Path.Combine(Path.GetTempPath(), $"api-keys-{Guid.NewGuid():N}.json");
            File.WriteAllText(path, $"{{\n  \"keys\": [\n    {{ \"value\": \"{keyValue}\", \"enabled\": true }}\n  ]\n}}\n");
            return path;
        }

        private static void DeleteFileIfExists(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    // Ignore cleanup failures in tests
                }
            }
        }
    }
}

