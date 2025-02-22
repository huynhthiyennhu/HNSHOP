namespace HNSHOP.Dtos.Response
{
    public class CompactProductResDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ShopResDto Shop { get; set; } = null!;
        public float DiscountPercent { get; set; }
        public List<ProductImageResDto> Images { get; set; } = [];

    }
}
