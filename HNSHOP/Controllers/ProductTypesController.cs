using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using HNSHOP.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HNSHOP.Controllers
{
    [Authorize]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ProductTypesController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// Hiển thị danh sách loại sản phẩm
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var productTypes = await _db.ProductTypes.ToListAsync();
            var result = _mapper.Map<List<ProductTypeResDto>>(productTypes);
            return View(result);
        }

        /// <summary>
        /// Hiển thị chi tiết loại sản phẩm
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var productType = await _db.ProductTypes.FindAsync(id);
            if (productType == null) return NotFound();

            var result = _mapper.Map<ProductTypeResDto>(productType);
            return View(result);
        }

        /// <summary>
        /// Hiển thị form tạo loại sản phẩm
        /// </summary>
        [Authorize(Roles = ConstConfig.AdminRoleName)]
        public IActionResult Create() => View();

        /// <summary>
        /// Xử lý tạo loại sản phẩm
        /// </summary>
        [Authorize(Roles = ConstConfig.AdminRoleName)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypeReqDto productTypeReq)
        {
            if (!ModelState.IsValid) return View(productTypeReq);

            var productType = _mapper.Map<ProductType>(productTypeReq);
            _db.ProductTypes.Add(productType);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Hiển thị form chỉnh sửa loại sản phẩm
        /// </summary>
        [Authorize(Roles = ConstConfig.AdminRoleName)]
        public async Task<IActionResult> Edit(int id)
        {
            var productType = await _db.ProductTypes.FindAsync(id);
            if (productType == null) return NotFound();

            var result = _mapper.Map<ProductTypeReqDto>(productType);
            return View(result);
        }

        /// <summary>
        /// Xử lý cập nhật loại sản phẩm
        /// </summary>
        [Authorize(Roles = ConstConfig.AdminRoleName)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductTypeReqDto updateProductTypeReq)
        {
            if (!ModelState.IsValid) return View(updateProductTypeReq);

            var productType = await _db.ProductTypes.FindAsync(id);
            if (productType == null) return NotFound();

            _mapper.Map(updateProductTypeReq, productType);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xóa loại sản phẩm
        /// </summary>
        [Authorize(Roles = ConstConfig.AdminRoleName)]
        public async Task<IActionResult> Delete(int id)
        {
            var productType = await _db.ProductTypes.FindAsync(id);
            if (productType == null) return NotFound();

            _db.ProductTypes.Remove(productType);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
