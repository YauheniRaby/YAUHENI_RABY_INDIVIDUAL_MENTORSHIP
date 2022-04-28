using BusinessLayer.DTOs;
using BusinessLayer.Vlidators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WeatherApi.Configuration;

namespace WeatherApi.Extensions
{
    public static class ValidatorsRegistration
    {
        public static void AddValidators(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<ForecastWeatherRequestDTO>, ForecastWeatherRequestDTOValidator>(serviseProvider => 
            {
                var appParams = serviseProvider.GetService<IOptions<AppConfiguration>>();
                return new ForecastWeatherRequestDTOValidator(appParams.Value.MinCountDaysForecast, appParams.Value.MaxCountDaysForecast);
            });
            services.AddSingleton<IValidator<HistoryWeatherRequestDTO>, HistoryWeatherRequestDTOValidator>();
        }
    }
}