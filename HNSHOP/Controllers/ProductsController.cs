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
using System.Text.Json.Serialization;
using System.Text.Json;
using HNSHOP.Utils.EnumTypes;
using System.Globalization;

public class ProductsController(ApplicationDbContext db, IMapper mapper, CartService cartService, IWebHostEnvironment env) : Controller
{
    private readonly ApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;
    private readonly CartService _cartService = cartService;
    private readonly IWebHostEnvironment _env=env;


   
    public async Task<IActionResult> Index()
    {
        var products = await _db.Products.Where(p => (!p.IsDeleted))
            .Include(p => p.ProductImages)
            .ToListAsync();

        return View(_mapper.Map<List<ProductResDto>>(products));
    }



    public async Task<IActionResult> Details(int id)
    {
        var product = await _db.Products.Where(p => (!p.IsDeleted))
            .Include(p => p.ProductImages)
            .Include(p => p.Shop)
            .Include(p => p.ProductType)
            .Include(p => p.Ratings)
                .ThenInclude(r => r.Customer)
            .Include(p => p.Likes)
            .Include(p => p.DetailOrders)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();

        int ratingCount = product.Ratings?.Count ?? 0;
        float averageRating = ratingCount > 0 ? (float)product.Ratings.Average(r => r.RatingValue) : 0;
        int likeCount = product.Likes.Count;

        int soldQuantity = product.DetailOrders.Sum(d => d.Quantity);

        var relatedProducts = await _db.Products.Where(p => (!p.IsDeleted))
            .Include(p => p.ProductImages)
            .Where(p => p.ProductTypeId == product.ProductTypeId && p.Id != id)
            .Take(4)
            .ToListAsync();

        var shopProducts = await _db.Products.Where(p => (!p.IsDeleted))
            .Include(p => p.ProductImages)
            .Where(p => p.ShopId == product.ShopId && p.Id != id)
            .Take(6)
            .ToListAsync();


        var productDto = _mapper.Map<DetailProductResDto>(product);
        var relatedProductsDto = _mapper.Map<List<ProductResDto>>(relatedProducts);
        var shopProductsDto = _mapper.Map<List<ProductResDto>>(shopProducts);
        var shopDto = _mapper.Map<ShopResDto>(product.Shop);
        // Tính trung bình đánh giá toàn bộ sản phẩm của shop
        var shopRatingData = await _db.Ratings
    .Include(r => r.Product) // quan trọng!
    .Where(r => r.Product.ShopId == product.ShopId && !r.Product.IsDeleted)
    .GroupBy(r => r.Product.ShopId)
    .Select(g => new
    {
        AverageRating = (float)g.Average(r => r.RatingValue),
        RatingCount = g.Count()
    })
    .FirstOrDefaultAsync();


        float shopAverageRating = shopRatingData != null ? (float)shopRatingData.AverageRating : 0f;
        int shopRatingCount = shopRatingData?.RatingCount ?? 0;
        productDto.SoldQuantity = soldQuantity;
        ViewBag.ShopAverageRating = shopRatingData?.AverageRating ?? 0f;
        ViewBag.ShopRatingCount = shopRatingData?.RatingCount ?? 0;
        ViewBag.RelatedProducts = relatedProductsDto;
        ViewBag.ShopProducts = shopProductsDto;
        ViewBag.Shop = shopDto;
        ViewBag.Ratings = product.Ratings?.ToList() ?? new List<Rating>();
        ViewBag.RatingCount = ratingCount;
        ViewBag.AverageRating = averageRating;
        ViewBag.LikeCount = likeCount;

        return View("Details", productDto);
    }

  
    private int GetUserIdFromToken()
    {
        return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : -1;
    }
    private int GetCustomerIdFromToken()
    {
        int userId = GetUserIdFromToken();
        var customer = _db.Customers.FirstOrDefault(c => c.AccountId == userId);
        return customer?.Id ?? -1;
    }


