using HNSHOP.Models;
using HNSHOP.Utils.EnumTypes;

namespace HNSHOP.Dtos.Response
{
    public class OrderResDto
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public AddressResDto Address { get; set; } = null!;
        public Customer Customer { get; set; } = null!;

        // ❌ XÓA DetailOrders (không còn ở Order)
        // public List<DetailOrderResDto> DetailOrders { get; set; } = [];

        // ✅ Thêm SubOrders (theo shop)
        public List<SubOrderResDto> SubOrders { get; set; } = [];
    }
}
