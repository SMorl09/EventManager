using Application.DTO.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class UserRequestValidation : AbstractValidator<UserRequest>
    {
        public UserRequestValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(2, 100).WithMessage("The name must contain 2 to 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required");

            RuleFor(x => x.Surename)
                .NotEmpty().WithMessage("Surename is required")
                .Length(2, 100).WithMessage("The surename must contain 2 to 100 characters");

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("BirthDate is required")
                .Must(BeAValidDate).WithMessage("BirthDate must be a valid date.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be a valid email address.");
        }

        private bool BeAValidDate(string birthDate)
        {
            return DateOnly.TryParse(birthDate, out _);
        }
    }
}
