using FluentValidation;
using HNSHOP.Dtos.Request;
using HNSHOP.Utils;

namespace HNSHOP.Validators
{
    public class RegisterShopValidator : AbstractValidator<RegisterShopReqDto>
    {
        public RegisterShopValidator()
        {
            CascadeMode = CascadeMode.Stop;

            // Email
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không hợp lệ");

            // Số điện thoại (VN: 0/ +84 và đầu 3/5/7/8/9 + 8 số)
            RuleFor(r => r.Phone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống")
                .Matches(@"^(?:\+84|0)(?:3|5|7|8|9)\d{8}$")
                    .WithMessage("Số điện thoại không hợp lệ");

            // Tên shop / chủ shop
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Tên không được để trống")
                .Must(v => !string.IsNullOrWhiteSpace(v))
                    .WithMessage("Tên không hợp lệ")
                .MaximumLength(ConstConfig.DisplayNameLength)
                    .WithMessage($"Tên không được dài quá {ConstConfig.DisplayNameLength} ký tự");

            // Mật khẩu
            RuleFor(r => r.Password)
                .Must(p => !string.IsNullOrWhiteSpace(p))
                    .WithMessage("Mật khẩu không được để trống")
                .MinimumLength(ConstConfig.MinPasswordLength)
                    .WithMessage($"Mật khẩu phải có ít nhất {ConstConfig.MinPasswordLength} ký tự")
                .MaximumLength(ConstConfig.MaxPasswordLength)
                    .WithMessage($"Mật khẩu không được dài quá {ConstConfig.MaxPasswordLength} ký tự");

            // Xác nhận mật khẩu
            RuleFor(r => r.ConfirmPassword)
                .Must(p => !string.IsNullOrWhiteSpace(p))
                    .WithMessage("Vui lòng nhập lại mật khẩu")
                .Equal(r => r.Password)
                    .WithMessage("Xác nhận mật khẩu không khớp");
        }
    }
}
