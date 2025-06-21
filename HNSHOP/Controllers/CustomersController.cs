using HNSHOP.Data;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _db;

    public CustomersController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string search = "")
    {
        var query = _db.Customers
            .Include(c => c.Account)
            .Include(c => c.CustomerType)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.Name.Contains(search) || c.Account.Email.Contains(search));
        }
        ViewBag.CustomerTypes = await _db.CustomerTypes.ToListAsync();
        var customers = await query.ToListAsync();
        return View(customers);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateType(int id, int customerTypeId)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer == null) return Json(new { success = false, message = "Khách hàng không tồn tại." });

        customer.CustomerTypeId = customerTypeId;
        await _db.SaveChangesAsync();
        return Json(new { success = true, message = "Loại khách hàng đã được cập nhật." });
    }


    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer == null) return Json(new { success = false, message = "Khách hàng không tồn tại." });

        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();
        return Json(new { success = true, message = "Đã xóa khách hàng thành công." });
    }


}
