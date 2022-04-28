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
                .NotEmpty();
            RuleFor(w => w.EndPeriod)
                .NotEmpty();
            RuleFor(w => w.StartPeriod)
                .LessThanOrEqualTo(w => w.EndPeriod)
                .WithMessage(x => $"'Start Period' must be less than or equal to 'End Period'.");
        }
    }
}