    [Authorize(Roles = ConstConfig.ShopRoleName)]
    [HttpGet]
    public async Task<IActionResult> Manage()
    {
        int userId = GetUserIdFromToken();

        var shop = await _db.Shops
            .Where(s => s.AccountId == userId)
            .Select(s => new
            {
                Shop = s,
                Products = s.Products
                    .Where(p => !p.IsDeleted)
                    .Select(p => new
                    {
                        Product = p,
                        Images = p.ProductImages
                    }).ToList()
            })
            .FirstOrDefaultAsync();

        if (shop == null)
            return NotFound();

        foreach (var item in shop.Products)
        {
            item.Product.ProductImages = item.Images.ToList();
        }

        ViewBag.ProductTypes = await _db.ProductTypes.ToListAsync();

        var products = _mapper.Map<List<ProductResDto>>(shop.Products.Select(x => x.Product).ToList());

        return View("ManageProducts", products);
    }


    [Authorize(Roles = ConstConfig.ShopRoleName)]
    [HttpGet]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _db.Products.Where(p => (!p.IsDeleted))
            .Include(p => p.ProductImages)
            .Include(p => p.ProductType)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return NotFound();

        var productDto = new ProductResDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity,
            Description = product.Description,
            ProductTypeId = product.ProductTypeId,
            Images = product.ProductImages.Select(i => new ProductImageResDto
            {
                Id = i.Id, // Bổ sung ID ảnh
                Path = i.Path
            }).ToList()
        };

        var productTypes = await _db.ProductTypes.Select(pt => new
        {
            pt.Id,
            pt.Name
        }).ToListAsync();

        return Json(new { product = productDto, productTypes });
    }



    [Authorize(Roles = ConstConfig.ShopRoleName)]
    [HttpPost]
    public async Task<IActionResult> Create(ProductReqDto productReq, List<IFormFile> Images)
    {
        int userId = GetUserIdFromToken();
        var shop = await _db.Shops.FirstOrDefaultAsync(s => s.AccountId == userId);

        if (shop == null)
            return Json(new { success = false, message = "Shop không tồn tại." });

        var product = new Product
        {
            Name = productReq.Name,
            Description = productReq.Description,
            Price = productReq.Price,
            Quantity = productReq.Quantity,
            ProductTypeId = productReq.ProductTypeId,
            ShopId = shop.Id
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        await SaveProductImages(Images, product.Id);
        return Json(new { success = true, message = "Thêm sản phẩm thành công!" });
    }

    [Authorize(Roles = ConstConfig.ShopRoleName)]
    [HttpPost]
    public async Task<IActionResult> Edit(int id, ProductReqDto productReq, List<IFormFile> Images)
    {
        try
        {
            int userId = GetUserIdFromToken();

            var product = await _db.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id && p.Shop.AccountId == userId);

            if (product == null) return NotFound();

            // Cập nhật thông tin sản phẩm
            product.Name = productReq.Name;
            product.Description = productReq.Description;
            product.Price = productReq.Price;
            //product.Quantity = productReq.Quantity;

            // Kiểm tra nếu người dùng chọn loại sản phẩm mới
            if (productReq.ProductTypeId != 0 && productReq.ProductTypeId != product.ProductTypeId)
            {
                // Kiểm tra loại sản phẩm mới có tồn tại không
                bool isValidType = await _db.ProductTypes.AnyAsync(pt => pt.Id == productReq.ProductTypeId);
                if (!isValidType)
                {
                    return Json(new { success = false, message = "Loại sản phẩm không tồn tại!" });
                }

                product.ProductTypeId = productReq.ProductTypeId;
            }

            await _db.SaveChangesAsync();

            // Xử lý ảnh mới nếu có
            if (Images != null && Images.Count > 0)
            {
                await SaveProductImages(Images, product.Id);
            }

            return Json(new { success = true, message = "Sản phẩm đã được cập nhật!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi cập nhật sản phẩm: {ex.Message}");
            return Json(new { success = false, message = $"Lỗi khi cập nhật sản phẩm: {ex.Message}" });
        }
    }

    [Authorize(Roles = ConstConfig.ShopRoleName)]
    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int id, int addQuantity)
    {
        if (addQuantity <= 0)
        {
            return Json(new { success = false, message = "Số lượng phải lớn hơn 0!" });
        }

        var userId = GetUserIdFromToken();
        var product = await _db.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.Shop.AccountId == userId);

        if (product == null)
        {
            return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
        }

        product.Quantity += addQuantity;
        await _db.SaveChangesAsync();

        return Json(new { success = true, message = "Đã cập nhật số lượng sản phẩm!", newQuantity = product.Quantity });
    }

    [Authorize(Roles = ConstConfig.ShopRoleName)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        int userId = GetUserIdFromToken();
        var product = await _db.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == id && p.Shop.AccountId == userId);

        if (product == null)
            return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc không thuộc quyền sở hữu." });

        // XÓA MỀM: Chỉ cập nhật IsDeleted = true, không xóa ảnh, không xóa cứng product
        product.IsDeleted = true;
        await _db.SaveChangesAsync();

        return Json(new { success = true, message = "Sản phẩm đã được ẩn khỏi hệ thống!" });
    }

    private async Task SaveProductImages(List<IFormFile> Images, int productId)
    {
        string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "hnshop");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        foreach (var image in Images)
        {
            if (image.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}_{image.FileName}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                _db.ProductImages.Add(new ProductImage
                {
                    Path = fileName,
                    ProductId = productId
                });
            }
        }

        await _db.SaveChangesAsync();
    }

    [Authorize(Roles = ConstConfig.ShopRoleName)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        var image = await _db.ProductImages.FindAsync(imageId);
        if (image == null)
            return Json(new { success = false, message = "Ảnh không tồn tại." });

        // Xóa ảnh khỏi thư mục
        var filePath = Path.Combine(_env.WebRootPath, "images", "hnshop", image.Path);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        _db.ProductImages.Remove(image);
        await _db.SaveChangesAsync();

        return Json(new { success = true, message = "Ảnh đã được xóa thành công." });
    }


    [HttpPost]
    public IActionResult AddToCart(AddToCartReqDto request)
    {
        var product = _db.Products.Include(p => p.ProductImages).FirstOrDefault(p => p.Id == request.ProductId);
        if (product == null) return NotFound();

        _cartService.AddToCart(request, product.Name, product.Price, product.ProductImages.FirstOrDefault()?.Path ?? "no-image.png");

        return RedirectToAction("Cart", "Cart");
    }

    private async Task<string> SaveImageAsync(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            throw new ArgumentException("File ảnh không hợp lệ.");
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/hnshop");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        return $"/images/hnshop/{uniqueFileName}"; 
    }


    [HttpPost]
    public async Task<IActionResult> ToggleLike(int productId)
    {
        int userId = GetUserIdFromToken(); 
        var customer = await _db.Customers.FirstOrDefaultAsync(c => c.AccountId == userId);

        if (customer == null)
        {
            return Json(new { success = false, message = "Bạn cần có tài khoản khách hàng để thích sản phẩm." });
        }

        var like = await _db.Likes.FirstOrDefaultAsync(l => l.CustomerId == customer.Id && l.ProductId == productId);

        if (like == null)
        {
            // Thêm sản phẩm vào danh sách yêu thích
            var newLike = new Like
            {
                CustomerId = customer.Id,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Likes.Add(newLike);
            await _db.SaveChangesAsync();

            return Json(new { success = true, liked = true });
        }
        else
        {
            // Xóa sản phẩm khỏi danh sách yêu thích
            _db.Likes.Remove(like);
            await _db.SaveChangesAsync();

            return Json(new { success = true, liked = false });
        }
    }



    [Authorize]
    public async Task<IActionResult> Favorites()
    {
        int userId = GetUserIdFromToken();

        var customer = await _db.Customers
            .Include(c => c.Likes)
            .ThenInclude(l => l.Product)
            .ThenInclude(p => p.ProductImages)
            .FirstOrDefaultAsync(c => c.AccountId == userId);

        if (customer == null) return Unauthorized();

        var favoriteProducts = customer.Likes.Select(l => new ProductResDto
        {
            Id = l.Product.Id,
            Name = l.Product.Name,
            Price = l.Product.Price,
            Images = l.Product.ProductImages.Select(img => new ProductImageResDto
            {
                Id = img.Id,
                Path = img.Path
            }).ToList()
        }).ToList();

        return View(favoriteProducts);
    }

   
}
