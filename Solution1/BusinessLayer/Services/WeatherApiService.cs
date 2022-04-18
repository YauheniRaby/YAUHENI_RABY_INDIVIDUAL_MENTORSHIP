using BusinessLayer.DTOs;
using BusinessLayer.DTOs.WeatherAPI;
using BusinessLayer.Infrastructure;
using BusinessLayer.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class WeatherApiService : IWeatherApiService
    {
        private readonly HttpClient _httpClient;
        
        public WeatherApiService(HttpClient httpClient)
        {
            _httpClient=httpClient;
        }

        public Task<WeatherApiDTO> GetByCityNameAsync(string cityName, string currentWeatherUrl, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var urlResult = string.Format(currentWeatherUrl, cityName);
            
            return _httpClient.GetFromJsonAsync<WeatherApiDTO>(urlResult, cancellationToken);            
        }

        public async Task<ForecastWeatherApiDTO> GetForecastByCityNameAsync(string cityName, int countWeatherPoint, string forecastWeatherUrl, string coordinatesUrl, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var urlResultForCoordinates = string.Format(coordinatesUrl, cityName);

            var coordinatesResponse = await _httpClient.GetAsync(urlResultForCoordinates, cancellationToken);
            var coordinatesResponceBody = await coordinatesResponse.Content.ReadAsStringAsync(cancellationToken);
            var cityCoordinates = JsonSerializer.Deserialize<List<CityCoordinatesDTO>>(coordinatesResponceBody).FirstOrDefault();

            var urlResultForForecast = string.Format(forecastWeatherUrl, cityCoordinates.Latitude, cityCoordinates.Longitude, countWeatherPoint);

            var forecastResponse = await _httpClient.GetAsync(urlResultForForecast, cancellationToken);
            var forecastResponseBody = await forecastResponse.Content.ReadAsStringAsync(cancellationToken);

            JsonSerializerOptions options = new();
            options.Converters.Add(new DateTimeConverterUsingDateTimeParse());
            Console.WriteLine($"TESTTESTTEST2 - {forecastResponseBody}");
            var t = JsonSerializer.Deserialize<ForecastWeatherApiDTO>(forecastResponseBody, options);
            Console.WriteLine($"TESTTESTTEST3 - {t.WeatherPoints[0].DateTime.ToString("1 -dd, 2 -MM, 3 -yyyy HH:mm:ss")}");
            return t;

            return JsonSerializer.Deserialize<ForecastWeatherApiDTO>(forecastResponseBody, options);
        }
    }
}
