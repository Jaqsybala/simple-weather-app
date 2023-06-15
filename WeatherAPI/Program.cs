using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Serilog;
using WeatherAPI.Exceptions;
using WeatherAPI.Middlewares;
using WeatherAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddSingleton<GlobalExceptionMiddleware>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseCors();
app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapGet("api/weather/current", async (
    [FromQuery] string city,
    [FromQuery] string lang,
    CancellationToken cancellationToken) =>
{
    var apiKey = builder.Configuration.GetSection("OpenWeatherAPI:Key").Value;

    using var client = new HttpClient();

    client.BaseAddress = new Uri("http://api.weatherapi.com/");

    var response = await client.GetAsync($"v1/current.json?key={apiKey}&q={city}&lang={lang}", cancellationToken);

    if (string.IsNullOrEmpty(city))
    {
        throw new BadRequestException("City field must not be empty");
    }

    if (!response.IsSuccessStatusCode)
    {
        throw new BadRequestException($"Location {city} not found");
    }

    var stringResult = await response.Content.ReadAsStringAsync(cancellationToken);
    var rawWeather = JsonConvert.DeserializeObject<OpenWeatherResponse>(stringResult);

    var currentWeatherResponse = new CurrentWeatherResponse()
    {
        Name = rawWeather.Location.Name,
        Temp = Math.Round(rawWeather.Current.Temp_C),
        Description = rawWeather.Current.Condition.Text,
        WindSpeedKph = rawWeather.Current.Wind_Kph,
        Humidity = rawWeather.Current.Humidity,
        FeelsLike = Math.Round(rawWeather.Current.Feelslike_C)
    };

    return Results.Ok(currentWeatherResponse);
})
.WithOpenApi()
.WithName("GetCurrentWeather")
.Produces<CurrentWeatherResponse>(200)
.Produces<ErrorResult>(400);

app.Run();
