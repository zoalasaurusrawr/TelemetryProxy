using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Samples.Services;
using TelemetryProxy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var resourceBuilder = ResourceBuilder.CreateDefault().AddService("Samples");
builder.Services.AddOpenTelemetryTracing((builder) => builder
        .SetResourceBuilder(resourceBuilder)
        .AddSource(TelemetrySource.ActivitySource.Name)
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter()
    );
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTelemetryProxy();
builder.Services.AddProxiedScoped<IWeatherService, WeatherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
