using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Models;
using HNSHOP.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = ConstConfig.UserRoleName)]
public class AddressesController(ApplicationDbContext db, IMapper mapper) : Controller
{
    private readonly ApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;


    public async Task<IActionResult> Index()
    {
        int customerId = GetUserIdFromToken();
        var addresses = await _db.Addresses.Where(a => a.CustomerId == customerId).ToListAsync();
        return View(addresses);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddressReqDto addressReq)
    {
        int customerId = GetUserIdFromToken();
        var address = _mapper.Map<Address>(addressReq);
        address.CustomerId = customerId;
        _db.Addresses.Add(address);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var address = await _db.Addresses.FindAsync(id);
        if (address == null) return NotFound();
        return View(_mapper.Map<AddressReqDto>(address));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AddressReqDto addressReq)
    {
        var address = await _db.Addresses.FindAsync(id);
        if (address == null) return NotFound();
        _mapper.Map(addressReq, address);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var address = await _db.Addresses.FindAsync(id);
        if (address == null) return NotFound();
        _db.Addresses.Remove(address);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    private int GetUserIdFromToken()
    {
        return int.TryParse(User.FindFirst(ConstConfig.UserIdClaimType)?.Value, out int userId) ? userId : -1;
    }
}
