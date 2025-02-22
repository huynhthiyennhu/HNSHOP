using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public interface IProductTypeService
    {
        Task<List<ProductTypeResDto>> GetAllProductTypesAsync();
        Task<ProductTypeResDto?> GetProductTypeByIdAsync(int productTypeId);
        Task<bool> CreateProductTypeAsync(ProductTypeReqDto productTypeReq);
        Task<bool> UpdateProductTypeAsync(int productTypeId, ProductTypeReqDto updateProductTypeReq);
        Task<bool> DeleteProductTypeAsync(int productTypeId);
    }
}
