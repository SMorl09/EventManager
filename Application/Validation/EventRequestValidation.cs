using Application.DTO.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class EventRequestValidation : AbstractValidator<EventRequest>
    {
        public EventRequestValidation()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Event name is required")
                .Length(3, 100).WithMessage("The title must contain 3 to 100 characters");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required")
                .Must(BeAValidDate).WithMessage("Invalid date format for StartDate");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required");

            RuleFor(x => x.MaxNumberOfUsers)
                .GreaterThan(0).WithMessage("Max number of users must be greater than 0");

            When(x => x.Address != null, () =>
            {
                RuleFor(x => x.Address).SetValidator(new AddressRequestValidation());
            });

        }
        private bool BeAValidDate(string startDate)
        {
            return DateOnly.TryParse(startDate, out _);
        }
    }
}
