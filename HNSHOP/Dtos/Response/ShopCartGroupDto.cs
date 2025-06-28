namespace HNSHOP.Dtos.Response
{
    public class ShopCartGroupDto
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;

        public List<CartItemResDto> Items { get; set; } = new();

        public decimal SubTotal => Items.Sum(i => i.Price * i.Quantity);
        
        public decimal Discount => Items.Sum(i => (i.Price * i.Quantity) * (decimal)i.DiscountPercent / 100);
        
        public decimal FinalTotal => SubTotal - Discount;
    }
}
