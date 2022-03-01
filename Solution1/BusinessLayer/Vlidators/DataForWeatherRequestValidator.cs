using BusinessLayer.DTOs;
using FluentValidation;
using System;
using System.Configuration;

namespace BusinessLayer.Vlidators
{
    public class DataForWeatherRequestValidator : AbstractValidator<DataForWeatherRequestDTO>
    {
        public DataForWeatherRequestValidator()
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
                .InclusiveBetween(
                    Convert.ToInt32(ConfigurationManager.AppSettings["minCounDays"]),
                    Convert.ToInt32(ConfigurationManager.AppSettings["maxCounDays"]))
                .WithMessage("Entered value is out of range.");
        }
    }
}
