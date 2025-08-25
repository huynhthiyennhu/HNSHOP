using FluentValidation;
using HNSHOP.Dtos.Request;
using HNSHOP.Utils;
using System.Linq;

namespace HNSHOP.Validators
{
    public class UpdateSaleEventValidator : AbstractValidator<UpdateSaleEventReqDto>
    {
        public UpdateSaleEventValidator()
        {
            CascadeMode = CascadeMode.Stop;

            // Name (optional)
            RuleFor(se => se.Name)
                .Must(v => v == null || v.Length > 0).WithMessage("Tên không hợp lệ")
                .When(se => se.Name != null)
                .MaximumLength(ConstConfig.LongNameLength)
                    .WithMessage($"Tên không được vượt quá {ConstConfig.LongNameLength} ký tự")
                .When(se => se.Name != null);

            // Description (optional)
            RuleFor(se => se.Description)
                .Must(v => v == null || v.Length > 0).WithMessage("Mô tả không hợp lệ")
                .When(se => se.Description != null)
                .MaximumLength(ConstConfig.LongDescriptionLength)
                    .WithMessage($"Mô tả không được vượt quá {ConstConfig.LongDescriptionLength} ký tự")
                .When(se => se.Description != null);

            // Illustration path (optional)
            RuleFor(se => se.Illustration)
                .Must(v => v == null || v.Length > 0).WithMessage("Đường dẫn ảnh không hợp lệ")
                .When(se => se.Illustration != null)
                .MaximumLength(ConstConfig.ImagePathLength)
                    .WithMessage($"Đường dẫn ảnh tối đa {ConstConfig.ImagePathLength} ký tự")
                .When(se => se.Illustration != null);

            RuleFor(se => se.Discount)
                .InclusiveBetween(0f, 100f).WithMessage("Giảm giá phải từ 0 đến 100%")
                .When(se => se.Discount.HasValue);

            RuleFor(se => se.EndDate)
                .GreaterThan(se => se.StartDate!.Value)
                .WithMessage("Ngày kết thúc phải sau ngày bắt đầu")
                .When(se => se.StartDate.HasValue && se.EndDate.HasValue);

            RuleFor(se => se.CustomerTypeIds)
                .Must(list => list == null || (list.Count == list.Distinct().Count()))
                .WithMessage("Danh sách nhóm khách hàng có phần tử trùng lặp");

            RuleForEach(se => se.CustomerTypeIds)
                .Must(id => id > 0).WithMessage("Mã nhóm khách hàng không hợp lệ")
                .When(se => se.CustomerTypeIds != null);

            RuleFor(se => se.ProductIds)
                .Must(list => list == null || (list.Count == list.Distinct().Count()))
                .WithMessage("Danh sách sản phẩm có phần tử trùng lặp");

            RuleForEach(se => se.ProductIds)
                .Must(id => id > 0).WithMessage("Mã sản phẩm không hợp lệ")
                .When(se => se.ProductIds != null);
        }
    }
}
