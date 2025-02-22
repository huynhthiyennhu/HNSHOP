using HNSHOP.Dtos.Request;
using HNSHOP.Utils;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class ProductTypeValidator : AbstractValidator<ProductTypeReqDto>
    {
        public ProductTypeValidator()
        {
            RuleFor(pt => pt.Name)
                .NotEmpty()
                .MaximumLength(ConstConfig.MediumNameLength);

            RuleFor(pt => pt.Description)
                .NotEmpty()
                .MaximumLength(ConstConfig.DescriptionLength);
        }
    }
}
