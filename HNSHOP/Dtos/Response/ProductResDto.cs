namespace HNSHOP.Dtos.Response
{
    public class ProductResDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ProductTypeId { get; set; }
        public int ShopId { get; set; }
        public float DiscountPercent { get; set; }
        public int LikeQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public float Rating { get; set; }
        public List<ProductImageResDto> Images { get; set; } = [];
    }
}
