using Scalar.AspNetCore;
using Senti.News.Api;
using Senti.Shared.Adapters.Storages;
using Senti.News.Core.Cache;
using Senti.News.Core.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ArticleCacheContext>();
builder.Services.AddTransient<ArticleCacheRepository>();
builder.Services.AddTransient<GetArticles>();
builder.Services.AddTransient<StorageAdapter>();
builder.Services.AddHostedService<HourlyBackgroundService>();

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();
app.MapAllMethods();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var repository = serviceProvider.GetRequiredService<ArticleCacheRepository>();

    await repository.Init();
}
app.Run();
