using BusinessLayer.DTOs;
using FluentValidation;
using System;
using System.Text.RegularExpressions;

namespace BusinessLayer.Vlidators
{
    public class HistoryWeatherRequestDTOValidator : AbstractValidator<HistoryWeatherRequestDTO>
    {
        public HistoryWeatherRequestDTOValidator()
        {
            RuleFor(w => w.CityName)
                .NotEmpty()
                .MaximumLength(20);
            RuleFor(w => w.StartPeriod)
                .NotEqual((DateTime)default)
                .WithErrorCode("NotEmptyValidator");
            RuleFor(w => w.EndPeriod)
               .NotEqual((DateTime)default)
               .WithErrorCode("NotEmptyValidator");
            RuleFor(w => w.StartPeriod)
                .LessThanOrEqualTo(w => w.EndPeriod)
                .WithMessage(x => $"'{Regex.Replace(nameof(x.StartPeriod), @"([A-Z])", " $1").Trim()}' must be less than or equal to '{Regex.Replace(nameof(x.EndPeriod), @"([A-Z])", " $1").Trim()}'.");
        }
    }
}