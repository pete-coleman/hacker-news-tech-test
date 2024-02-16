using NewsAggregator.MinimalApis;
using NewsAggregator.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddScoped<IHackerNewsService, HackerNewsService>()
    .AddHttpClient<IHackerNewsService, HackerNewsService>(options =>
    {
        options.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

HackerNews.Bind(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
