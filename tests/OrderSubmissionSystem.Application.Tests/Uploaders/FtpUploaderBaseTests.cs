using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderSubmissionSystem.Infrastructure.Uploaders;

namespace OrderSubmissionSystem.Application.Tests.Uploaders
{
    [TestClass]
    public class FtpUploaderBaseTests
    {
        [TestMethod]
        public void SanitizeFileName_ReplacesInvalidCharacters()
        {
            var sanitized = InvokeSanitizeFileName("Order:123?.csv");
            Assert.AreEqual("Order_123_.csv", sanitized);
        }

        [TestMethod]
        public void SanitizeFileName_GeneratesFallbackForEmpty()
        {
            var sanitized = InvokeSanitizeFileName(string.Empty);
            StringAssert.StartsWith(sanitized, "order_");
        }

        private static string InvokeSanitizeFileName(string input)
        {
            var method = typeof(FtpUploaderBase)
                .GetMethod("SanitizeFileName", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method, "Expected private SanitizeFileName method to exist.");
            return (string)method.Invoke(null, new object[] { input });
        }
    }
}
