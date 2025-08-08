using HNSHOP.Data;
using HNSHOP.Models;
using HNSHOP.Dtos.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;

namespace HNSHOP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(
       int? productTypeId,
       string? search,
       decimal? minPrice,
       decimal? maxPrice,
       int? shopId,
       string? sortBy,
       string? sortType,
       int page = 1,
       int pageSize = 6)
        {
            // Bước 1: Query filter thô và select các trường cần thiết + LikeCount, AvgRating
            var query = _db.Products
                .AsNoTracking()
                .Where(p => (!p.IsDeleted)
                &&(!productTypeId.HasValue || p.ProductTypeId == productTypeId )
                         && (string.IsNullOrWhiteSpace(search) ||
                              p.Name.ToLower().Contains(search.ToLower()) ||
                              p.Description.ToLower().Contains(search.ToLower()))
                         && (!minPrice.HasValue || p.Price >= minPrice)
                         && (!maxPrice.HasValue || p.Price <= maxPrice)
                         && (!shopId.HasValue || p.ShopId == shopId))
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Quantity,
                    p.Description,
                    Images = p.ProductImages.Select(img => new ProductImageResDto
                    {
                        Id = img.Id,
                        Path = img.Path
                    }).ToList(),
                    LikeCount = p.Likes.Count(),
                    AvgRating = p.Ratings.Any() ? p.Ratings.Average(r => (double?)r.RatingValue) ?? 0 : 0
                });

            // Bước 2: Sắp xếp
            sortBy = (sortBy ?? "id").ToLowerInvariant();
            sortType = (sortType ?? "desc").ToLowerInvariant();
            switch (sortBy)
            {
                case "price":
                    query = (sortType == "asc") ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                    break;
                case "name":
                    query = (sortType == "asc") ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                    break;
                case "likes":
                    query = (sortType == "asc") ? query.OrderBy(p => p.LikeCount) : query.OrderByDescending(p => p.LikeCount);
                    break;
                case "rating":
                    query = (sortType == "asc") ? query.OrderBy(p => p.AvgRating) : query.OrderByDescending(p => p.AvgRating);
                    break;
                default:
                    query = query.OrderByDescending(p => p.Id);
                    break;
            }

            // Bước 3: Phân trang
            int totalProducts = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page > totalPages) page = totalPages;
            if (page < 1) page = 1;

            var productList = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Bước 4: Xác định các sản phẩm đã yêu thích (nếu đăng nhập)
            var likedProductIds = new HashSet<int>();
            int userId = GetUserId();
            if (userId > 0)
            {
                var customer = await _db.Customers.FirstOrDefaultAsync(c => c.AccountId == userId);
                if (customer != null)
                {
                    likedProductIds = _db.Likes
                        .Where(l => l.CustomerId == customer.Id)
                        .Select(l => l.ProductId)
                        .ToHashSet();
                }
            }

            // Bước 5: Map sang ProductResDto (và check isLiked)
            var dtos = productList.Select(p => new ProductResDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Images = p.Images,
                Quantity=p.Quantity,
                IsLiked = likedProductIds.Contains(p.Id)
            }).ToList();

            // Bước 6: Truyền ViewBag cho filter, sort, phân trang, gợi ý...
            ViewBag.ProductTypes = await _db.ProductTypes.AsNoTracking().ToListAsync();
            ViewBag.ShopList = await _db.Shops.AsNoTracking().ToListAsync();
            ViewBag.ProductTypeId = productTypeId;
            ViewBag.Search = search;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.ShopId = shopId;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.SortBy = sortBy;
            ViewBag.SortType = sortType;

            ViewBag.SaleEvents = await _db.SaleEvents
                .Where(e => e.EndDate >= DateTime.Now)
                .OrderByDescending(e => e.StartDate)
                .Take(5)
                .ToListAsync();

            // Bước 7: AJAX hoặc Full View
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_ProductListPartial", dtos);

            ViewBag.SuggestedProducts = await GetSuggestedProductsAsync();

            return View(dtos);
        }



        // Hàm trả về danh sách sản phẩm gợi ý dựa trên Content-Based Filtering (TF-IDF)
        private async Task<List<ProductResDto>> GetSuggestedProductsAsync()
        {
            var userId = GetUserId();
            if (userId <= 0) return new List<ProductResDto>();

            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.AccountId == userId);
            if (customer == null) return new List<ProductResDto>();

            // Lấy danh sách sản phẩm mà người dùng đã tương tác (like + mua)
            var likedProductIds = await _db.Likes
                .Where(l => l.CustomerId == customer.Id)
                .Select(l => l.ProductId)
                .ToListAsync();

            var purchasedProductIds = await _db.SubOrders
                .Where(so => so.Order.CustomerId == customer.Id)
                .SelectMany(so => so.DetailOrders.Select(d => d.ProductId))
                .Distinct()
                .ToListAsync();

            var interactedIds = likedProductIds.Concat(purchasedProductIds).Distinct().ToList();
            if (!interactedIds.Any()) return new List<ProductResDto>();

            // Lấy toàn bộ sản phẩm còn bán
            var allProducts = await _db.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductType)
                .Include(p => p.Shop)
                .Where(p => !p.IsDeleted)
                .ToListAsync();

            // -------------------- 1. Chuẩn bị dữ liệu nội dung --------------------
            var productContents = allProducts.Select(p =>
                $"{p.Name} {p.Description} {p.ProductType?.Name} {p.Shop?.Name}".ToLower()
            ).ToList();

            // Tokenize thành danh sách từ
            var tokenizedProducts = productContents
                .Select(doc => doc.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                .ToList();

            // Xây dựng từ điển từ vựng
            var vocabulary = tokenizedProducts
                .SelectMany(x => x)
                .Distinct()
                .ToList();

            int N = allProducts.Count;           // Số sản phẩm
            int V = vocabulary.Count;            // Kích thước từ vựng

            // -------------------- 2. Tính ma trận TF-IDF --------------------
            double[][] tfidfMatrix = tokenizedProducts.Select(tokens =>
            {
                return vocabulary.Select(term =>
                {
                    int tf = tokens.Count(t => t == term);
                    int df = tokenizedProducts.Count(doc => doc.Contains(term));
                    double idf = Math.Log((double)N / (1 + df));
                    return tf * idf;
                }).ToArray();
            }).ToArray();

            // -------------------- 3. Tạo hồ sơ người dùng --------------------
            var userVectors = allProducts
                .Select((p, idx) => new { p.Id, Vector = tfidfMatrix[idx] })
                .Where(x => interactedIds.Contains(x.Id))
                .Select(x => x.Vector)
                .ToList();

            // Trung bình cộng vector để tạo hồ sơ người dùng
            double[] userProfile = new double[V];
            foreach (var vec in userVectors)
            {
                for (int i = 0; i < V; i++)
                    userProfile[i] += vec[i];
            }
            for (int i = 0; i < V; i++)
                userProfile[i] /= userVectors.Count;

            // -------------------- 4. Tính Cosine Similarity cho sản phẩm chưa tương tác --------------------
            double CosineSimilarity(double[] a, double[] b)
            {
                double dot = 0.0, normA = 0.0, normB = 0.0;
                for (int i = 0; i < a.Length; i++)
                {
                    dot += a[i] * b[i];
                    normA += a[i] * a[i];
                    normB += b[i] * b[i];
                }
                return dot / (Math.Sqrt(normA) * Math.Sqrt(normB) + 1e-9); // +1e-9 để tránh chia 0
            }

            var recommendations = allProducts
                .Select((p, idx) => new
                {
                    Product = p,
                    Score = interactedIds.Contains(p.Id)
                        ? -1 // bỏ qua sản phẩm đã tương tác
                        : CosineSimilarity(userProfile, tfidfMatrix[idx])
                })
                .OrderByDescending(x => x.Score)
                .Take(8)
                .ToList();

            var likedSet = likedProductIds.ToHashSet();

            // -------------------- 5. Map sang DTO --------------------
            return recommendations.Select(x => new ProductResDto
            {
                Id = x.Product.Id,
                Name = x.Product.Name,
                Price = x.Product.Price,
                Quantity = x.Product.Quantity,
                Description = x.Product.Description,
                Images = x.Product.ProductImages.Select(img => new ProductImageResDto
                {
                    Id = img.Id,
                    Path = img.Path
                }).ToList(),
                IsLiked = likedSet.Contains(x.Product.Id)
            }).ToList();
        }

        // Action để load block gợi ý sản phẩm (AJAX hoặc render trực tiếp)
        public async Task<IActionResult> SuggestedProducts()
        {
            var dtos = await GetSuggestedProductsAsync();
            return PartialView("_ProductListPartial", dtos);
        }


        // Lấy userId đăng nhập
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
