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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSaleEventReqDto req, IFormFile? Illustration)
    {
        if (Illustration == null || Illustration.Length == 0)
        {
            ModelState.AddModelError("Illustration", "Ảnh minh họa là bắt buộc");
        }

        if (!ModelState.IsValid)
        {
            await LoadCreateViewBagsAsync();
            return View(req);
        }

        try
        {
            string illustrationPath = string.Empty;
            if (Illustration != null && Illustration.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images/hnshop");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Illustration.FileName)}";
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Illustration.CopyToAsync(stream);
                }

                illustrationPath = fileName; 
            }

            var customerTypes = (req.CustomerTypeIds ?? new List<int>())
                .Select(id => new CustomerTypeSaleEvent { CustomerTypeId = id })
                .ToList();

            var products = (req.ProductIds ?? new List<int>())
                .Select(id => new ProductSaleEvent { ProductId = id })
                .ToList();

            if (!products.Any())
            {
                ModelState.AddModelError("ProductIds", "Vui lòng chọn ít nhất một sản phẩm áp dụng.");
                await LoadCreateViewBagsAsync();
                return View(req);
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

            TempData["Message"] = "Tạo chương trình giảm giá thành công.";
            return RedirectToAction("Index");
        }
        catch (DbUpdateException dbEx)
        {
            ModelState.AddModelError("", "Lỗi khi lưu vào database: " + dbEx.Message);
            await LoadCreateViewBagsAsync();
            return View(req);
        }
        catch (Exception)
        {
            TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại.";
            return RedirectToAction("Index");
        }
    }

    private async Task LoadCreateViewBagsAsync()
    {
        ViewBag.CustomerTypes = await _db.CustomerTypes
            .AsNoTracking()
            .ToListAsync();

        ViewBag.Products = await _db.Products
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .Include(p => p.ProductImages)
            .Select(p => new ProductResDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Images = p.ProductImages.Select(img => new ProductImageResDto
                {
                    Id = img.Id,
                    Path = img.Path
                }).ToList()
            })
            .ToListAsync();
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] UpdateSaleEventReqDto req, IFormFile? Illustration)
    {
        var saleEvent = await _db.SaleEvents
            .Include(se => se.ProductSaleEvents)
            .Include(se => se.CustomerTypeSaleEvents)
            .FirstOrDefaultAsync(se => se.Id == id);

        if (saleEvent == null) return NotFound();

        if (!ModelState.IsValid)
        {
            var vm = await BuildEditViewModelAsync(saleEvent, req);
            return View(vm); 
        }

        if (Illustration != null && Illustration.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "images/hnshop");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Illustration.FileName)}";
            var path = Path.Combine(uploads, fileName);
            using (var fs = new FileStream(path, FileMode.Create))
                await Illustration.CopyToAsync(fs);

            saleEvent.Illustration = fileName;
        }

        saleEvent.Name = req.Name ?? saleEvent.Name;
        saleEvent.Description = req.Description ?? saleEvent.Description;
        saleEvent.Discount = req.Discount ?? saleEvent.Discount;   
        saleEvent.StartDate = req.StartDate ?? saleEvent.StartDate;
        saleEvent.EndDate = req.EndDate ?? saleEvent.EndDate;

        if (req.CustomerTypeIds != null)
        {
            saleEvent.CustomerTypeSaleEvents.Clear();
            saleEvent.CustomerTypeSaleEvents = req.CustomerTypeIds
                .Distinct()
                .Select(id => new CustomerTypeSaleEvent { CustomerTypeId = id })
                .ToList();
        }

        if (req.ProductIds != null)
        {
            saleEvent.ProductSaleEvents.Clear();
            saleEvent.ProductSaleEvents = req.ProductIds
                .Distinct()
                .Select(id => new ProductSaleEvent { ProductId = id })
                .ToList();
        }

        await _db.SaveChangesAsync();
        TempData["Message"] = "Cập nhật chương trình giảm giá thành công.";
        return RedirectToAction("Index");
    }

    private async Task<SaleEventProductResDto> BuildEditViewModelAsync(SaleEvent se, UpdateSaleEventReqDto req)
    {
        var selectedCt = (req.CustomerTypeIds ?? se.CustomerTypeSaleEvents.Select(x => x.CustomerTypeId).ToList()).ToHashSet();
        var selectedPd = (req.ProductIds ?? se.ProductSaleEvents.Select(x => x.ProductId).ToList()).ToHashSet();

        return new SaleEventProductResDto
        {
            Id = se.Id,
            Name = req.Name ?? se.Name,
            Description = req.Description ?? se.Description,
            Illustration = se.Illustration,
            Discount = req.Discount ?? se.Discount,       
            StartDate = req.StartDate ?? se.StartDate,
            EndDate = req.EndDate ?? se.EndDate,

            CustomerTypes = await _db.CustomerTypes
                .AsNoTracking()
                .Select(ct => new CustomerTypeResDto
                {
                    Id = ct.Id,
                    Name = ct.Name,
                    Description = ct.Description,
                    IsSelected = selectedCt.Contains(ct.Id)
                }).ToListAsync(),

            Products = await _db.Products
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Include(p => p.ProductImages)
                .Select(p => new ProductResDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Images = p.ProductImages.Select(i => new ProductImageResDto { Id = i.Id, Path = i.Path }).ToList(),
                    IsSelected = selectedPd.Contains(p.Id)
                }).ToListAsync()
        };
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
