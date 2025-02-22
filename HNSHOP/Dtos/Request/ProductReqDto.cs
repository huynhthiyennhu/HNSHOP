namespace HNSHOP.Dtos.Request
{
    public class ProductReqDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ProductTypeId { get; set; }
    }
}
