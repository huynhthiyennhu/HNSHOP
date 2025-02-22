
using HNSHOP.Utils.EnumTypes;
using System.ComponentModel.DataAnnotations.Schema;

namespace HNSHOP.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Column(TypeName = "decimal(9, 1)")]
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Processing;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public DateTime CreatedAt { get; set; } = new DateTime(2024, 1, 1);
        public DateTime UpdatedAt { get; set; } = new DateTime(2024, 1, 1);

        public int CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; } = null!;

        public int AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public Address Address { get; set; } = null!;

        public List<DetailOrder> DetailOrders { get; set; } = [];

        public List<Rating> Ratings { get; set; } = [];
    }
}
