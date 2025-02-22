using System.ComponentModel.DataAnnotations.Schema;

namespace HNSHOP.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string AddressDetail { get; set; } = string.Empty;

        public int CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; } = null!;

        public List<Order> Orders { get; set; } = [];
    }
}
