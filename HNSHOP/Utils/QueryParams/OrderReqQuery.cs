using HNSHOP.Utils.EnumTypes;
using Swashbuckle.AspNetCore.Annotations;

namespace HNSHOP.Utils.QueryParams
{
    public class OrderReqQuery
    {
        private int _page = 1;
        private string _sortBy = "id";
        private string _sortType = "asc";

        [SwaggerParameter(Description = "Trạng thái của đơn hàng: { Processing: Đang xử lý, " +
            "Shipping: Đang vận chuyển, " +
            "Delivered: Đã giao, " +
            "Cancelled: Đã hủy }")]
        public OrderStatus? Status { get; set; }
        [SwaggerParameter(Description = "Trạng thái thanh toán của đơn hàng: { Pending: Đang chờ thanh toán, " +
            "Completed: Đã thanh toán thành công }")]
        public PaymentStatus? PaymentStatus { get; set; }

        public int Page
        {
            get => _page;
            set => _page = value >= 1 ? value : _page;
        }

        [SwaggerParameter(Description = "SortBy: attributeName (vd: id) " +
           "attributeName{id, date} " +
           "date: ngày đặt")]
        public string SortBy
        {
            get => _sortBy;
            set => _sortBy = string.IsNullOrEmpty(value) ? _sortBy : value.ToLower();
        }

        [SwaggerParameter(Description = "SortType: type (vd: asc) " +
            "type{asc, desc} " +
            "asc: sắp xếp tăng dần " +
            "desc: sắp xếp giảm dần")]
        public string SortType
        {
            get => _sortType;
            set => _sortType = string.IsNullOrEmpty(value) ? _sortType : value.ToLower();
        }
    }
}
