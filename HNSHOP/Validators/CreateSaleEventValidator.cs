using HNSHOP.Dtos.Request;
using HNSHOP.Utils;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class CreateSaleEventValidator : AbstractValidator<CreateSaleEventReqDto>
    {
        public CreateSaleEventValidator()
        {
            RuleFor(se => se.Name)
                .NotEmpty()
                .MaximumLength(ConstConfig.LongNameLength);

            RuleFor(se => se.Description)
                .NotEmpty()
                .MaximumLength(ConstConfig.LongDescriptionLength);

            RuleFor(se => se.Illustration)
                .NotEmpty();

            RuleFor(se => se.Discount)
                 .GreaterThan(0f)
                 .LessThanOrEqualTo(1f);

            RuleFor(se => se.EndDate)
                .GreaterThan(se => se.StartDate);

            RuleFor(se => se.StartDate)
                .LessThan(se => se.EndDate);

            RuleFor(se => se.CustomerTypeIds)
                .NotEmpty();

            RuleFor(se => se.ProductIds)
                .NotEmpty();
        }
    }
}
