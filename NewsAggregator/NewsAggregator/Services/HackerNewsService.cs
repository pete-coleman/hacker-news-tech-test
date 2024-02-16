using NewsAggregator.Models;
using System.Text.Json;

namespace NewsAggregator.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HackerNewsService> _logger;

        private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);

        public HackerNewsService(HttpClient httpClient, ILogger<HackerNewsService> logger) => (_httpClient, _logger) = (httpClient, logger);

        public async Task<IEnumerable<int>> GetBestStoryIds(int count)
        {
            _logger.LogInformation($"{nameof(GetBestStoryIds)} called with: {count}");
            HttpRequestMessage request = new(HttpMethod.Get, "beststories.json");
            IEnumerable<int>? data = await SendRequest<IEnumerable<int>>(request);

            return data?.Take(count) ?? throw new($"An error occured whilst loading data from beststories.json in {nameof(GetBestStoryIds)}");
        }

        public async Task<HackerNewsStory> GetStory(int id)
        {
            _logger.LogInformation($"{nameof(GetStory)} called with: {id}");

            HttpRequestMessage request = new(HttpMethod.Get, $"item/{id}.json");
            HackerNewsStory? data = await SendRequest<HackerNewsStory>(request);

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
