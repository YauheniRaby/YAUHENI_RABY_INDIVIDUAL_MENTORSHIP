using BusinessLayer.Configuration.Abstract;
using BusinessLayer.DTOs;
using FluentValidation;

namespace BusinessLayer.Vlidators
{
    public class ForecastWeatherRequestDTOValidator : AbstractValidator<ForecastWeatherRequestDTO>
    {
        public ForecastWeatherRequestDTOValidator(IConfig config)
        {
            RuleSet("CityName", () =>
            {
                RuleFor(p => p.CityName)
                    .NotEmpty()
                    .MaximumLength(20);
            });

            RuleFor(p => p.PeriodOfDays)
                .InclusiveBetween(config.MinCountDaysForecast, config.MaxCountDaysForecast);
        }
    }
}
