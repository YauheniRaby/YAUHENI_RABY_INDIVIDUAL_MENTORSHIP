using BusinessLayer.Command;
using BusinessLayer.Command.Abstract;
using BusinessLayer.Configuration.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Infrastructure;
using BusinessLayer.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly IConfig _config;
        private readonly IInvoker _invoker;

        public WeatherController(IWeatherServiсe weatherServiсe, IConfig config, IInvoker invoker)
        {
            _weatherServiсe = weatherServiсe;
            _config = config;
            _invoker = invoker;
        }

        [HttpGet("CurrentWeather/{cityName}")]
        public async Task<ActionResult<WeatherDTO>> GetCurrentWeatherByCityName([FromRoute] string cityName)
        {
            var command = new CurrentWeatherCommand(_weatherServiсe, cityName);
            var result = await _invoker.RunAsync(command, TokenGenerator.GetCancellationToken(_config.RequestTimeout));
            return Ok(result);
        }
        
        [HttpGet("ForecastWeather/{cityName}/{countDays}")]
        public async Task<ActionResult<ForecastWeatherDTO>> GetForecastWeatherByCityName([FromRoute] string cityName, [FromRoute] int countDays)
        {
            var command = new ForecastWeatherCommand(_weatherServiсe, cityName, countDays);
            var result = await _invoker.RunAsync(command, TokenGenerator.GetCancellationToken(_config.RequestTimeout));
            return Ok(result);
        }
    }
}