using BusinessLayer.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace BusinessLayer.Vlidators
{
    public class HistoryWeatherRequestDTOValidator : AbstractValidator<HistoryWeatherRequestDTO>
    {
        public HistoryWeatherRequestDTOValidator()
        {
            RuleFor(p => p.CityName)
                .NotEmpty()
                .MaximumLength(20);
            RuleFor(p => p.StartPeriod)
                .Must(x =>
                    x != default)
                .WithErrorCode("NotEmptyValidator");
            RuleFor(p => p.EndPeriod)
               .Must(x =>
                   x != default)
               .WithErrorCode("NotEmptyValidator");
            RuleFor(p => p)
                .Must(x =>
                    x.StartPeriod <= x.EndPeriod)                
                .WithMessage(x => $"'{Regex.Replace(nameof(x.EndPeriod), @"([A-Z])", " $1").Trim()}' must be more or equal than '{Regex.Replace(nameof(x.StartPeriod), @"([A-Z])", " $1").Trim()}'.");
        }
    }
}