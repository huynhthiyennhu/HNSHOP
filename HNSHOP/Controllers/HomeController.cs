using HNSHOP.Data;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Net.Mail;
using System.Net;

namespace HNSHOP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public HomeController(ApplicationDbContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(int? productTypeId)
        {
            var productsQuery = _db.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Shop)
                .AsQueryable();

            if (productTypeId.HasValue && productTypeId > 0)
            {
                productsQuery = productsQuery.Where(p => p.ProductTypeId == productTypeId);
            }
            

            var products = await productsQuery.ToListAsync();
            var productDtos = _mapper.Map<List<ProductResDto>>(products);

            var productTypes = await _db.ProductTypes.ToListAsync();
            ViewBag.ProductTypes = _mapper.Map<List<ProductTypeResDto>>(productTypes);






            return View(productDtos);
        }

        public async Task Test()
        {
            //Handle register

            //Random number
            int test = RandomInt();

            string mail = "abc.com";
            SendMail(mail, test);

        }

        private int RandomInt ()
        {
            return 12355;
        }

        private async Task SendMail(string mailTo, int code)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.mailersend.net")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("MS_Ujklt4@trial-jpzkmgq1x6vl059v.mlsender.net", "mssp.hCbe59r.0p7kx4xq9vmg9yjr.SrBjeiJ"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("MS_Ujklt4@trial-jpzkmgq1x6vl059v.mlsender.net"),
                    Subject = "This is subject",
                    Body = $"<h1>{code}</h1>     <a>localhost:1234/verify?user=123123123&code={code}</a>",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(mailTo);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
