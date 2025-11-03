using System.Reflection;
using OrderSubmissionSystem.Infrastructure.Uploaders;
using Xunit;

namespace OrderSubmissionSystem.UnitTests.Uploaders
{
    public class FtpUploaderBaseTests
    {
        [Theory]
        [InlineData("Order:123?.csv", "Order_123_.csv")]
        [InlineData("", null)]
        [InlineData(null, null)]
        public void SanitizeFileName_RewritesUnsafeNames(string input, string expected)
        {
            var method = typeof(FtpUploaderBase)
                .GetMethod("SanitizeFileName", BindingFlags.NonPublic | BindingFlags.Static);

            Assert.NotNull(method);
            var sanitized = (string)method.Invoke(null, new object[] { input });

            if (expected == null)
            {
                Assert.StartsWith("order_", sanitized);
            }
            else
            {
                Assert.Equal(expected, sanitized);
            }
        }
    }
}
