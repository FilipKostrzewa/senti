using Scalar.AspNetCore;
using Senti.Quotes.Core.Cache;
using Senti.Quotes.Core.Commands;
using Senti.Shared.Adapters.Storages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<QuoteCacheContext>();
builder.Services.AddTransient<QuoteCacheRepository>();
builder.Services.AddTransient<GetQuotes>();
builder.Services.AddTransient<StorageAdapter>();

var app = builder.Build();

if (true || app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();




app.MapGet("/quotes/{stock}", (GetQuotes getQuotes, string stock) =>
{
    return getQuotes.GetByStock(stock);
});

app.MapGet("/quotes", (HttpRequest request, GetQuotes getQuotes) =>
{
    var stock = request.Query["stock"];
    var from = long.Parse(request.Query["from"]);
    var to = long.Parse(request.Query["to"]);

    return getQuotes.Get(stock, from, to);
});

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    var repository = serviceProvider.GetRequiredService<QuoteCacheRepository>();

    // Use the repository
    await repository.Init();
    repository.InitData();
}
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
