using Scalar.AspNetCore;
using Senti.Quotes.Api;
using Senti.Quotes.Core.Cache;
using Senti.Quotes.Core.Commands;
using Senti.Shared.Adapters.Storages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<QuoteCacheContext>();
builder.Services.AddTransient<QuoteCacheRepository>();
builder.Services.AddTransient<GetQuotes>();
builder.Services.AddTransient<StorageAdapter>();

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();
app.MapAllMethods();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var repository = serviceProvider.GetRequiredService<QuoteCacheRepository>();

    // Use the repository
    await repository.Init();
    repository.InitData();
}
app.Run();

