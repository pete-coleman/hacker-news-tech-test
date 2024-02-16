using NewsAggregator.Configuration;
using NewsAggregator.Models;
using NewsAggregator.Services;

namespace NewsAggregator.MinimalApis
{
    public static class HackerNews
    {
        public static void Bind(WebApplication app, HackerNewsOptions? config)
        {
            app.MapGet($"{config?.ApiRoot}/stories/best", async (IHackerNewsService hackerNewsService, int count) =>
            {
                if (count <= 0)
                {
                    return Results.BadRequest();
                }

                IEnumerable<int> storyIds = await hackerNewsService.GetBestStoryIds(count);
                IEnumerable<Task<HackerNewsStory>> httpTasks = storyIds.Select(x => hackerNewsService.GetStory(x));
                HackerNewsStory[] stories = await Task.WhenAll(httpTasks);

                return Results.Ok(stories.Where(s => s != default).OrderByDescending(s => s.Descendants).Select(s => new
                {
                    title = s.Title,
                    uri = s.Url,
                    postedBy = s.By,
                    time = DateTimeOffset.FromUnixTimeSeconds(s.Time).LocalDateTime.ToString(config?.DateFormat),
                    score = s.Score,
                    commentCount = s.Descendants
                }));
            });
        }
    }
}
