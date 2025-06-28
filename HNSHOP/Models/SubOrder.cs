using HNSHOP.Models;
using HNSHOP.Utils.EnumTypes;
using System.ComponentModel.DataAnnotations.Schema;

public class SubOrder
{
    public int Id { get; set; }

    [Column(TypeName = "decimal(9, 1)")]
    public decimal SubTotal { get; set; } // ⬅️ thêm dòng này

    [Column(TypeName = "decimal(9, 1)")]
    public decimal Total { get; set; }

    public SubOrderStatus Status { get; set; } = SubOrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;

    public int ShopId { get; set; }
    [ForeignKey(nameof(ShopId))]
    public Shop Shop { get; set; } = null!;

    public List<DetailOrder> DetailOrders { get; set; } = new();
}
