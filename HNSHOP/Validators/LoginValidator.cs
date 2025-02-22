using HNSHOP.Dtos.Request;
using HNSHOP.Utils;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class LoginValidator : AbstractValidator<LoginReqDto>
    {
        public LoginValidator()
        {
            RuleFor(l => l.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(l => l.Password)
                .NotEmpty()
                .MinimumLength(ConstConfig.MinPasswordLength)
                .MaximumLength(ConstConfig.MaxPasswordLength);
        }
    }
}
