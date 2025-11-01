using System;
using System.Configuration;
using StackExchange.Redis;

namespace OrderSubmissionSystem.Api.Security
{
    public static class RedisConnectionFactory
    {
        private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new Lazy<ConnectionMultiplexer>(CreateConnection);

        public static ConnectionMultiplexer Connection => LazyConnection.Value;

        private static ConnectionMultiplexer CreateConnection()
        {
            var connectionString = Environment.GetEnvironmentVariable("RedisConnectionString")
                                   ?? ConfigurationManager.AppSettings["RedisConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("RedisConnectionString appSetting is required for API key validation");
            }

            return ConnectionMultiplexer.Connect(connectionString);
        }
    }
}
