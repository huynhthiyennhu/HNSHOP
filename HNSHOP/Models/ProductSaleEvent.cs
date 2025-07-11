using HNSHOP.Models;

public class ProductSaleEvent
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int SaleEventId { get; set; }
    public SaleEvent SaleEvent { get; set; } = null!; 
}
