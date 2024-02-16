using NewsAggregator.Configuration;
using NewsAggregator.MinimalApis;
using NewsAggregator.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HackerNewsOptions>(
    builder.Configuration.GetSection(HackerNewsOptions.Key));

HackerNewsOptions? hackerNewsConfig = builder.Configuration.GetSection(HackerNewsOptions.Key).Get<HackerNewsOptions>();

// Add services to the container.
builder.Services
    .AddScoped<IHackerNewsService, HackerNewsService>()
    .AddMemoryCache()
    .AddHttpClient<IHackerNewsService, HackerNewsService>(options =>
    {
        options.BaseAddress = hackerNewsConfig?.BaseUri;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

HackerNews.Bind(app, hackerNewsConfig);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
