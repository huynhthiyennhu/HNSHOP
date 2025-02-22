using HNSHOP.Dtos.Request;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class RefreshVerificationValidator : AbstractValidator<RefreshVerificationReqDto>
    {
        public RefreshVerificationValidator()
        {
            RuleFor(e => e.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
