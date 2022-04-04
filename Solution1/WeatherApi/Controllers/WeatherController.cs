﻿using BusinessLayer.Command;
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
        private readonly IOptions<AppConfiguration> _appParams;
        private readonly IInvoker _invoker;

        public WeatherController(IWeatherServiсe weatherServiсe, IOptions<AppConfiguration> appParams, IInvoker invoker)
        {
            _weatherServiсe = weatherServiсe;
            _appParams = appParams;
            _invoker = invoker;
        }
        [HttpGet("current")]
        public async Task<ActionResult<WeatherDTO>> GetCurrentWeatherByCityNameAsync([FromQuery] string cityName)
        {
            var token = TokenGenerator.GetCancellationToken(_appParams.Value.RequestTimeout);
            token.ThrowIfCancellationRequested();
            var command = new CurrentWeatherCommand(_weatherServiсe, cityName);
            var result = await _invoker.RunAsync(command, token);
            return Ok(result);
        }

        [HttpGet("forecast")]
        public async Task<ActionResult<ForecastWeatherDTO>> GetForecastWeatherByCityNameAsync([FromQuery] string cityName, [FromQuery] int countDays)
        {
            var token = TokenGenerator.GetCancellationToken(_appParams.Value.RequestTimeout);
            token.ThrowIfCancellationRequested();
            var command = new ForecastWeatherCommand(_weatherServiсe, cityName, countDays);
            var result = await _invoker.RunAsync(command, token);
            return Ok(result);            
        }
    }
}