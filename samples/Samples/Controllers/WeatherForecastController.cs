using Microsoft.AspNetCore.Mvc;
using Samples.Services;

namespace Samples.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherService _weatherService;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IWeatherService weatherService)
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        var summary = _weatherService.GetSummary(1);
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = _weatherService.GetRandomSummary()
        })
        .ToArray();
    }
}
