using AutoMapper;
using HNSHOP.Data;
using HNSHOP.Dtos.Response;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public class CustomerTypeService : ICustomerTypeService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CustomerTypeService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<CustomerTypeResDto>> GetCustomerTypesAsync()
        {
            var customerTypes = await _db.CustomerTypes.ToListAsync();
            return _mapper.Map<List<CustomerTypeResDto>>(customerTypes);
        }
    }
}
