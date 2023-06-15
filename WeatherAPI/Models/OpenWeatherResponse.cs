using WeatherAPI.Models.OpenWeatherAPI;

namespace WeatherAPI.Models;

internal record OpenWeatherResponse(Location Location, Current Current);
