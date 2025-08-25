using FluentValidation;
using HNSHOP.Dtos.Request;
using HNSHOP.Utils;

namespace HNSHOP.Validators
{
    public class VerifyEmailValidator : AbstractValidator<VerifyEmailReqDto>
    {
        public VerifyEmailValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(ve => ve.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không hợp lệ");

            RuleFor(ve => ve.Token)
                .NotEmpty().WithMessage("Mã xác thực không được để trống")
                .Must(t => !string.IsNullOrWhiteSpace(t))
                    .WithMessage("Mã xác thực không hợp lệ")
                .Length(ConstConfig.VerifyEmailTokenLength)
                    .WithMessage($"Mã xác thực phải có đúng {ConstConfig.VerifyEmailTokenLength} ký tự");
        }
    }
}
