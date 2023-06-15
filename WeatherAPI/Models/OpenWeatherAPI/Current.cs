namespace WeatherAPI.Models.OpenWeatherAPI;

internal record Current()
{
    public decimal Temp_C { get; init; }

    public Condition Condition { get; init; }

    public decimal Wind_Kph { get; init; }

    public decimal Feelslike_C { get; init; }

    public int Humidity { get; init; }
}
