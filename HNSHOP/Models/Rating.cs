using System.ComponentModel.DataAnnotations.Schema;

namespace HNSHOP.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int? OrderId { get; set; } 


        public string UserName { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int RatingValue { get; set; }
        public DateTime CreatedAt { get; set; }

        // Quan hệ
        public Product Product { get; set; } = null!;
        public Customer Customer { get; set; } = null!;

    }
}
