namespace HNSHOP.Models
{
    public class SaleEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Illustration { get; set; } = string.Empty;
        public float Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<CustomerType> CustomerTypes { get; set; } = [];
        public List<CustomerTypeSaleEvent> CustomerTypeSaleEvents { get; set; } = [];

        public List<Product> Products { get; set; } = [];
        public List<ProductSaleEvent> ProductSaleEvents { get; set; } = [];
    }
}
