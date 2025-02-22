using HNSHOP.Dtos.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public interface ICustomerTypeService
    {
        Task<List<CustomerTypeResDto>> GetCustomerTypesAsync();
    }
}
