using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // 
        string apiKey = "c823abb7adae9a183bbbd5b04a456011";
        int numberOfWeatherData = 50;

        List<MyWeather> weatherData = new List<MyWeather>();
        Random random = new Random();

        for (int i = 0; i < numberOfWeatherData; i++)
        {
            double latitude = 90 - random.NextDouble() * 180;
            double longitude = 180 - random.NextDouble() * 360;

            string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}";



            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);

                    Root root = JsonConvert.DeserializeObject<Root>(json);
                    MyWeather weather = new MyWeather();
                    weather.country = root.sys.country;
                    weather.description = root.weather[0].description;
                    weather.temp = root.main.temp;
                    weather.name = root.name;



                    if (string.IsNullOrEmpty(weather.country) || string.IsNullOrEmpty(weather.name))
                    {
                        i--;
                    }
                    else
                    { 
                        Console.WriteLine($"{weather.country}, {weather.name}, {weather.temp}, {weather.description}.");
                        weatherData.Add(weather);
                    }


                }
            }
        }

        Console.WriteLine($"weatherData size = {weatherData.Count}");


        var maxTempCountry = weatherData.Aggregate((max, next) => next.temp > max.temp ? next : max).country;
        var minTempCountry = weatherData.Aggregate((min, next) => next.temp < min.temp ? next : min).country;
        var averageTemp = weatherData.Average(w => w.temp);
        var countryCount = weatherData.Select(w => w.country).Distinct().Count();

        var firstClearSky = weatherData.FirstOrDefault(w => w.description == "clear sky");
        var firstRain = weatherData.FirstOrDefault(w => w.description == "rain");
        var firstFewClouds = weatherData.FirstOrDefault(w => w.description == "few clouds");

        Console.WriteLine($"Country with maximum temperature: {maxTempCountry}");
        Console.WriteLine($"Country with minimum temperature: {minTempCountry}");
        Console.WriteLine($"Average temperature in the world: {averageTemp:F2}");
        Console.WriteLine($"Number of unique countries: {countryCount}");

        if (!firstClearSky.Equals(default(Weather)))
        {
            Console.WriteLine($"First 'clear sky' location: {firstClearSky.country}, {firstClearSky.name}");
        }

        if (!firstRain.Equals(default(Weather)))
        {
            Console.WriteLine($"First 'rain' location: {firstRain.country}, {firstRain.name}");
        }

        if (!firstFewClouds.Equals(default(Weather)))
        {
            Console.WriteLine($"First 'few clouds' location: {firstFewClouds.country}, {firstFewClouds.name}");
        }
        Console.WriteLine("end");
        Console.ReadKey();
    }
}

public struct MyWeather
{
    public string country { get; set; }
    public string name { get; set; }
    public double temp { get; set; }
    public string description { get; set; }
}

public class Clouds
{
    public int all { get; set; }
}

public class Coord
{
    public double lon { get; set; }
    public double lat { get; set; }
}

public class Main
{
    public double temp { get; set; }
    public double feels_like { get; set; }
    public double temp_min { get; set; }
    public double temp_max { get; set; }
    public int pressure { get; set; }
    public int humidity { get; set; }
    public int sea_level { get; set; }
    public int grnd_level { get; set; }
}

public class Root
{
    public Coord coord { get; set; }
    public List<Weather> weather { get; set; }
    public string @base { get; set; }
    public Main main { get; set; }
    public int visibility { get; set; }
    public Wind wind { get; set; }
    public Clouds clouds { get; set; }
    public int dt { get; set; }
    public Sys sys { get; set; }
    public int timezone { get; set; }
    public int id { get; set; }
    public string name { get; set; }
    public int cod { get; set; }
}

public class Sys
{
    public int type { get; set; }
    public int id { get; set; }
    public string country { get; set; }
    public int sunrise { get; set; }
    public int sunset { get; set; }
}

public class Weather
{
    public int id { get; set; }
    public string main { get; set; }
    public string description { get; set; }
    public string icon { get; set; }
}

public class Wind
{
    public double speed { get; set; }
    public int deg { get; set; }
    public double gust { get; set; }
}

