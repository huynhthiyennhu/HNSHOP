using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using HNSHOP.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class SalesController(ApplicationDbContext db, IMapper mapper) : Controller
{
    private readonly ApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public async Task<IActionResult> Index()
    {
        var sales = await _db.SaleEvents.Include(s => s.Products).ToListAsync();
        return View(_mapper.Map<List<SaleEventResDto>>(sales));
    }

    public async Task<IActionResult> Details(int id)
    {
        var sale = await _db.SaleEvents.Include(s => s.Products).FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null) return NotFound();
        return View(_mapper.Map<SaleEventResDto>(sale));
    }

    [Authorize(Roles = ConstConfig.AdminRoleName)]
    public IActionResult Create() => View();

    [Authorize(Roles = ConstConfig.AdminRoleName)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSaleEventReqDto saleReq)
    {
        var saleEvent = _mapper.Map<SaleEvent>(saleReq);
        _db.SaleEvents.Add(saleEvent);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [Authorize(Roles = ConstConfig.AdminRoleName)]
    public async Task<IActionResult> Edit(int id)
    {
        var sale = await _db.SaleEvents.FindAsync(id);
        if (sale == null) return NotFound();
        return View(_mapper.Map<UpdateSaleEventReqDto>(sale));
    }

    [Authorize(Roles = ConstConfig.AdminRoleName)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateSaleEventReqDto updateSaleReq)
    {
        var sale = await _db.SaleEvents.FindAsync(id);
        if (sale == null) return NotFound();

        _mapper.Map(updateSaleReq, sale);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [Authorize(Roles = ConstConfig.AdminRoleName)]
    public async Task<IActionResult> Delete(int id)
    {
        var sale = await _db.SaleEvents.FindAsync(id);
        if (sale == null) return NotFound();

        _db.SaleEvents.Remove(sale);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
