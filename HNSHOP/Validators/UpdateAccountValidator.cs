using HNSHOP.Dtos.Request;
using HNSHOP.Utils;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class UpdateAccountValidator : AbstractValidator<UpdateAccountReqDto>
    {
        public UpdateAccountValidator()
        {
            RuleFor(a => a.Avatar)
                .MaximumLength(ConstConfig.ImagePathLength);

            RuleFor(a => a.Email)
                .EmailAddress();

            RuleFor(a => a.Phone)
                .Matches(@"^(?:\+84|0)(?:3|5|7|8|9)\d{8}$");
        }
    }
}
