using HNSHOP.Models;
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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; } = null!;

        public int AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public Address Address { get; set; } = null!;

        public List<SubOrder> SubOrders { get; set; } = [];
    }
}