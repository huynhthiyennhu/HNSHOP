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
    public class SaleEventService : ISaleEventService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public SaleEventService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<SaleEventResDto>> GetAllSalesAsync()
        {
            var sales = await _db.SaleEvents.ToListAsync();
            return _mapper.Map<List<SaleEventResDto>>(sales);
        }

        public async Task<SaleEventResDto?> GetSaleEventByIdAsync(int saleEventId)
        {
            var sale = await _db.SaleEvents.FirstOrDefaultAsync(se => se.Id == saleEventId);
            return _mapper.Map<SaleEventResDto>(sale);
        }

        public async Task<bool> CreateSaleEventAsync(CreateSaleEventReqDto saleEventReq)
        {
            var newSaleEvent = _mapper.Map<SaleEvent>(saleEventReq);
            await _db.SaleEvents.AddAsync(newSaleEvent);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateSaleEventAsync(int saleEventId, UpdateSaleEventReqDto updateSaleReq)
        {
            var sale = await _db.SaleEvents.FirstOrDefaultAsync(se => se.Id == saleEventId);
            if (sale == null) return false;

            _mapper.Map(updateSaleReq, sale);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteSaleEventAsync(int saleEventId)
        {
            var sale = await _db.SaleEvents.FirstOrDefaultAsync(se => se.Id == saleEventId);
            if (sale == null) return false;

            _db.SaleEvents.Remove(sale);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}
