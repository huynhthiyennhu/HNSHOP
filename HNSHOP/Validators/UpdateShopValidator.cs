using HNSHOP.Dtos.Request;
using HNSHOP.Utils;
using FluentValidation;

namespace HNSHOP.Validators
{
    public class UpdateShopValidator : AbstractValidator<UpdateShopReqDto>
    {
        public UpdateShopValidator()
        {
            RuleFor(s => s.Name)
                .MaximumLength(ConstConfig.LongNameLength);

            RuleFor(s => s.Description)
                .MaximumLength(ConstConfig.LongDescriptionLength);
        }
    }
}
