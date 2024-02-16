using NewsAggregator.Configuration;
using NewsAggregator.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;
using NewsAggregator.Exceptions;

namespace NewsAggregator.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HackerNewsService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly HackerNewsOptions _config;

        private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);

        public HackerNewsService(HttpClient httpClient, ILogger<HackerNewsService> logger, IMemoryCache memoryCache, IOptionsSnapshot<HackerNewsOptions> config) =>
            (_httpClient, _logger, _memoryCache, _config) = (httpClient, logger, memoryCache, config.Value);

        public async Task<IEnumerable<int>> GetBestStoryIds(int count)
        {
            _logger.LogInformation($"{nameof(GetBestStoryIds)} called with: {count}");
            HackerNewsRequest? requestConfig = _config.Requests?[nameof(GetBestStoryIds)];
            if (requestConfig == null || requestConfig.Endpoint == null)
            {
                throw new Exception($"Loading configuration for {nameof(GetBestStoryIds)} resulted in a null RequestConfig or Endpoint");
            }

            if (!_memoryCache.TryGetValue(requestConfig.Endpoint, out IEnumerable<int>? data))
            {
                _logger.LogInformation($"{requestConfig.Endpoint} not found in cache or has expired, requesting it");
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(requestConfig.Expiry));

                HttpRequestMessage request = new(HttpMethod.Get, requestConfig.Endpoint);
                data = await SendRequest<IEnumerable<int>>(request);
                _memoryCache.Set(requestConfig.Endpoint, data, cacheEntryOptions);
            }

            return data?.Take(count) ?? throw new HackerNewsResultsException($"An error occured whilst loading data from {requestConfig.Endpoint} in {nameof(GetBestStoryIds)}");
        }

        public async Task<HackerNewsStory> GetStory(int id)
        {
            _logger.LogInformation($"{nameof(GetStory)} called with: {id}");
            HackerNewsRequest? requestConfig = _config.Requests?[nameof(GetStory)];
            if (requestConfig == null || requestConfig.Endpoint == null)
            {
                throw new Exception($"Loading configuration for {nameof(GetStory)} resulted in a null RequestConfig or Endpoint");
            }

            string endpoint = string.Format(requestConfig.Endpoint, id);
            if (!_memoryCache.TryGetValue(endpoint, out HackerNewsStory? data))
            {
                _logger.LogInformation($"{endpoint} not found in cache or has expired, requesting it");
                MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(requestConfig.Expiry));

                HttpRequestMessage request = new(HttpMethod.Get, endpoint);
                data = await SendRequest<HackerNewsStory>(request);
                _memoryCache.Set(endpoint, data, cacheEntryOptions);
            }

            return data ?? throw new HackerNewsResultsException($"An error occured whilst loading data from {requestConfig.Endpoint} in {nameof(GetStory)}");
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
