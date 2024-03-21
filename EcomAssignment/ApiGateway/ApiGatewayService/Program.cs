using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using JWTAuthentication;

using Ocelot.Provider.Eureka;
using Ocelot.Provider.Polly;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Configuration
//    .SetBasePath(builder.Environment.ContentRootPath)
//    .AddJsonFile("./ocelot.json", optional: false, reloadOnChange: true)
//    .AddEnvironmentVariables();
builder.Configuration.AddJsonFile("ocelot.json");
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration).AddEureka();

builder.Services.AddCustomJwtAuthentication();


builder.Services.AddDiscoveryClient(builder.Configuration);



var app = builder.Build();

await app.UseOcelot();

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.Run();
