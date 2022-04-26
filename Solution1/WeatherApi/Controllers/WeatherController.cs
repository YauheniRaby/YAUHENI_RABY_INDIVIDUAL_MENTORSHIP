﻿using BusinessLayer.Command;
using BusinessLayer.Command.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Infrastructure;
using BusinessLayer.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WeatherApi.Configuration;
using BusinessLayer.Helpers;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly IHistoryWeatherService _historyWeatherService;
        private readonly IOptionsMonitor<AppConfiguration> _appConfiguration;
        private readonly IOptionsMonitor<WeatherApiConfiguration> _apiConfiguration;
        private readonly IInvoker _invoker; 

        public WeatherController(IWeatherServiсe weatherServiсe, IHistoryWeatherService historyWeatherService, IOptionsMonitor<AppConfiguration> appConfiguration, IOptionsMonitor<WeatherApiConfiguration> apiConfiguration, IInvoker invoker)
        {
            _weatherServiсe = weatherServiсe;
            _historyWeatherService = historyWeatherService;
            _appConfiguration = appConfiguration;
            _apiConfiguration = apiConfiguration;
            _invoker = invoker;
        }

        [HttpGet("current")]
        public async Task<ActionResult<WeatherDTO>> GetCurrentWeatherByCityNameAsync([FromQuery] string cityName)
        {
            var token = TokenGenerator.GetCancellationToken(_appConfiguration.CurrentValue.RequestTimeout);
            token.ThrowIfCancellationRequested();
            var command = new CurrentWeatherCommand(_weatherServiсe, cityName, UrlHelper.Combine(_apiConfiguration.CurrentValue.CurrentWeatherUrl, _apiConfiguration.CurrentValue.Key));
            var result = await _invoker.RunAsync(command, token);
            return Ok(result);
        }

        [HttpGet("forecast")]
        public async Task<ActionResult<ForecastWeatherDTO>> GetForecastWeatherByCityNameAsync([FromQuery] string cityName, [FromQuery] int countDays)
        {
            var token = TokenGenerator.GetCancellationToken(_appConfiguration.CurrentValue.RequestTimeout);
            token.ThrowIfCancellationRequested();
            var command = new ForecastWeatherCommand(
                _weatherServiсe, 
                cityName, 
                countDays,
                UrlHelper.Combine(_apiConfiguration.CurrentValue.ForecastWeatherUrl, _apiConfiguration.CurrentValue.Key),
                UrlHelper.Combine(_apiConfiguration.CurrentValue.CoordinatesUrl, _apiConfiguration.CurrentValue.Key), 
                _apiConfiguration.CurrentValue.CountPointsInDay);
            var result = await _invoker.RunAsync(command, token);
            return Ok(result);            
        }

        [HttpGet("history")]
        public async Task<ActionResult> GetHistoryWeatherByCityNameAsync([FromQuery] HistoryWeatherRequestDTO requestHistoryWeatherDto)
        {
            var token = TokenGenerator.GetCancellationToken(_appConfiguration.CurrentValue.RequestTimeout);
            token.ThrowIfCancellationRequested();
            var command = new HistoryWeatherCommand(_historyWeatherService, requestHistoryWeatherDto);
            var result = await _invoker.RunAsync(command, token);
            return Ok(result);
        }
    }
}