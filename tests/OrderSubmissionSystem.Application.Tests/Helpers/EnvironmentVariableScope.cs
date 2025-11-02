using System;
using System.Collections.Generic;

namespace OrderSubmissionSystem.Application.Tests.Helpers
{
    internal sealed class EnvironmentVariableScope : IDisposable
    {
        private readonly IDictionary<string, string> _originalValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public EnvironmentVariableScope(IDictionary<string, string> variables)
        {
            if (variables == null)
            {
                throw new ArgumentNullException(nameof(variables));
            }

            foreach (var kvp in variables)
            {
                _originalValues[kvp.Key] = Environment.GetEnvironmentVariable(kvp.Key);
                Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
            }
        }

        public void Dispose()
        {
            foreach (var kvp in _originalValues)
            {
                Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
            }
        }
    }
}
