using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Senti.Web.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddRadzenComponents();

//builder.Services.addh
//builder.Services.AddCors(options =>

//{

//    options.AddPolicy("AllowBlazorApp", builder =>

//    {

//        builder.WithOrigins("https://example.com") // the origin of your Blazor app

//               .AllowAnyMethod()

//               .AllowAnyHeader();

//    });

//});

var host = builder.Build();
    
await host.RunAsync();
