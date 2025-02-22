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
    public class ShopService : IShopService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ShopService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<ShopResDto>> GetAllShopsAsync()
        {
            var shops = await _db.Shops.ToListAsync();
            return _mapper.Map<List<ShopResDto>>(shops);
        }

        public async Task<ShopResDto?> GetShopByIdAsync(int shopId)
        {
            var shop = await _db.Shops.FirstOrDefaultAsync(s => s.Id == shopId);
            return _mapper.Map<ShopResDto>(shop);
        }

        public async Task<bool> UpdateShopAsync(int shopId, UpdateShopReqDto updateShopReq)
        {
            var shop = await _db.Shops.FirstOrDefaultAsync(s => s.Id == shopId);
            if (shop == null) return false;

            _mapper.Map(updateShopReq, shop);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteShopAsync(int shopId)
        {
            var shop = await _db.Shops.FirstOrDefaultAsync(s => s.Id == shopId);
            if (shop == null) return false;

            _db.Shops.Remove(shop);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
