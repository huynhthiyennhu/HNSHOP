using HNSHOP.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

[PrimaryKey(nameof(SubOrderId), nameof(ProductId))]
public class DetailOrder
{
    public int SubOrderId { get; set; }
    [ForeignKey(nameof(SubOrderId))]
    public SubOrder SubOrder { get; set; } = null!;

    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; } = null!;
    [Column(TypeName = "float")]
    public float DiscountPercent { get; set; } = 0;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(9, 1)")]
    public decimal UnitPrice { get; set; }
}
