using HNSHOP.Data;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Components
{
    public class ProductTypeViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ProductTypeViewComponent(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var productTypes = await _db.ProductTypes.ToListAsync();
            var result = _mapper.Map<List<ProductTypeResDto>>(productTypes);
            return View(result);
        }
    }
}
