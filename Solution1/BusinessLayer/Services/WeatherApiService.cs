using BusinessLayer.DTOs;
using BusinessLayer.DTOs.WeatherAPI;
using BusinessLayer.Infrastructure;
using BusinessLayer.Services.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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

        public Task<WeatherApiDTO> GetByCityNameAsync(string cityName)
        {
            var urlResult = string.Format(Constants.WeatherAPI.WeatherByCityName, cityName, Constants.WeatherAPI.KeyApi);
            
            return _httpClient.GetFromJsonAsync<WeatherApiDTO>(urlResult);            
        }

        public async Task<ForecastWeatherApiDTO> GetForecastByCityNameAsync(string cityName, int countWeatherPoint)
        {
            var urlResultForCoordinates = string.Format(Constants.WeatherAPI.CoordinatesByCityName, cityName, Constants.WeatherAPI.KeyApi);

            var coordinatesResponse = await _httpClient.GetAsync(urlResultForCoordinates);
            var coordinatesResponceBody = await coordinatesResponse.Content.ReadAsStringAsync();
            var cityCoordinates = JsonSerializer.Deserialize<List<CityCoordinatesDTO>>(coordinatesResponceBody).FirstOrDefault();

            var urlResultForForecast = string.Format(Constants.WeatherAPI.ForecastByCoordinates, cityCoordinates.Latitude, cityCoordinates.Longitude, countWeatherPoint, Constants.WeatherAPI.KeyApi);

            var forecastResponse = await _httpClient.GetAsync(urlResultForForecast);
            var forecastResponseBody = await forecastResponse.Content.ReadAsStringAsync();

            JsonSerializerOptions options = new();
            options.Converters.Add(new DateTimeConverterUsingDateTimeParse());

            return JsonSerializer.Deserialize<ForecastWeatherApiDTO>(forecastResponseBody, options);
        }
    }
}
