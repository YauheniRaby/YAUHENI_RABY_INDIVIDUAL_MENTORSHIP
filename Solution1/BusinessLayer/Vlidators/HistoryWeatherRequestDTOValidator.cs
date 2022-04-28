using BusinessLayer.DTOs;
using FluentValidation;

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
                .NotEmpty();
            RuleFor(w => w.EndPeriod)
                .NotEmpty();
            RuleFor(w => w.StartPeriod)
                .LessThanOrEqualTo(w => w.EndPeriod)
                .WithMessage(x => $"'{nameof(x.StartPeriod)}' must be less than or equal to '{nameof(x.EndPeriod)}'.");
        }
    }
}