using BusinessLayer.DTOs;
using FluentValidation;
using System.Globalization;

namespace BusinessLayer.Vlidators
{
    public class ForecastWeatherRequestDTOValidator : AbstractValidator<ForecastWeatherRequestDTO>
    {
        public ForecastWeatherRequestDTOValidator(int minCountDays, int maxCountDays)
        {
            ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en-US");
            RuleSet("CityName", () =>
            {
                RuleFor(p => p.CityName)
                    .NotEmpty()
                    .MaximumLength(20);
            });

            RuleFor(p => p.PeriodOfDays)
                .InclusiveBetween(minCountDays, maxCountDays);
        }
    }
}