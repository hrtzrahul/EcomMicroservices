using Ocelot.DependencyInjection;
using Steeltoe.Discovery.Client;
using JwtAuthenticationManager;
using Ocelot.Middleware;
using Ocelot.Provider.Eureka;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("./ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddOcelot(builder.Configuration).AddEureka();

builder.Services.AddCustomJwtAuthentication();



builder.Services.AddDiscoveryClient(builder.Configuration);




var app = builder.Build();
await app.UseOcelot();

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.Run();
