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
            // Здесь можно добавить правила для проверки адреса
            // Например, если AddressRequest имеет поля Street, City, ZipCode и т.д.
            RuleFor(x => x.Street)
                .NotEmpty().WithMessage("Street is required");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required");
        }
    }
}
