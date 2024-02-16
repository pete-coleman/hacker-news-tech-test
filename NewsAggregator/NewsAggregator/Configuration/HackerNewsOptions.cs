namespace NewsAggregator.Configuration
{
    public record HackerNewsOptions
    {
        public const string Key = "HackerNews";

        public string? ApiRoot { get; set; }
        public string? BaseAddress { get; set; }
        public Uri? BaseUri
        {
            get
            {
                return BaseAddress != null ? new(BaseAddress) : null;
            }
        }
        public string? DateFormat { get; set; }
        public Dictionary<string, HackerNewsRequest>? Requests { get; set; }
    }
}
