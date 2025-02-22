namespace HNSHOP.Dtos.Request
{
    public class UpdateProductReqDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? ProductTypeId { get; set; }
    }
}
