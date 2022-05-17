using System.Text;
using Ecoba.AzureAuth;
using Ecoba.Consul.ServiceQuery;
using ServiceDiscovery.Consul;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();

var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>();
builder.Services.AddAzureAuthScheme(jwtConfig);

builder.Services.AddScoped<IConsulService, ConsulService>();

var serverConfig = builder.Configuration.GetServiceConfig();
builder.Services.AddConsul(serverConfig);

// builder.Services.AddControllers().AddJsonOptions(option => option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAzureAuth();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();