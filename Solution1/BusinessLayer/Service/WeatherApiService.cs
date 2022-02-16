using BusinessLayer.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BusinessLayer.Service.Abstract
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
            var urlResult = string.Format(Constants.WeatherAPI.UrlApi, cityName, Constants.WeatherAPI.KeyApi);

            return _httpClient.GetFromJsonAsync<WeatherApiDTO>(urlResult);            
        }
    }
}
