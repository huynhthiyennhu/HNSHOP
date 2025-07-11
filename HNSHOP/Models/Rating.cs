using System.ComponentModel.DataAnnotations.Schema;

namespace HNSHOP.Models
{
    public class Rating
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public int CustomerId { get; set; }

        public int SubOrderId { get; set; }  // ➤ Đánh giá theo SubOrder (Shop)

        public string UserName { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public int RatingValue { get; set; }  // ➤ Giá trị từ 1 đến 5

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Product Product { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public SubOrder SubOrder { get; set; } = null!;

    }
}
