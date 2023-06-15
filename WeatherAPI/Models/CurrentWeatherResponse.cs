namespace WeatherAPI.Models;

internal record CurrentWeatherResponse()
{
    public string Name { get; init; }
        
    public decimal Temp { get; init; }
    
    public string Description { get; init; }
    
    public decimal WindSpeedKph { get; init; }
    
    public int Humidity { get; init; }
    
    public decimal FeelsLike { get; init; }
}