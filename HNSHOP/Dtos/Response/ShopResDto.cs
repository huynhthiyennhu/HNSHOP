namespace HNSHOP.Dtos.Response
{
    public class ShopResDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ProductResDto> Products { get; set; } = new List<ProductResDto>();
    }
}
