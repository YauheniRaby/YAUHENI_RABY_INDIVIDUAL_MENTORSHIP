using BusinessLayer.DTOs;
using FluentValidation;

namespace BusinessLayer.Vlidators
{
    public class ForecastWeatherRequestDTOValidator : AbstractValidator<ForecastWeatherRequestDTO>
    {
        public ForecastWeatherRequestDTOValidator(int minCountDays, int maxCountDays)
        {
            RuleSet(Constants.Validators.OnlyCityName, () =>
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