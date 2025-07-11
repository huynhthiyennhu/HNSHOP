using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Models;
using HNSHOP.Services;
using HNSHOP.Utils.EnumTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HNSHOP.Controllers
{
    [Authorize(Roles = "User")]
    public class RatingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RatingsController(ApplicationDbContext context, IMapper mapper,  IWebHostEnvironment env)
        {
            _context = context;
            IMapper _mapper = mapper;
            IWebHostEnvironment _env = env;
        }
        private int GetUserIdFromToken()
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : -1;
        }
        [HttpGet]
        public async Task<IActionResult> Create(int subOrderId)
        {
            var subOrder = await _context.SubOrders
                .Include(s => s.Shop)
                .Include(s => s.DetailOrders)
                    .ThenInclude(d => d.Product)
                    .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(s => s.Id == subOrderId);

            if (subOrder == null)
                return NotFound();

            return View(subOrder); // Trả về View với Model là SubOrder
        }


        [HttpPost]
        public async Task<IActionResult> Create(List<Rating> ratings)
        {
            int userId = GetUserIdFromToken();
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.AccountId == userId);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Index", "Orders");
            }

            foreach (var rating in ratings)
            {
                bool isPurchased = await _context.DetailOrders
                 .Include(d => d.SubOrder)
                     .ThenInclude(so => so.Order)
                 .AnyAsync(d =>
                     d.ProductId == rating.ProductId &&
                     d.SubOrder.Id == rating.SubOrderId &&
                     d.SubOrder.Order.CustomerId == customer.Id &&
                     d.SubOrder.Status == SubOrderStatus.Completed);


                if (!isPurchased)
                {
                    TempData["ErrorMessage"] = "Bạn chỉ có thể đánh giá sản phẩm đã mua.";
                    continue;
                }

                // Kiểm tra đã đánh giá chưa
                bool alreadyRated = await _context.Ratings
                    .AnyAsync(r =>
                        r.ProductId == rating.ProductId &&
                        r.CustomerId == customer.Id &&
                        r.SubOrderId == rating.SubOrderId);

                if (alreadyRated)
                {
                    TempData["ErrorMessage"] = "Bạn đã đánh giá sản phẩm này.";
                    continue;
                }

                rating.CustomerId = customer.Id;
                rating.UserName = customer.Name ?? "Người dùng";
                rating.CreatedAt = DateTime.Now;

                _context.Ratings.Add(rating);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cảm ơn bạn đã đánh giá sản phẩm!";
            return RedirectToAction("Index", "Orders");
        }

        private int GetCustomerId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
