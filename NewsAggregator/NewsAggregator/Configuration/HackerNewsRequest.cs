namespace NewsAggregator.Configuration
{
    public record HackerNewsRequest
    {
        public string? Endpoint { get; set; }
        public int Expiry { get; set; }
    }
}
