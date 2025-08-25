using FluentValidation;
using HNSHOP.Dtos.Request;
using HNSHOP.Utils;
using System;
using System.Linq;

namespace HNSHOP.Validators
{
    public class CreateSaleEventValidator : AbstractValidator<CreateSaleEventReqDto>
    {
        public CreateSaleEventValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(se => se.Name)
                .NotEmpty().WithMessage("Tên chương trình không được để trống")
                .MaximumLength(ConstConfig.LongNameLength)
                .WithMessage($"Tên không được vượt quá {ConstConfig.LongNameLength} ký tự");

            RuleFor(se => se.Description)
                .NotEmpty().WithMessage("Mô tả không được để trống")
                .MaximumLength(ConstConfig.LongDescriptionLength)
                .WithMessage($"Mô tả không được vượt quá {ConstConfig.LongDescriptionLength} ký tự");

            //RuleFor(se => se.Illustration)
            //    .NotEmpty().WithMessage("Ảnh minh họa là bắt buộc");

            RuleFor(se => se.Discount)
                .NotEmpty().WithMessage("Mô tả là bắt buộc")
                .GreaterThan(0f).WithMessage("Giảm giá phải lớn hơn 0");

            RuleFor(se => se.StartDate)
                .NotEmpty().WithMessage("Ngày bắt đầu là bắt buộc")
                .Must(d => d >= DateTime.UtcNow.Date).WithMessage("Ngày bắt đầu phải từ hôm nay trở đi");

            RuleFor(se => se.EndDate)
                .NotEmpty().WithMessage("Ngày kết thúc là bắt buộc")
                .Must((se, end) => end > se.StartDate)
                    .WithMessage("Ngày kết thúc phải sau ngày bắt đầu")
                .Must(end => end <= DateTime.UtcNow.Date.AddYears(1))
                    .WithMessage("Ngày kết thúc không được quá 1 năm kể từ hôm nay");

            RuleFor(se => se.CustomerTypeIds)
                .NotEmpty().WithMessage("Phải chọn ít nhất 1 nhóm khách hàng")
                .Must(list => list.Distinct().Count() == list.Count)
                    .WithMessage("Danh sách nhóm khách hàng có phần tử trùng lặp");

            RuleForEach(se => se.CustomerTypeIds)
                .Must(id => id > 0).WithMessage("Mã nhóm khách hàng không hợp lệ");

            RuleFor(se => se.ProductIds)
                .NotEmpty().WithMessage("Phải chọn ít nhất 1 sản phẩm")
                .Must(list => list.Distinct().Count() == list.Count)
                    .WithMessage("Danh sách sản phẩm có phần tử trùng lặp");

            RuleForEach(se => se.ProductIds)
                .Must(id => id > 0).WithMessage("Mã sản phẩm không hợp lệ");

            RuleFor(se => se.ProductIds.Count)
                .LessThanOrEqualTo(2000)
                .WithMessage("Số lượng sản phẩm áp dụng không được vượt quá 2000");

        }
    }
}
