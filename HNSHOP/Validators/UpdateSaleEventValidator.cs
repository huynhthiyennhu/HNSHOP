using HNSHOP.Dtos.Request;
using HNSHOP.Utils;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class UpdateSaleEventValidator : AbstractValidator<UpdateSaleEventReqDto>
    {
        public UpdateSaleEventValidator()
        {
            RuleFor(se => se.Name)
                .MaximumLength(ConstConfig.LongNameLength);

            RuleFor(se => se.Description)
                .MaximumLength(ConstConfig.LongDescriptionLength);

            RuleFor(se => se.Illustration)
                .MaximumLength(ConstConfig.ImagePathLength);

            RuleFor(se => se.Discount)
                .GreaterThan(0f)
                .LessThanOrEqualTo(1f);

            RuleFor(se => se.EndDate)
                .GreaterThan(se => se.StartDate);

            RuleFor(se => se.StartDate)
               .LessThan(se => se.EndDate);
        }
    }
}
