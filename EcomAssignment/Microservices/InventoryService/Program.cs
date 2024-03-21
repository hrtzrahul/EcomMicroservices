using Microsoft.Extensions.DependencyInjection;
using InventoryService.Service;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddServiceDiscovery(o => o.UseEureka());


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IInventoryService, InventoryServices>();
builder.Services.AddMemoryCache();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IHealthCheckHandler, ScopedEurekaHealthCheckHandler>();
builder.Services.AddDiscoveryClient(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDiscoveryClient();
app.UseAuthentication();
app.UseAuthorization();
app.UseHealthChecks("/info");

app.MapControllers();

app.Run();
