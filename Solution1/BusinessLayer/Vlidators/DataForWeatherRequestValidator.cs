using BusinessLayer.DTOs;
using FluentValidation;
using System;
using System.Configuration;

namespace BusinessLayer.Vlidators
{
    public class DataForWeatherRequestValidator : AbstractValidator<ForecastWeatherRequestDTO>
    {
        public DataForWeatherRequestValidator()
        {
            RuleSet("CityName", () =>
            {
                RuleFor(p => p.CityName)
                    .NotEmpty()
                    .MaximumLength(20);
            });

            RuleFor(p => p.PeriodOfDays)
                .InclusiveBetween(
                    Convert.ToInt32(ConfigurationManager.AppSettings["minCountDays"]),
                    Convert.ToInt32(ConfigurationManager.AppSettings["maxCountDays"]))
                .WithMessage("Entered value is out of range.");
        }
    }
}
