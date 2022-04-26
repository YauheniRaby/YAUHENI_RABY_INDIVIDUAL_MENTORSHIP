using BusinessLayer.DTOs;
using FluentValidation;

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
                .WithMessage(x => $"Value \'{nameof(x.StartPeriod)}\' must not be empty.");
            RuleFor(p => p.EndPeriod)
               .Must(x =>
                   x != default)
               .WithMessage(x => $"Value \'{nameof(x.EndPeriod)}\' must not be empty.");
            RuleFor(p => p)
                .Must(x =>
                    x.StartPeriod <= x.EndPeriod)
                .WithMessage(x => $"Value \'{nameof(x.EndPeriod)}\' must be more or equal than value \'{nameof(x.EndPeriod)}\'.");
        }
    }
}