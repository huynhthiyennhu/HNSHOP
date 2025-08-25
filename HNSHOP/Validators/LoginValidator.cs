using FluentValidation;
using HNSHOP.Dtos.Request;
using HNSHOP.Utils;

namespace HNSHOP.Validators
{
    public class LoginValidator : AbstractValidator<LoginReqDto>
    {
        public LoginValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(l => l.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không hợp lệ");

            RuleFor(l => l.Password)
                .Must(p => !string.IsNullOrWhiteSpace(p))
                    .WithMessage("Mật khẩu không được để trống")
                .MinimumLength(ConstConfig.MinPasswordLength)
                    .WithMessage($"Mật khẩu phải có ít nhất {ConstConfig.MinPasswordLength} ký tự")
                .MaximumLength(ConstConfig.MaxPasswordLength)
                    .WithMessage($"Mật khẩu không được dài quá {ConstConfig.MaxPasswordLength} ký tự");
        }
    }
}
