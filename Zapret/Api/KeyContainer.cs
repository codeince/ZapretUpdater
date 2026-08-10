using System.Collections.Concurrent;

namespace ZapretUpdater.Zapret.Api
{
    internal static class KeyContainer
    {
        private static readonly List<string> _apiTypes = ["github"];
        public static ConcurrentDictionary<string, string> ApiKeys = [];

        private static void GetApiKey(string apiType)
        {
            var apiKey = Environment.GetEnvironmentVariable($"{apiType}_API_KEY".ToUpper());

            if (!string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine($"Loaded API key for {apiType}");
                ApiKeys.TryAdd(apiType, apiKey);
            }
        }

        public static void GetApiKeys()
        {
            _apiTypes.AsParallel().ForAll(GetApiKey);
        }
    }
}
