using NewsAggregator.Models;

namespace NewsAggregator.Services
{
    public interface IHackerNewsService
    {
        Task<IEnumerable<int>> GetBestStoryIds(int count);
        Task<HackerNewsStory> GetStory(int id);
    }
}