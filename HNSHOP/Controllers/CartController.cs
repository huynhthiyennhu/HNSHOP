using HNSHOP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }
    [Authorize]
    public IActionResult Index()
    {
        var cartItems = _cartService.GetCartItems();
        ViewBag.CartCount = cartItems.Count;
        return View(cartItems);
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        _cartService.RemoveFromCart(productId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult ClearCart()
    {
        _cartService.ClearCart();
        return RedirectToAction("Index");
    }
}
