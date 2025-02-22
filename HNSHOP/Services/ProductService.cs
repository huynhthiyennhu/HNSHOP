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
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<ProductResDto>> GetAllProductsAsync()
        {
            var products = await _db.Products.Include(p => p.ProductImages).ToListAsync();
            return _mapper.Map<List<ProductResDto>>(products);
        }

        public async Task<ProductResDto?> GetProductByIdAsync(int productId)
        {
            var product = await _db.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == productId);
            return _mapper.Map<ProductResDto>(product);
        }

        public async Task<bool> CreateProductAsync(ProductReqDto productReq)
        {
            var newProduct = _mapper.Map<Product>(productReq);
            await _db.Products.AddAsync(newProduct);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProductAsync(int productId, UpdateProductReqDto updateProductReq)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return false;

            _mapper.Map(updateProductReq, product);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return false;

            _db.Products.Remove(product);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
