using FluentValidation;
using HNSHOP.Dtos.Request;
using HNSHOP.Utils;

public class RegisterValidator : AbstractValidator<RegisterReqDto>
{
    public RegisterValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không hợp lệ");

        RuleFor(r => r.Phone)
            .NotEmpty().WithMessage("Số điện thoại không được để trống")
            .Matches(@"^(?:\+84|0)(?:3|5|7|8|9)\d{8}$").WithMessage("Số điện thoại không hợp lệ");

        RuleFor(r => r.Name)
            .NotEmpty().WithMessage("Tên không được để trống")
            .MaximumLength(ConstConfig.DisplayNameLength)
            .WithMessage($"Tên không được dài quá {ConstConfig.DisplayNameLength} ký tự");

        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(ConstConfig.MinPasswordLength)
            .WithMessage($"Mật khẩu phải có ít nhất {ConstConfig.MinPasswordLength} ký tự")
            .MaximumLength(ConstConfig.MaxPasswordLength)
            .WithMessage($"Mật khẩu không được dài quá {ConstConfig.MaxPasswordLength} ký tự");

        RuleFor(r => r.ConfirmPassword)
            .NotEmpty().WithMessage("Vui lòng nhập lại mật khẩu")
            .Equal(r => r.Password).WithMessage("Xác nhận mật khẩu không khớp");
    }
}
