using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;
using UserService.Service;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDiscoveryClient(builder.Configuration);
// Add services to the container.
builder.Services.AddServiceDiscovery(o => o.UseEureka());
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddScoped <IUserService,UserServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

app.Run();
