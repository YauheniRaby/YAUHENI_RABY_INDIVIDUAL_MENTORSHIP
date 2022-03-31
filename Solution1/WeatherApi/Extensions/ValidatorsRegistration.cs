using BusinessLayer.DTOs;
using BusinessLayer.Vlidators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Configuration;

namespace WeatherApi.Extensions
{
    public static class ValidatorsRegistration
    {
        public static void AddValidators(this IServiceCollection services, Config config)
        {
            services.AddSingleton<IValidator<ForecastWeatherRequestDTO>>(service => new ForecastWeatherRequestDTOValidator(config.MinCountDaysForecast, config.MaxCountDaysForecast));
        }
    }
}