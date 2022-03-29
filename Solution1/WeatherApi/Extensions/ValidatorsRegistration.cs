using BusinessLayer.DTOs;
using BusinessLayer.Vlidators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherApi.Extensions
{
    public static class ValidatorsRegistration
    {
        public static void AddValidators(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<ForecastWeatherRequestDTO>, ForecastWeatherRequestDTOValidator>();
        }
    }
}