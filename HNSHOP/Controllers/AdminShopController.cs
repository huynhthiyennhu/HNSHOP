using HNSHOP.Data;
using HNSHOP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using HNSHOP.Services;

[Authorize(Roles = "Admin")]
public class AdminShopController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IEmailService _emailService;

    public AdminShopController(ApplicationDbContext db, IEmailService emailService)
    {
        _db = db;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _db.Shops
            .Include(s => s.Account)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(s => s.Name.Contains(search) || s.Account.Email.Contains(search));
        }

        var shops = await query.ToListAsync();
        return View(shops);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id) 
    {
        var shop = await _db.Shops
            .Include(s => s.Account)
            .FirstOrDefaultAsync(s => s.AccountId == id); 

        if (shop == null || shop.Account == null)
            return Json(new { success = false, message = "Shop không tồn tại!" });

        if (shop.Account.IsApproved)
            return Json(new { success = false, message = "Shop đã được duyệt trước đó." });

        // Duyệt tài khoản
        shop.Account.IsApproved = true;
        string token = new Random().Next(100000, 999999).ToString();
        shop.Account.VerifyToken = token;

        await _db.SaveChangesAsync();

        await _emailService.SendApprovedNotification(shop.Account.Email, shop.Name, token);

        return Json(new { success = true, message = "Đã duyệt Shop và gửi mã xác thực!" });
    }


    [HttpPost]
    public async Task<IActionResult> Reject(int accountId)
    {
        var account = await _db.Accounts
            .Include(a => a.Shop)
            .FirstOrDefaultAsync(a => a.Id == accountId && a.RoleId == 3);

        if (account == null)
            return Json(new { success = false, message = "Tài khoản không tồn tại hoặc không hợp lệ!" });

        // Gửi email từ chối
        await _emailService.SendRejectedShopEmail(account.Email, account.Shop?.Name ?? "Shop");

        // Xóa Shop và Account
        if (account.Shop != null)
            _db.Shops.Remove(account.Shop);

        _db.Accounts.Remove(account);
        await _db.SaveChangesAsync();

        return Json(new { success = true, message = "Đã từ chối và xóa tài khoản Shop!" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var shop = await _db.Shops.Include(s => s.Account).FirstOrDefaultAsync(s => s.Id == id);
        if (shop == null) return NotFound();

        _db.Shops.Remove(shop);
        await _db.SaveChangesAsync();

        return Json(new { success = true, message = "Shop đã được xóa thành công!" });
    }
}
