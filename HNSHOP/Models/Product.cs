using System.ComponentModel.DataAnnotations.Schema;

namespace HNSHOP.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(9, 1)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public int ProductTypeId { get; set; }
        [ForeignKey(nameof(ProductTypeId))]
        public ProductType ProductType { get; set; } = null!;

        public int ShopId { get; set; }
        [ForeignKey(nameof(ShopId))]
        public Shop Shop { get; set; } = null!;

        public List<SaleEvent> SaleEvents { get; set; } = [];
        public List<ProductSaleEvent> ProductSaleEvents { get; set; } = [];

        public List<Like> Likes { get; set; } = [];

        public List<DetailOrder> DetailOrders { get; set; } = [];

        public List<ProductImage> ProductImages { get; set; } = [];

        public List<Rating> Ratings { get; set; } = [];
    }
}
