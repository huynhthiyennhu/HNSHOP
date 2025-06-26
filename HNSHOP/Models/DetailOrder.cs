using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HNSHOP.Models
{
    [PrimaryKey(nameof(OrderId), nameof(ProductId))]
    public class DetailOrder
    {
        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        [Column(TypeName = "decimal(9, 1)")]
        public decimal UnitPrice { get; set; }

    }
}