using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class AccountsController : Controller
{
    private readonly ApplicationDbContext _db;

    public AccountsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        int userId = GetUserIdFromToken();
        var account = await _db.Accounts
            .Include(a => a.Customer)
            .Include(a => a.Shop)
                .ThenInclude(s => s.Products)
                    .ThenInclude(p => p.ProductImages)
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
                Name = account.Shop.Name,
                Description = account.Shop.Description,
                Products = account.Shop.Products.Select(p => new ProductResDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Images = p.ProductImages.Select(i => new ProductImageResDto { Path = i.Path }).ToList()
                }).ToList()
            } : null,
            Customer = account.Customer != null ? new CustomerResDto { Name = account.Customer.Name, Description = account.Customer.Description } : null
        };

        return View(profileDto);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateAccountReqDto request)
    {
        int userId = GetUserIdFromToken();
        var account = await _db.Accounts.FindAsync(userId);
        if (account == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(request.Email)) account.Email = request.Email;
        if (!string.IsNullOrWhiteSpace(request.Phone)) account.Phone = request.Phone;
        if (!string.IsNullOrWhiteSpace(request.Avatar)) account.Avatar = request.Avatar;

        account.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
        return RedirectToAction("Index");
    }

    private int GetUserIdFromToken()
    {
        return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : -1;
    }
}
