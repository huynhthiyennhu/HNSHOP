using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace HNSHOP.Models
{
    [Index(nameof(Path), IsUnique = true)]
    public class ProductImage
    {
        public int Id { get; set; }
        public string Path { get; set; } = string.Empty;

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;
    }
}
