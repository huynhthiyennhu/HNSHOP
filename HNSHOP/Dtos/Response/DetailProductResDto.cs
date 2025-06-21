namespace HNSHOP.Dtos.Response
{
    public class DetailProductResDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public ProductTypeResDto ProductType { get; set; } = null!;
        public ShopResDto Shop { get; set; } = null!;
        public float DiscountPercent { get; set; }
        public int LikeQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public float Rating { get; set; }
        public List<ProductImageResDto> Images { get; set; } = [];
        public bool CanReview { get; set; } 

    }
}
