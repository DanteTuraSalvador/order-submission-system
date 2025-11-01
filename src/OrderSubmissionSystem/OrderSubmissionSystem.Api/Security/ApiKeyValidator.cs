using System;
using System.Threading;

namespace OrderSubmissionSystem.Api.Security
{
    public static class ApiKeyValidator
    {
        private static IApiKeyStore _store;

        public static void Initialize(IApiKeyStore store)
        {
            Volatile.Write(ref _store, store);
        }

        public static bool TryValidate(string apiKey)
        {
            var store = Volatile.Read(ref _store);
            if (store == null)
            {
                throw new InvalidOperationException("API key store has not been initialised.");
            }

            return store.IsValid(apiKey);
        }
    }
}
