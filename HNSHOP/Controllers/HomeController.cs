using HNSHOP.Data;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using HNSHOP.Utils;
using System.Security.Claims;

namespace HNSHOP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public HomeController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        private int GetUserIdFromToken()
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : -1;
        }
        public async Task<IActionResult> Index(
     int? productTypeId,
     string? search,
     string? sortBy,
     string? sortType,
     int page = 1,
     int pageSize = 6)
        {
            int userId = GetUserIdFromToken();

            // Lấy danh sách sản phẩm đã thích của người dùng
            var likedProducts = new HashSet<int>();
            if (userId > 0)
            {
                var customer = await _db.Customers.FirstOrDefaultAsync(c => c.AccountId == userId);
                if (customer != null)
                {
                    likedProducts = await _db.Likes
                        .Where(l => l.CustomerId == customer.Id)
                        .Select(l => l.ProductId)
                        .ToHashSetAsync();
                }
            }

            // Truy vấn danh sách sản phẩm
            var query = _db.Products
                .AsNoTracking()
                .Include(p => p.ProductImages)
                .Include(p => p.Shop)
                .Include(p => p.Likes)
                .Include(p => p.Ratings)
                .AsQueryable();

            // 1. Lọc theo loại sản phẩm
            if (productTypeId.HasValue && productTypeId > 0)
            {
                query = query.Where(p => p.ProductTypeId == productTypeId);
            }

            // 2. Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                string searchLower = search.ToLowerInvariant();
                query = query.Where(p => p.Name.ToLower().Contains(searchLower) ||
                                         p.Description.ToLower().Contains(searchLower));
            }

            // 3. Sắp xếp sản phẩm theo tiêu chí
            query = (sortBy?.ToLower(), sortType?.ToLower()) switch
            {
                ("price", "asc") => query.OrderBy(p => p.Price),
                ("price", "desc") => query.OrderByDescending(p => p.Price),
                ("name", "asc") => query.OrderBy(p => p.Name),
                ("name", "desc") => query.OrderByDescending(p => p.Name),
                ("likes", _) => query.OrderByDescending(p => p.Likes.Count),
                ("rating", _) => query.OrderByDescending(p => p.Ratings.Any() ? p.Ratings.Average(r => r.RatingValue) : 0),
                _ => query.OrderByDescending(p => p.Likes.Count)
            };

            // 4. Phân trang
            int totalProducts = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // 5. Lấy danh sách loại sản phẩm
            var productTypes = await _db.ProductTypes.AsNoTracking().ToListAsync();

            // 6. Chuyển đổi dữ liệu sang DTO và đánh dấu sản phẩm đã yêu thích
            var productDtos = products.Select(p => new ProductResDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Images = p.ProductImages.Select(img => new ProductImageResDto { Id = img.Id, Path = img.Path }).ToList(),
                IsLiked = likedProducts.Contains(p.Id) // Đánh dấu nếu sản phẩm đã được yêu thích
            }).ToList();

            var productTypeDtos = _mapper.Map<List<ProductTypeResDto>>(productTypes);

            // 7. Truyền dữ liệu qua ViewBag
            ViewBag.ProductTypes = productTypeDtos;
            ViewBag.ProductTypeId = productTypeId;
            ViewBag.Search = search;
            ViewBag.SortBy = sortBy;
            ViewBag.SortType = sortType;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.LikedProducts = likedProducts; // Truyền danh sách yêu thích vào ViewBag để sử dụng trong View

            var currentDate = DateTime.UtcNow;

            // 8. Lấy danh sách chương trình giảm giá đang diễn ra hoặc sắp diễn ra
            var saleEvents = await _db.SaleEvents
                .Where(se => se.EndDate >= currentDate)
                .OrderBy(se => se.StartDate)
                .ToListAsync();

            ViewBag.SaleEvents = saleEvents;

            return View(productDtos);
        }

    }

}
