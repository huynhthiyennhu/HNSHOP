using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public AddressService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<Address>> GetAddressesByCustomerIdAsync(int customerId)
        {
            return await _db.Addresses
                .Where(a => a.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<bool> CreateAddressAsync(int customerId, AddressReqDto addressReq)
        {
            var addressString = $"{addressReq.HouseNumber}, {addressReq.Street}, {addressReq.Ward}, {addressReq.District}, {addressReq.City}";

            var newAddress = new Address()
            {
                AddressDetail = addressString,
                CustomerId = customerId
            };

            await _db.Addresses.AddAsync(newAddress);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAddressAsync(int id, AddressReqDto addressReq)
        {
            var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == id);
            if (address == null) return false;

            address.AddressDetail = $"{addressReq.HouseNumber}, {addressReq.Street}, {addressReq.Ward}, {addressReq.District}, {addressReq.City}";

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == id);
            if (address == null) return false;

            _db.Addresses.Remove(address);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
