using HNSHOP.Dtos.Request;
using HNSHOP.Utils;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterReqDto>
    {
        public RegisterValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(r => r.Phone)
                .NotEmpty()
                .Matches(@"^(?:\+84|0)(?:3|5|7|8|9)\d{8}$");

            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(ConstConfig.DisplayNameLength);

            RuleFor(r => r.Password)
                .MinimumLength(ConstConfig.MinPasswordLength)
                .MaximumLength(ConstConfig.MaxPasswordLength);

            RuleFor(r => r.ConfirmPassword)
                .NotEmpty()
                .Equal(r => r.Password)
                .WithMessage("Confirm password is not matched!");
        }
    }
}
