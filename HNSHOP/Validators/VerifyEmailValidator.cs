using HNSHOP.Dtos.Request;
using HNSHOP.Utils;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class VerifyEmailValidator : AbstractValidator<VerifyEmailReqDto>
    {
        public VerifyEmailValidator()
        {
            RuleFor(ve => ve.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(ve => ve.Token)
                .NotEmpty()
                .Length(ConstConfig.VerifyEmailTokenLength);
        }
    }
}
