using BusinessLayer.Command;
using BusinessLayer.Command.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Infrastructure;
using BusinessLayer.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WeatherApi.Configuration;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly IOptions<AppParams> _appParams;
        private readonly IInvoker _invoker;

        public WeatherController(IWeatherServiсe weatherServiсe, IOptions<AppParams> appParams, IInvoker invoker)
        {
            _weatherServiсe = weatherServiсe;
            _appParams = appParams;
            _invoker = invoker;
        }

        [HttpGet("{cityName}")]
        public async Task<ActionResult<WeatherDTO>> GetCurrentWeatherByCityNameAsync([FromRoute] string cityName)
        {
            var token = TokenGenerator.GetCancellationToken(_appParams.Value.RequestTimeout);
            token.ThrowIfCancellationRequested();
            var command = new CurrentWeatherCommand(_weatherServiсe, cityName);
            var result = await _invoker.RunAsync(command, token);
            return Ok(result);
        }
        
        [HttpGet("{cityName}/{countDays}")]
        public async Task<ActionResult<ForecastWeatherDTO>> GetForecastWeatherByCityNameAsync([FromRoute] string cityName, [FromRoute] int countDays)
        {
            var token = TokenGenerator.GetCancellationToken(_appParams.Value.RequestTimeout);
            token.ThrowIfCancellationRequested();
            var command = new ForecastWeatherCommand(_weatherServiсe, cityName, countDays);
            var result = await _invoker.RunAsync(command, token);
            return Ok(result);            
        }
    }
}