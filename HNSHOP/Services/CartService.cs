using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HNSHOP.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "CartItems";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Lấy danh sách giỏ hàng từ Session
        public List<CartItemResDto> GetCartItems()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return new List<CartItemResDto>();

            var cartJson = session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson)
                ? new List<CartItemResDto>()
                : JsonConvert.DeserializeObject<List<CartItemResDto>>(cartJson);
        }


        // Thêm sản phẩm vào giỏ hàng và trả về số lượng sản phẩm
        public int AddToCart(AddToCartReqDto request, string name, decimal price, string image)
        {
            var cart = GetCartItems();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Add(new CartItemResDto
                {
                    ProductId = request.ProductId,
                    Name = name,
                    Price = price,
                    Quantity = request.Quantity,
                    Image = image
                });
            }

            SaveCart(cart);
            return GetCartItemCount(); // Trả về tổng số lượng sau khi thêm
        }

        // Cập nhật số lượng sản phẩm và trả về tổng số lượng mới
        public int UpdateCart(int productId, int quantity)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    cart.Remove(item);
                }
                SaveCart(cart);
            }

            return GetCartItemCount(); // Trả về tổng số lượng mới
        }

        // Xóa sản phẩm khỏi giỏ hàng và trả về tổng số lượng mới
        public int RemoveFromCart(int productId)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return GetCartItemCount(); // Trả về tổng số lượng mới
        }


        // Xóa toàn bộ giỏ hàng
        public void ClearCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            session?.Remove(CartSessionKey);
        }


        // Lưu giỏ hàng vào Session
        private void SaveCart(List<CartItemResDto> cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }
        // Lấy tổng số lượng sản phẩm trong giỏ hàng
        public int GetCartItemCount()
        {
            var cart = GetCartItems();
            return cart.Sum(item => item.Quantity);
        }

    }
}
