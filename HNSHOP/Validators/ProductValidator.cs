using HNSHOP.Dtos.Request;
using HNSHOP.Utils;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class ProductValidator : AbstractValidator<ProductReqDto>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .MaximumLength(ConstConfig.MediumNameLength);

            RuleFor(p => p.Description)
                .NotEmpty()
                .MaximumLength(ConstConfig.DescriptionLength);

            RuleFor(p => p.Price)
                .GreaterThanOrEqualTo(0m);

            RuleFor(p => p.Quantity)
                .GreaterThanOrEqualTo(0);

            RuleFor(p => p.ProductTypeId)
                .GreaterThanOrEqualTo(0);
        }
    }
}
