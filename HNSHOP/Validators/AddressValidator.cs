using HNSHOP.Dtos.Request;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class AddressValidator : AbstractValidator<AddressReqDto>
    {
        public AddressValidator()
        {
            RuleFor(a => a.HouseNumber)
                .NotEmpty();

            RuleFor(a => a.Street)
                .NotEmpty();

            RuleFor(a => a.Ward)
                .NotEmpty();

            RuleFor(a => a.District)
                .NotEmpty();

            RuleFor(a => a.City)
                .NotEmpty();
        }
    }
}
