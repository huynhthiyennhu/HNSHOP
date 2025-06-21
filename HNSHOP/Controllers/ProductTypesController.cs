using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class ProductTypeController : Controller
{
    private readonly ApplicationDbContext _db;

    public ProductTypeController(ApplicationDbContext db)
    {
        _db = db;
    }

    // Danh sách loại sản phẩm
    public async Task<IActionResult> Index()
    {
        var productTypes = await _db.ProductTypes.AsNoTracking().ToListAsync();
        return View(productTypes);
    }

    // Thêm loại sản phẩm
    [HttpPost]
    public async Task<IActionResult> Create(ProductTypeReqDto req)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });

        var productType = new ProductType
        {
            Name = req.Name,
            Description = req.Description
        };

        _db.ProductTypes.Add(productType);
        await _db.SaveChangesAsync();

        return Json(new { success = true, message = "Thêm loại sản phẩm thành công!" });
    }

    // Lấy thông tin loại sản phẩm để sửa
    public async Task<IActionResult> Get(int id)
    {
        var productType = await _db.ProductTypes.FindAsync(id);
        if (productType == null) return NotFound();

        return Json(productType);
    }

    // Cập nhật loại sản phẩm
    [HttpPost]
    public async Task<IActionResult> Edit(int id, ProductTypeReqDto req)
    {
        var productType = await _db.ProductTypes.FindAsync(id);
        if (productType == null) return NotFound();

        productType.Name = req.Name;
        productType.Description = req.Description;

        await _db.SaveChangesAsync();
        return Json(new { success = true, message = "Cập nhật loại sản phẩm thành công!" });
    }

    // Xóa loại sản phẩm
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var productType = await _db.ProductTypes.FindAsync(id);
        if (productType == null) return NotFound();

        _db.ProductTypes.Remove(productType);
        await _db.SaveChangesAsync();
        return Json(new { success = true, message = "Xóa loại sản phẩm thành công!" });
    }
}
