using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ShopsController(ApplicationDbContext db, IMapper mapper) : Controller
{
    private readonly ApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public async Task<IActionResult> Index()
    {
        var shops = await _db.Shops.ToListAsync();
        return View(_mapper.Map<List<ShopResDto>>(shops));
    }

    public async Task<IActionResult> Details(int id)
    {
        var shop = await _db.Shops.Include(s => s.Products).FirstOrDefaultAsync(s => s.Id == id);
        if (shop == null) return NotFound();
        return View(_mapper.Map<ShopResDto>(shop));
    }

    [Authorize(Roles = ConstConfig.ShopRoleName)]
    public async Task<IActionResult> Edit(int id)
    {
        var shop = await _db.Shops.FindAsync(id);
        if (shop == null) return NotFound();
        return View(_mapper.Map<UpdateShopReqDto>(shop));
    }

    [Authorize(Roles = ConstConfig.ShopRoleName)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateShopReqDto updateShopReq)
    {
        var shop = await _db.Shops.FindAsync(id);
        if (shop == null) return NotFound();

        _mapper.Map(updateShopReq, shop);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [Authorize(Roles = ConstConfig.AdminRoleName)]
    public async Task<IActionResult> Delete(int id)
    {
        var shop = await _db.Shops.FindAsync(id);
        if (shop == null) return NotFound();

        _db.Shops.Remove(shop);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
