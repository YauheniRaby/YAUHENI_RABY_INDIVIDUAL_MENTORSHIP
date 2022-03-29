using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherServiсe _weatherServiсe;

        public WeatherController(IWeatherServiсe weatherServiсe)
        {
            _weatherServiсe = weatherServiсe;            
        }

        [HttpGet("CurrentWeather/{cityName}")]
        public async Task<ActionResult<WeatherDTO>> GetCurrentWeatherByCityName([FromRoute] string cityName)
        {
            var result = await _weatherServiсe.GetByCityNameAsync(cityName, CancellationToken.None);
            return Ok(result);
        }
        
        [HttpGet("ForecastWeather/{cityName}/{countDays}")]
        public async Task<ActionResult<ForecastWeatherDTO>> GetForecastWeatherByCityName([FromRoute] string cityName, [FromRoute] int countDays)
        {
            var result = await _weatherServiсe.GetForecastByCityNameAsync(cityName, countDays, CancellationToken.None);
            return Ok(result);
        }
    }
}