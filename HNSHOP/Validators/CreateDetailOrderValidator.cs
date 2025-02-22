using HNSHOP.Dtos.Request;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class CreateDetailOrderValidator : AbstractValidator<CreateDetailOrderReqDto>
    {

        public CreateDetailOrderValidator()
        {
            RuleFor(o => o.ProductId)
                .GreaterThanOrEqualTo(1);

            RuleFor(o => o.Quantity)
                .GreaterThanOrEqualTo(1);

        }
    }
}
