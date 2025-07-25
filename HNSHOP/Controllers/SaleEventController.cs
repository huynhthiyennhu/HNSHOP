using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using HNSHOP.Dtos.Response;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

[Authorize(Roles = "Admin")]
public class SaleEventController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;


    public SaleEventController(ApplicationDbContext db, IWebHostEnvironment env, IMapper mapper)
    {
        _db = db;
        _env = env;
        _mapper = mapper;
    }

    // View danh sách chương trình giảm giá
    public async Task<IActionResult> Index()
    {
        var saleEvents = await _db.SaleEvents.ToListAsync();
        var saleEventDtos = _mapper.Map<List<SaleEventResDto>>(saleEvents);
        return View(saleEventDtos);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var saleEvent = await _db.SaleEvents
            .Include(se => se.ProductSaleEvents)
            .Include(se => se.CustomerTypeSaleEvents)
            .FirstOrDefaultAsync(se => se.Id == id);

        if (saleEvent == null) return NotFound();

        // Lấy danh sách tất cả sản phẩm và loại khách hàng
        var allProducts = await _db.Products.Where(p => (!p.IsDeleted))
            .Include(p => p.ProductImages)
            .ToListAsync();
        var allCustomerTypes = await _db.CustomerTypes.ToListAsync();

        // Lấy thông tin tài khoản hiện tại
        int userId = GetUserIdFromToken();
        var user = await _db.Accounts
            .Include(u => u.Customer)
            .ThenInclude(c => c.CustomerType) // Lấy loại khách hàng nếu có
            .FirstOrDefaultAsync(u => u.Id == userId);

        bool isApplicableCustomer = user?.Customer?.CustomerType != null &&
            saleEvent.CustomerTypeSaleEvents.Any(ct => ct.CustomerTypeId == user.Customer.CustomerType.Id);

        // Chuyển đổi dữ liệu sang DTO
        var saleEventDto = new SaleEventProductResDto
        {
            Id = saleEvent.Id,
            Name = saleEvent.Name,
            Description = saleEvent.Description,
            Illustration = saleEvent.Illustration,
            Discount = saleEvent.Discount,
            StartDate = saleEvent.StartDate,
            EndDate = saleEvent.EndDate,
            CustomerTypes = allCustomerTypes.Select(ct => new CustomerTypeResDto
            {
                Id = ct.Id,
                Name = ct.Name,
                Description = ct.Description,
                IsSelected = saleEvent.CustomerTypeSaleEvents.Any(cse => cse.CustomerTypeId == ct.Id)
            }).ToList(),
            Products = allProducts.Select(p => new ProductResDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Images = p.ProductImages.Select(img => new ProductImageResDto { Id = img.Id, Path = img.Path }).ToList(),
                IsSelected = saleEvent.ProductSaleEvents.Any(pse => pse.ProductId == p.Id)
            }).ToList(),
            IsApplicableCustomer = isApplicableCustomer 
        };

        return View(saleEventDto);
    }



    private int GetUserIdFromToken()
    {
        return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : -1;
    }


    public IActionResult Create()
    {
        try
        {
            var customerTypes = _db.CustomerTypes.ToList();
            var products = _db.Products.Where(p => (!p.IsDeleted))
                .Include(p => p.ProductImages)
                .ToList();

            var productDtos = products.Select(p => new ProductResDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Images = p.ProductImages?.Select(img => new ProductImageResDto { Id = img.Id, Path = img.Path }).ToList() ?? new List<ProductImageResDto>()
            }).ToList();

            ViewBag.CustomerTypes = customerTypes;
            ViewBag.Products = productDtos;

            return View();
        }
        catch (Exception ex)
        {
            // Log lỗi nếu cần thiết
            return BadRequest($"Lỗi khi lấy dữ liệu: {ex.Message}");
        }
    }



    [HttpPost]
    public async Task<IActionResult> Create(CreateSaleEventReqDto req, IFormFile? Illustration)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join("; ", errors) });
            }

            string illustrationPath = string.Empty;

            // Xử lý lưu ảnh minh họa nếu có
            if (Illustration != null && Illustration.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images/hnshop");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Illustration.FileName)}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Illustration.CopyToAsync(stream);
                }

                illustrationPath = $"{fileName}";
            }

            // Kiểm tra danh sách loại khách hàng & sản phẩm trước khi thêm
            var customerTypes = req.CustomerTypeIds?.Select(id => new CustomerTypeSaleEvent { CustomerTypeId = id }).ToList() ?? new List<CustomerTypeSaleEvent>();
            var products = req.ProductIds?.Select(id => new ProductSaleEvent { ProductId = id }).ToList() ?? new List<ProductSaleEvent>();

            // Đảm bảo danh sách sản phẩm không trống
            if (!products.Any())
            {
                return Json(new { success = false, message = "Vui lòng chọn ít nhất một sản phẩm áp dụng." });
            }

            var saleEvent = new SaleEvent
            {
                Name = req.Name,
                Description = req.Description,
                Illustration = illustrationPath,
                Discount = req.Discount,
                StartDate = req.StartDate,
                EndDate = req.EndDate,
                CustomerTypeSaleEvents = customerTypes,
                ProductSaleEvents = products
            };

            _db.SaleEvents.Add(saleEvent);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        catch (DbUpdateException dbEx)
        {
            return Json(new { success = false, message = "Lỗi khi lưu vào database: " + dbEx.Message });
        }
        catch (Exception ex)
        {
            return RedirectToAction("Index");
        }
    }
    public async Task<IActionResult> Edit(int id)
    {
        var saleEvent = await _db.SaleEvents
            .Include(se => se.ProductSaleEvents)
            .Include(se => se.CustomerTypeSaleEvents)
            .FirstOrDefaultAsync(se => se.Id == id);

        if (saleEvent == null) return NotFound();

       
        var allProducts = await _db.Products.Where(p => (!p.IsDeleted))
            .Include(p => p.ProductImages)
            .ToListAsync();

        // Lấy danh sách tất cả loại khách hàng
        var allCustomerTypes = await _db.CustomerTypes
            .ToListAsync();

        // Chuyển đổi dữ liệu sang DTO, đánh dấu checkbox nếu sản phẩm/khách hàng đã được chọn
        var saleEventDto = new SaleEventProductResDto
        {
            Id = saleEvent.Id,
            Name = saleEvent.Name,
            Description = saleEvent.Description,
            Illustration = saleEvent.Illustration,
            Discount = saleEvent.Discount,
            StartDate = saleEvent.StartDate,
            EndDate = saleEvent.EndDate,
            CustomerTypes = allCustomerTypes.Select(ct => new CustomerTypeResDto
            {
                Id = ct.Id,
                Name = ct.Name,
                Description = ct.Description,
                IsSelected = saleEvent.CustomerTypeSaleEvents.Any(cse => cse.CustomerTypeId == ct.Id)
            }).ToList(),
            Products = allProducts.Select(p => new ProductResDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Images = p.ProductImages.Select(img => new ProductImageResDto { Id = img.Id, Path = img.Path }).ToList(),
                IsSelected = saleEvent.ProductSaleEvents.Any(pse => pse.ProductId == p.Id)
            }).ToList()
        };

        return View(saleEventDto);
    }






    [HttpPost]
    public async Task<IActionResult> Edit(int id, UpdateSaleEventReqDto req, IFormFile? Illustration)
    {
        var saleEvent = await _db.SaleEvents
            .Include(se => se.ProductSaleEvents)
            .Include(se => se.CustomerTypeSaleEvents)
            .FirstOrDefaultAsync(se => se.Id == id);

        if (saleEvent == null) return NotFound();

        if (Illustration != null && Illustration.Length > 0)
        {
            string fileName = $"{Guid.NewGuid()}_{Illustration.FileName}";
            string path = Path.Combine(_env.WebRootPath, "images/hnshop", fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await Illustration.CopyToAsync(stream);
            }
            saleEvent.Illustration = $"{fileName}";
        }

        saleEvent.Name = req.Name ?? saleEvent.Name;
        saleEvent.Description = req.Description ?? saleEvent.Description;
        saleEvent.Discount = req.Discount ?? saleEvent.Discount;
        saleEvent.StartDate = req.StartDate ?? saleEvent.StartDate;
        saleEvent.EndDate = req.EndDate ?? saleEvent.EndDate;

        // Cập nhật danh sách khách hàng áp dụng
        saleEvent.CustomerTypeSaleEvents.Clear();
        if (req.CustomerTypeIds != null)
        {
            saleEvent.CustomerTypeSaleEvents = req.CustomerTypeIds
                .Select(id => new CustomerTypeSaleEvent { CustomerTypeId = id })
                .ToList();
        }

        // Cập nhật danh sách sản phẩm áp dụng
        saleEvent.ProductSaleEvents.Clear();
        if (req.ProductIds != null)
        {
            saleEvent.ProductSaleEvents = req.ProductIds
                .Select(id => new ProductSaleEvent { ProductId = id })
                .ToList();
        }

        await _db.SaveChangesAsync();
        return RedirectToAction("Index");
    }



    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var saleEvent = await _db.SaleEvents.FindAsync(id);
        if (saleEvent == null)
            return Json(new { success = false, message = "Chương trình giảm giá không tồn tại." });

        try
        {
            _db.SaleEvents.Remove(saleEvent);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Chương trình giảm giá đã được xóa thành công!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Lỗi khi xóa: " + ex.Message });
        }
    }

}
