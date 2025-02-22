using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class CustomerTypesController(ApplicationDbContext db, IMapper mapper) : Controller
{
    private readonly ApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public async Task<IActionResult> Index()
    {
        var customerTypes = await _db.CustomerTypes.ToListAsync();
        return View(_mapper.Map<List<CustomerTypeResDto>>(customerTypes));
    }

    public async Task<IActionResult> Details(int id)
    {
        var customerType = await _db.CustomerTypes.FindAsync(id);
        if (customerType == null) return NotFound();
        return View(_mapper.Map<CustomerTypeResDto>(customerType));
    }
}
