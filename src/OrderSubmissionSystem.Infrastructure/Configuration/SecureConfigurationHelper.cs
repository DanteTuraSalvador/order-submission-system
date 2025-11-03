using System;
using System.Configuration;

namespace OrderSubmissionSystem.Infrastructure.Configuration
{
    /// <summary>
    /// Helper class for reading configuration with fallback to environment variables
    /// Prioritizes: Environment Variables > secrets.config > Web.config
    /// </summary>
    public static class SecureConfigurationHelper
    {
        /// <summary>
        /// Gets a configuration value with fallback to environment variable
        /// </summary>
        public static string GetSetting(string key, string defaultValue = null)
        {
            // Priority 1: Environment variable (can override Web.config)
            var envValue = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrWhiteSpace(envValue))
            {
                return envValue;
            }

            // Priority 2: Web.config or secrets.config (via file attribute)
            var configValue = ConfigurationManager.AppSettings[key];
            if (!string.IsNullOrWhiteSpace(configValue))
            {
                return configValue;
            }

            // Priority 3: Default value
            return defaultValue;
        }

        /// <summary>
        /// Gets a required configuration value, throws if not found
        /// </summary>
        public static string GetRequiredSetting(string key)
        {
            var value = GetSetting(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ConfigurationErrorsException(
                    $"Required configuration setting '{key}' is missing. " +
                    $"Please add it to Web.config, secrets.config, or as an environment variable.");
            }
            return value;
        }

        /// <summary>
        /// Gets a connection string with fallback to environment variable
        /// </summary>
        public static string GetConnectionString(string name, string defaultValue = null)
        {
            // Priority 1: Environment variable with _CONNECTIONSTRING suffix
            var envKey = $"{name.ToUpperInvariant()}_CONNECTIONSTRING";
            var envValue = Environment.GetEnvironmentVariable(envKey);
            if (!string.IsNullOrWhiteSpace(envValue))
            {
                return envValue;
            }

            // Priority 2: ConnectionStrings section
            var connectionString = ConfigurationManager.ConnectionStrings[name];
            if (connectionString != null && !string.IsNullOrWhiteSpace(connectionString.ConnectionString))
            {
                return connectionString.ConnectionString;
            }

            // Priority 3: Default value
            return defaultValue;
        }

        /// <summary>
        /// Gets a required connection string, throws if not found
        /// </summary>
        public static string GetRequiredConnectionString(string name)
        {
            var value = GetConnectionString(name);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ConfigurationErrorsException(
                    $"Required connection string '{name}' is missing. " +
                    $"Please add it to Web.config, secrets.config, or as an environment variable.");
            }
            return value;
        }

        /// <summary>
        /// Gets a boolean configuration value
        /// </summary>
        public static bool GetBoolSetting(string key, bool defaultValue = false)
        {
            var value = GetSetting(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return bool.TryParse(value, out var result) ? result : defaultValue;
        }

        /// <summary>
        /// Gets an integer configuration value
        /// </summary>
        public static int GetIntSetting(string key, int defaultValue = 0)
        {
            var value = GetSetting(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return int.TryParse(value, out var result) ? result : defaultValue;
        }
    }
}