using HNSHOP.Utils.EnumTypes;

namespace HNSHOP.Dtos.Request
{
    public class UpdateOrderShopReqDto
    {
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}
