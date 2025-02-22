using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public class ProductTypeService : IProductTypeService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ProductTypeService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<ProductTypeResDto>> GetAllProductTypesAsync()
        {
            var productTypes = await _db.ProductTypes.ToListAsync();
            return _mapper.Map<List<ProductTypeResDto>>(productTypes);
        }

        public async Task<ProductTypeResDto?> GetProductTypeByIdAsync(int productTypeId)
        {
            var productType = await _db.ProductTypes.FirstOrDefaultAsync(pt => pt.Id == productTypeId);
            return _mapper.Map<ProductTypeResDto>(productType);
        }

        public async Task<bool> CreateProductTypeAsync(ProductTypeReqDto productTypeReq)
        {
            var newProductType = _mapper.Map<ProductType>(productTypeReq);
            await _db.ProductTypes.AddAsync(newProductType);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProductTypeAsync(int productTypeId, ProductTypeReqDto updateProductTypeReq)
        {
            var productType = await _db.ProductTypes.FirstOrDefaultAsync(pt => pt.Id == productTypeId);
            if (productType == null) return false;

            _mapper.Map(updateProductTypeReq, productType);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteProductTypeAsync(int productTypeId)
        {
            var productType = await _db.ProductTypes.FirstOrDefaultAsync(pt => pt.Id == productTypeId);
            if (productType == null) return false;

            _db.ProductTypes.Remove(productType);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
