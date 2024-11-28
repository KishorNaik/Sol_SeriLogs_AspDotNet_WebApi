using Microsoft.AspNetCore.Mvc;
using Sol_Demos.Extensions.Services;

namespace Sol_Demos.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class WeatherForecastController : ControllerBase
{
    private readonly DataResponseFactory _dataResponseFactory;

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, DataResponseFactory dataResponseFactory)
    {
        _logger = logger;
        _dataResponseFactory = dataResponseFactory;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("GetWeatherForecast: Start");

        var lists = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();

        _logger.LogInformation("GetWeatherForecast: length:{length}", lists.Length);
        _logger.LogInformation("GetWeatherForecast: Data:{@lists}", lists.ToArray());

        _logger.LogInformation("GetWeatherForecast: Finish");

        return lists;
    }

    [HttpPost("test")]
    public IActionResult Test([FromBody] User user)
    {
        //_logger.LogInformation("Test");

        var response = _dataResponseFactory.SetResponse<User>(true, "Success", 200, user);

        return base.Ok(response);
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class DataResponse<T>
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public int StatusCode { get; set; }

    public T? Data { get; set; }

    public string? TraceId { get; set; }
}

public class DataResponseFactory
{
    private readonly ITraceIdService _traceIdService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DataResponseFactory(ITraceIdService traceIdService, IHttpContextAccessor httpContextAccessor)
    {
        _traceIdService = traceIdService;
        _httpContextAccessor = httpContextAccessor;
    }

    public DataResponse<T> SetResponse<T>(bool success, string? message, int statusCode, T? data)
    {
        return new DataResponse<T>
        {
            Success = success,
            Message = message,
            StatusCode = statusCode,
            Data = data,
            TraceId = _traceIdService.GetOrGenerateTraceId(_httpContextAccessor.HttpContext!)
        };
    }
}