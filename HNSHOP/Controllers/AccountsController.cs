using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using HNSHOP.Utils.EnumTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Http;
using HNSHOP.Utils;
using HNSHOP.Utils.QueryParams;
using HNSHOP.Services;

public class AccountsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;


    public AccountsController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index(int? id)
    {
        int userId = id ?? GetUserIdFromToken();
        int loggedInUserId = GetUserIdFromToken();
        var account = await _db.Accounts
            .Include(a => a.Customer)
            .Include(a => a.Shop)
                .ThenInclude(s => s.Products)
                .ThenInclude(p => p.ProductImages)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == userId);

        if (account == null) return NotFound();

        var profileDto = new AccountResDto
        {
            Id = account.Id,
            Email = account.Email,
            Phone = account.Phone,
            Avatar = string.IsNullOrWhiteSpace(account.Avatar) ? "/images/default-avatar.png" : account.Avatar,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt,
            Shop = account.Shop != null ? new ShopResDto
            {
                Id = account.Shop.Id,
                Name = account.Shop.Name,
                Description = account.Shop.Description,
                Products = account.Shop.Products.Select(p => new ProductResDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Images = p.ProductImages.Select(img => new ProductImageResDto
                    {
                        Id = img.Id,
                        Path = img.Path
                    }).ToList()
                }).ToList()
            } : null,
            Customer = account.Customer != null ? new CustomerResDto
            {
                Name = account.Customer.Name,
                Description = account.Customer.Description
            } : null
        };
        ViewBag.IsOwner = (loggedInUserId == userId);

        return View(profileDto);
    }


    [HttpPost]
    public async Task<IActionResult> Update(UpdateAccountReqDto request)
    {
        int userId = GetUserIdFromToken();
        var account = await _db.Accounts
            .Include(a => a.Customer)
            .Include(a => a.Shop)
            .FirstOrDefaultAsync(a => a.Id == userId);

        if (account == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            if (account.Shop != null)
            {
                account.Shop.Name = request.Name;
            }
            else if (account.Customer != null)
            {
                account.Customer.Name = request.Name;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Email)) account.Email = request.Email;
        if (!string.IsNullOrWhiteSpace(request.Phone)) account.Phone = request.Phone;

        if (account.Shop != null && !string.IsNullOrWhiteSpace(request.Description))
        {
            account.Shop.Description = request.Description;
        }

        account.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(int id, OrderStatus status)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = status;
        order.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAvatar(IFormFile Avatar)
    {
        if (Avatar == null || Avatar.Length == 0)
            return Json(new { success = false, message = "Vui lòng chọn một ảnh hợp lệ." });

        int userId = GetUserIdFromToken();
        var user = await _db.Accounts.FindAsync(userId);

        if (user == null)
            return Json(new { success = false, message = "Người dùng không tồn tại." });

        try
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "hnshop", "avatar");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                string oldImagePath = Path.Combine(_env.WebRootPath, user.Avatar.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            string fileName = $"user_{userId}.jpg";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Avatar.CopyToAsync(stream);
            }

            user.Avatar =fileName;
            await _db.SaveChangesAsync();

            return Json(new { success = true, imageName = fileName });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi cập nhật ảnh: {ex.Message}");
            return Json(new { success = false, message = "Lỗi khi cập nhật ảnh: " + ex.Message });
        }
    }

    private int GetUserIdFromToken()
    {
        return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : -1;
    }


   

}
