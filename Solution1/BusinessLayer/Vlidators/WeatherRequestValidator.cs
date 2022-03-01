using BusinessLayer.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Vlidators
{
    public class WeatherRequestValidator : AbstractValidator<DataForWeatherRequestDTO>
    {
        public WeatherRequestValidator()
        {
            RuleSet("CityName", () =>
            {
                RuleFor(p => p.CityName)
                    .NotEmpty()
                    .WithMessage("Entered empty value.")
                    .MaximumLength(20)
                    .WithMessage("Entered very long value.");
            });

            RuleFor(p => p.PeriodOfDays)
                .InclusiveBetween(1, 5)
                .WithMessage("Entered value is out of range.");
        }
    }
}
