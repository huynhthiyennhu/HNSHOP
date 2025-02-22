using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public interface IProductService
    {
        Task<List<ProductResDto>> GetAllProductsAsync();
        Task<ProductResDto?> GetProductByIdAsync(int productId);
        Task<bool> CreateProductAsync(ProductReqDto productReq);
        Task<bool> UpdateProductAsync(int productId, UpdateProductReqDto updateProductReq);
        Task<bool> DeleteProductAsync(int productId);
    }
}
