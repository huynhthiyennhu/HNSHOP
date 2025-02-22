using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using HNSHOP.Services;
using HNSHOP.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class ProductsController(ApplicationDbContext db, IMapper mapper, CartService cartService) : Controller
{
    private readonly ApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;
    private readonly CartService _cartService = cartService;


    /// <summary>
    /// Lấy danh sách tất cả sản phẩm
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var products = await _db.Products
            .Include(p => p.ProductImages)
            .ToListAsync();

        return View(_mapper.Map<List<ProductResDto>>(products));
    }

    /// <summary>
    /// Lấy chi tiết sản phẩm
    /// </summary>
    public async Task<IActionResult> Details(int id)
    {
        var product = await _db.Products
            .Include(p => p.ProductImages)
            .Include(p => p.Shop)
            .Include(p => p.ProductType)
            .Include(p => p.Ratings)
                .ThenInclude(r => r.Customer)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        // Đếm số lượt đánh giá & tính điểm trung bình
        int ratingCount = product.Ratings?.Count ?? 0;
        float averageRating = ratingCount > 0 ? (float)product.Ratings.Average(r => r.RatingValue) : 0;
        int likeCount = product.Likes.Count;
        // Lấy sản phẩm cùng loại (tối đa 4 sản phẩm)
        var relatedProducts = await _db.Products
            .Include(p => p.ProductImages)
            .Where(p => p.ProductTypeId == product.ProductTypeId && p.Id != id)
            .Take(4)
            .ToListAsync();

        // Lấy sản phẩm của shop (tối đa 4 sản phẩm)
        var shopProducts = await _db.Products
            .Include(p => p.ProductImages)
            .Where(p => p.ShopId == product.ShopId && p.Id != id)
            .Take(4)
            .ToListAsync();

        // Lấy ID của người dùng đang đăng nhập
        int userId = GetUserIdFromToken();

        // Kiểm tra xem người dùng đã mua sản phẩm chưa
        bool hasPurchased = await _db.Orders
            .Include(o => o.DetailOrders)
            .AnyAsync(o => o.CustomerId == userId && o.DetailOrders.Any(d => d.ProductId == id));

        var productDto = _mapper.Map<DetailProductResDto>(product);
        var relatedProductsDto = _mapper.Map<List<ProductResDto>>(relatedProducts);
        var shopProductsDto = _mapper.Map<List<ProductResDto>>(shopProducts);
        var shopDto = _mapper.Map<ShopResDto>(product.Shop);

        // Gửi dữ liệu đến View
        ViewBag.RelatedProducts = relatedProductsDto;
        ViewBag.ShopProducts = shopProductsDto;
        ViewBag.Shop = shopDto;
        ViewBag.Ratings = product.Ratings ?? new List<Rating>(); // Fix lỗi null
        ViewBag.RatingCount = ratingCount;
        ViewBag.AverageRating = averageRating;
        ViewBag.HasPurchased = hasPurchased;
        ViewBag.CustomerId = userId;
        ViewBag.LikeCount = likeCount;

        return View(productDto);
    }

    /// <summary>
    /// Người dùng gửi đánh giá sản phẩm (chỉ cho khách đã mua hàng)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SubmitReview(RatingReqDto reviewReq)
    {
        int userId = GetUserIdFromToken();

        if (!ModelState.IsValid)
            return RedirectToAction("Details", new { id = reviewReq.ProductId });

        // Kiểm tra xem người dùng đã mua sản phẩm chưa
        var order = await _db.Orders
            .Include(o => o.DetailOrders)
            .FirstOrDefaultAsync(o => o.CustomerId == userId && o.DetailOrders.Any(d => d.ProductId == reviewReq.ProductId));

        if (order == null)
        {
            TempData["ErrorMessage"] = "Bạn chỉ có thể đánh giá sản phẩm khi đã mua.";
            return RedirectToAction("Details", new { id = reviewReq.ProductId });
        }

        var newReview = new Rating
        {
            ProductId = reviewReq.ProductId,
            CustomerId = userId,
            OrderId = order.Id,
            UserName = User.Identity?.Name ?? "Người dùng ẩn danh",
            Comment = reviewReq.Comment,
            RatingValue = reviewReq.Rating,
            CreatedAt = DateTime.Now
        };

        _db.Ratings.Add(newReview);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", new { id = reviewReq.ProductId });
    }

    /// <summary>
    /// Lấy ID người dùng từ Token
    /// </summary>
    private int GetUserIdFromToken()
    {
        return int.TryParse(User.FindFirst(ConstConfig.UserIdClaimType)?.Value, out int userId) ? userId : -1;
    }

    /// <summary>
    /// Form thêm sản phẩm (Chỉ shop mới có quyền)
    /// </summary>
    [Authorize(Roles = ConstConfig.ShopRoleName)]
    public IActionResult Create() => View();

    /// <summary>
    /// Xử lý thêm sản phẩm
    /// </summary>
    [Authorize(Roles = ConstConfig.ShopRoleName)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductReqDto productReq)
    {
        var product = _mapper.Map<Product>(productReq);
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Form chỉnh sửa sản phẩm (Chỉ shop mới có quyền)
    /// </summary>
    [Authorize(Roles = ConstConfig.ShopRoleName)]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        return View(_mapper.Map<UpdateProductReqDto>(product));
    }

    /// <summary>
    /// Xử lý cập nhật sản phẩm
    /// </summary>
    [Authorize(Roles = ConstConfig.ShopRoleName)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateProductReqDto updateProductReq)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        _mapper.Map(updateProductReq, product);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Xóa sản phẩm (Chỉ shop mới có quyền)
    /// </summary>
    [Authorize(Roles = ConstConfig.ShopRoleName)]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult AddToCart(int productId, int quantity)
    {
        _cartService.AddToCart(productId, quantity);
        return RedirectToAction("Cart", "Cart");
    }



}
