using Microsoft.Extensions.Caching.Memory;
using NewsAggregator.Models;
using System.Text.Json;

namespace NewsAggregator.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HackerNewsService> _logger;
        private readonly IMemoryCache _memoryCache;

        private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);

        public HackerNewsService(HttpClient httpClient, ILogger<HackerNewsService> logger, IMemoryCache memoryCache) =>
            (_httpClient, _logger, _memoryCache) = (httpClient, logger, memoryCache);

        public async Task<IEnumerable<int>> GetBestStoryIds(int count)
        {
            _logger.LogInformation($"{nameof(GetBestStoryIds)} called with: {count}");
            string endpoint = "beststories.json";
            if (!_memoryCache.TryGetValue(endpoint, out IEnumerable<int>? data))
            {
                _logger.LogInformation($"{endpoint} not found in cache or has expired, requesting it");
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));

                HttpRequestMessage request = new(HttpMethod.Get, "beststories.json");
                data = await SendRequest<IEnumerable<int>>(request);
                _memoryCache.Set("beststories.json", data, cacheEntryOptions);
            }

            return data?.Take(count) ?? throw new($"An error occured whilst loading data from beststories.json in {nameof(GetBestStoryIds)}");
        }

        public async Task<HackerNewsStory> GetStory(int id)
        {
            _logger.LogInformation($"{nameof(GetStory)} called with: {id}");
            string endpoint = $"item/{id}.json";
            if (!_memoryCache.TryGetValue(endpoint, out HackerNewsStory? data))
            {
                _logger.LogInformation($"{endpoint} not found in cache or has expired, requesting it");
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(300));

                HttpRequestMessage request = new(HttpMethod.Get, endpoint);
                data = await SendRequest<HackerNewsStory>(request);
                _memoryCache.Set(endpoint, data, cacheEntryOptions);
            }

            return data ?? throw new($"An error occured whilst loading data from item/{id}.json in {nameof(GetStory)}");
        }

        private async Task<T?> SendRequest<T>(HttpRequestMessage request)
        {
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Status code received when accessing {request.RequestUri} does not indicate success, processing will continue.");
                return default;
            }

            string resultStr = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(resultStr, serializerOptions) ?? default;
        }
    }
}
