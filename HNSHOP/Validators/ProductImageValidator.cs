using HNSHOP.Dtos.Request;
using HNSHOP.Utils;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class ProductImageValidator : AbstractValidator<ProductImageReqDto>
    {
        public ProductImageValidator()
        {
            RuleFor(pi => pi.Path)
                .NotEmpty()
                .MaximumLength(ConstConfig.ImagePathLength);
        }
    }
}
