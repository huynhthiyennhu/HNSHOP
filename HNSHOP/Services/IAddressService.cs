using HNSHOP.Dtos.Request;
using HNSHOP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public interface IAddressService
    {
        Task<List<Address>> GetAddressesByCustomerIdAsync(int customerId);
        Task<bool> CreateAddressAsync(int customerId, AddressReqDto addressReq);
        Task<bool> UpdateAddressAsync(int id, AddressReqDto addressReq);
        Task<bool> DeleteAddressAsync(int id);
    }
}
