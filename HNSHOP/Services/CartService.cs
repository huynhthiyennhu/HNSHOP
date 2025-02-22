using HNSHOP.Data;
using HNSHOP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class CartService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CartSessionKey = "CartSession"; // Khóa session cho giỏ hàng

    public CartService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Thêm sản phẩm vào giỏ hàng
    /// </summary>
    public void AddToCart(int productId, int quantity)
    {
        var product = _db.Products
                         .Include(p => p.ProductImages)
                         .FirstOrDefault(p => p.Id == productId);

        if (product == null) return;

        // Lấy giỏ hàng từ Session
        var cartItems = GetCartItems();

        var existingItem = cartItems.FirstOrDefault(c => c.ProductId == productId);
        if (existingItem != null)
        {
            // Kiểm tra nếu số lượng vượt quá tồn kho
            if (existingItem.Quantity + quantity > product.Quantity)
            {
                existingItem.Quantity = product.Quantity;
            }
            else
            {
                existingItem.Quantity += quantity;
            }
        }
        else
        {
            cartItems.Add(new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = Math.Min(quantity, product.Quantity), // Không cho vượt quá tồn kho
                Image = product.ProductImages.FirstOrDefault()?.Path ?? "default.jpg"
            });
        }

        // Cập nhật giỏ hàng vào Session
        SaveCartItems(cartItems);
    }

    /// <summary>
    /// Lấy danh sách sản phẩm trong giỏ hàng từ Session
    /// </summary>
    public List<CartItem> GetCartItems()
    {
        var session = _httpContextAccessor.HttpContext.Session;
        var cartJson = session.GetString(CartSessionKey);
        return cartJson != null ? JsonConvert.DeserializeObject<List<CartItem>>(cartJson) ?? new List<CartItem>() : new List<CartItem>();
    }

    /// <summary>
    /// Lưu danh sách sản phẩm vào giỏ hàng trong Session
    /// </summary>
    private void SaveCartItems(List<CartItem> cartItems)
    {
        var session = _httpContextAccessor.HttpContext.Session;
        session.SetString(CartSessionKey, JsonConvert.SerializeObject(cartItems));
    }

    /// <summary>
    /// Xóa một sản phẩm khỏi giỏ hàng
    /// </summary>
    public void RemoveFromCart(int productId)
    {
        var cartItems = GetCartItems();
        var item = cartItems.FirstOrDefault(c => c.ProductId == productId);
        if (item != null)
        {
            cartItems.Remove(item);
            SaveCartItems(cartItems);
        }
    }

    /// <summary>
    /// Xóa toàn bộ giỏ hàng
    /// </summary>
    public void ClearCart()
    {
        SaveCartItems(new List<CartItem>());
    }
}
