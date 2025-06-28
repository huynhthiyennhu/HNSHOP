namespace HNSHOP.Dtos.Response
{
    public class CartItemResDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; } = string.Empty;
        public decimal Total => Price * Quantity;
        public int ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public float DiscountPercent { get; set; } // Bổ sung nếu chưa có

    }
}
