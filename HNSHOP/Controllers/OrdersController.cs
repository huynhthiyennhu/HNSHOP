using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Response;
using HNSHOP.Utils;
using HNSHOP.Utils.EnumTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class OrdersController(ApplicationDbContext db, IMapper mapper) : Controller
{
    private readonly ApplicationDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    public async Task<IActionResult> Index()
    {
        int customerId = GetUserIdFromToken();
        var orders = await _db.Orders.Where(o => o.CustomerId == customerId).ToListAsync();
        return View(_mapper.Map<List<OrderResDto>>(orders));
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _db.Orders
            .Include(o => o.DetailOrders)
            .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();
        return View(_mapper.Map<OrderResDto>(order));
    }

    [Authorize(Roles = ConstConfig.UserRoleName)]
    public async Task<IActionResult> Cancel(int id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order == null) return NotFound();
        order.Status = OrderStatus.Cancelled;
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    private int GetUserIdFromToken()
    {
        return int.TryParse(User.FindFirst(ConstConfig.UserIdClaimType)?.Value, out int userId) ? userId : -1;
    }
}
