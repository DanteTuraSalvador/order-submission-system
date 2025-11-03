using System;
using System.Collections.Generic;

namespace OrderSubmissionSystem.UnitTests.Helpers
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

            foreach (var entry in variables)
            {
                _originalValues[entry.Key] = Environment.GetEnvironmentVariable(entry.Key);
                Environment.SetEnvironmentVariable(entry.Key, entry.Value);
            }
        }

        public void Dispose()
        {
            foreach (var entry in _originalValues)
            {
                Environment.SetEnvironmentVariable(entry.Key, entry.Value);
            }
        }
    }
}
