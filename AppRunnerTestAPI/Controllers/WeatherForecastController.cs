using Microsoft.AspNetCore.Mvc;
using TestAPIClassLibrary;

namespace AppRunnerTestAPI.Controllers;

[ApiController]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("api/getWeatherCondtions")]
    public IEnumerable<WeatherForecast> Get()
    {
        var items = WeatherService.GetWeatherCondtions();
        return items;
    }
    
    [HttpGet]
    [Route("api/health")]
    public IActionResult HealthCheck()
    {
        return Ok("App is healthy");
    }
}