namespace HNSHOP.Dtos.Response
{
    public class SaleEventProductResDto
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Illustration { get; set; } = string.Empty;
        public float Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<CustomerTypeResDto> CustomerTypes { get; set; } = [];
        public List<ProductResDto> Products { get; set; } = [];
    }
}
