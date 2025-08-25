using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace HNSHOP.Services
{
    public class CartItemSessionDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _db;
        private const string CartSessionKey = "CartItems";

        public CartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext db)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        // ✅ Lấy dữ liệu session (dạng rút gọn)
        private List<CartItemSessionDto> GetCartSession()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var json = session?.GetString(CartSessionKey);
            return string.IsNullOrEmpty(json)
                ? new List<CartItemSessionDto>()
                : JsonConvert.DeserializeObject<List<CartItemSessionDto>>(json)!;
        }

        // ✅ Ghi session
        private void SaveCartSession(List<CartItemSessionDto> cart)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var json = JsonConvert.SerializeObject(cart);
            session?.SetString(CartSessionKey, json);
        }

        // ✅ Lấy giỏ hàng đầy đủ (dùng cho View)
        public List<CartItemResDto> GetCartItems()
        {
            var cart = GetCartSession();
            if (!cart.Any()) return new();

            var productIds = cart.Select(x => x.ProductId).ToList();
            var products = _db.Products
                              .Where(p => productIds.Contains(p.Id) && !p.IsDeleted)
                              .Include(p => p.Shop)
                              .Include(p => p.ProductImages)
                              .Include(p => p.ProductSaleEvents) // Thêm Include
                                .ThenInclude(pse => pse.SaleEvent)
                              .ToDictionary(p => p.Id);

            DateTime now = DateTime.UtcNow;

            return cart.Select(item =>
            {
                var product = products[item.ProductId];

                // Lấy discount % đang áp dụng (nếu có)
                var discountPercent = product.ProductSaleEvents
        .Where(pse => pse.SaleEvent.StartDate <= DateTime.UtcNow && pse.SaleEvent.EndDate >= DateTime.UtcNow)
        .Select(pse => pse.SaleEvent.Discount)
        .DefaultIfEmpty(0)
        .Max();

                return new CartItemResDto
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    Image = product.ProductImages.FirstOrDefault()?.Path ?? "no-image.png",
                    ShopId = product.ShopId,
                    ShopName = product.Shop.Name,
                    DiscountPercent = discountPercent,
                    StockQuantity = product.Quantity
                };
            }).ToList();
        }


        // ✅ Thêm sản phẩm vào giỏ
        public int AddToCart(AddToCartReqDto request, string name, decimal price, string image)
        {
            var cart = GetCartSession();
            var existing = cart.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (existing != null)
            {
                existing.Quantity += request.Quantity;
            }
            else
            {
                cart.Add(new CartItemSessionDto
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                });
            }

            SaveCartSession(cart);
            return GetCartItemCount();
        }

        // ✅ Cập nhật số lượng
        public int UpdateCart(int productId, int quantity)
        {
            var cart = GetCartSession();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                var product = _db.Products.Find(productId); // 🔥 Lấy tồn kho thật
                if (product == null)
                    return GetCartItemCount(); // hoặc throw nếu cần

                if (quantity > product.Quantity)
                {
                    quantity = product.Quantity; // Giới hạn về tồn kho
                }

                if (quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    cart.Remove(item);
                }

                SaveCartSession(cart);
            }

            return GetCartItemCount();
        }


        // ✅ Xóa sản phẩm
        public bool RemoveFromCart(int productId)
        {
            var cart = GetCartSession();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCartSession(cart);
                return true;
            }

            return false;
        }


        // ✅ Xóa toàn bộ giỏ
        public void ClearCart()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(CartSessionKey);
        }

        // ✅ Tổng số lượng
        public int GetCartItemCount()
        {
            return GetCartSession().Sum(item => item.Quantity);
        }
    }
}
