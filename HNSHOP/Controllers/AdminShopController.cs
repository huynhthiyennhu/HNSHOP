using HNSHOP.Data;
using HNSHOP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")]
public class AdminShopController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminShopController(ApplicationDbContext db)
    {
        _db = db;
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


    // 📌 Xóa Shop
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
