using System.Text.Json.Serialization;
using Microsoft.AspNetCore.RateLimiting;
using Polly;
using TpInsuranceAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IInsuranceDataProvider, HardcodedInsuranceDataProvider>();
builder.Services.AddHttpClient<IVehicleClient, VehicleApiClient>(client =>
{
    var baseUrl = builder.Configuration.GetValue<string>("VehicleApiBaseUrl") ?? "http://localhost:5005";
    client.BaseAddress = new Uri(baseUrl);
})
    .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(200 * i)));

builder.Services.ConfigureHttpJsonOptions(opts =>
    opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

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
app.MapInsuranceEndpoints();

app.Run();

public partial class Program;
