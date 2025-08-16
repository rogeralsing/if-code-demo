using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using TpVehicleAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IVehicleDataProvider, HardcodedVehicleDataProvider>();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", o =>
    {
        o.Window = TimeSpan.FromSeconds(1);
        o.PermitLimit = 10;
        o.QueueLimit = 0;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();
app.MapVehicleEndpoints();

app.Run();

public partial class Program;
