namespace NewsAggregator.MinimalApis
{
    public static class HackerNews
    {
        public static void Bind(WebApplication app)
        {
            app.MapGet($"api/hackernews/stories/best", (int count) =>
            {
                if (count <= 0) {
                    return Results.BadRequest();
                }
                return Results.Ok();
            })
            .WithName("GetBestNStories")
            .WithOpenApi();
        }
    }
}
