using Application.DTO.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class AddressRequestValidation : AbstractValidator<AddressRequest>
    {
        public AddressRequestValidation()
        {
            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required");
            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required");
        }
    }
}
