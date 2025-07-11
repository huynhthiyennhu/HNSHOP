using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CartController : Controller
{
    private readonly CartService _cartService;
    private readonly ApplicationDbContext _db;

    public CartController(CartService cartService, ApplicationDbContext db)
    {
        _cartService = cartService;
        _db = db;
    }

    // Trang hiển thị giỏ hàng
    public IActionResult Index()
    {
        var cartItems = _cartService.GetCartItems();

        var grouped = cartItems
            .GroupBy(x => new { x.ShopId, x.ShopName })
            .Select(g => new ShopCartGroupDto
            {
                ShopId = g.Key.ShopId,
                ShopName = g.Key.ShopName,
                Items = g.ToList()
            }).ToList();

        decimal total = 0;
        decimal totalDiscount = 0;
        decimal finalTotal = 0;

        foreach (var item in cartItems)
        {
            var product = _db.Products.Find(item.ProductId);

            var discount = product.ProductSaleEvents
                .Where(pse => pse.SaleEvent.StartDate <= DateTime.UtcNow && pse.SaleEvent.EndDate >= DateTime.UtcNow)
                .Select(pse => pse.SaleEvent.Discount)
                .FirstOrDefault(); // mặc định là 0 nếu không có khuyến mãi

            decimal itemPrice = product.Price;
            decimal discountAmount = itemPrice * ((decimal)discount / 100);
            decimal finalPrice = itemPrice - discountAmount;

            total += itemPrice * item.Quantity;
            totalDiscount += discountAmount * item.Quantity;
            finalTotal += finalPrice * item.Quantity;
        }

        ViewBag.Total = total;
        ViewBag.TotalDiscount = totalDiscount;
        ViewBag.FinalTotal = finalTotal;

        return View(grouped);
    }




    // Thêm sản phẩm vào giỏ hàng
    [HttpPost]
    public IActionResult AddToCart([FromBody] AddToCartReqDto request)
    {
        if (request == null || request.ProductId <= 0)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var product = _db.Products.Include(p => p.ProductImages)
                                  .FirstOrDefault(p => p.Id == request.ProductId);

        if (product == null)
        {
            return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
        }

        _cartService.AddToCart(request, product.Name, product.Price, product.ProductImages.FirstOrDefault()?.Path ?? "no-image.png");

        int cartCount = _cartService.GetCartItemCount(); // Lấy lại số lượng giỏ hàng

        return Json(new { success = true, message = "Thêm vào giỏ hàng thành công!", cartCount });
    }

    // Cập nhật số lượng sản phẩm trong giỏ hàng
    [HttpPost]
    public IActionResult UpdateCart([FromBody] UpdateCartReqDto request)
    {
        _cartService.UpdateCart(request.ProductId, request.Quantity);
        int cartCount = _cartService.GetCartItemCount(); // Lấy lại số lượng giỏ hàng

        return Json(new { success = true, cartCount });
    }

    // Xóa sản phẩm khỏi giỏ hàng
    [HttpPost]
    public IActionResult RemoveFromCart([FromBody] RemoveFromCartReqDto request)
    {
        var productId = request.ProductId;
        var removed = _cartService.RemoveFromCart(productId);

        if (!removed)
        {
            return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
        }

        int cartCount = _cartService.GetCartItemCount();
        return Json(new { success = true, cartCount });
    }



    // Xóa toàn bộ giỏ hàng
    [HttpPost]
    public IActionResult ClearCart()
    {
        _cartService.ClearCart();
        return Json(new { success = true, cartCount = 0 });
    }



    [HttpGet]
    public IActionResult GetCartCount()
    {
        int count = _cartService.GetCartItemCount();
        return Json(new { count });
    }
}
