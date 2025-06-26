using System.ComponentModel.DataAnnotations.Schema;

namespace HNSHOP.Models
{
    public class Shop
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int AccountId { get; set; }
        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; } = null!;

        public List<Product> Products { get; set; } = [];


    }
}
