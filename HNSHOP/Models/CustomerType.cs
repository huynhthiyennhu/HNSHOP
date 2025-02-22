namespace HNSHOP.Models
{
    public class CustomerType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<Customer> Customers { get; set; } = [];

        public List<SaleEvent> SaleEvents { get; set; } = [];
        public List<CustomerTypeSaleEvent> CustomerTypeSaleEvents { get; set; } = [];
    }
}
