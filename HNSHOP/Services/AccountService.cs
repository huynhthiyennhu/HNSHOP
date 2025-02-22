using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using HNSHOP.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _emailService;

        public AccountService(ApplicationDbContext db, IEmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<bool> RegisterAsync(RegisterReqDto registerReq)
        {
            if (await _db.Accounts.AnyAsync(a => a.Email == registerReq.Email))
                return false;

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerReq.Password);
            string verifyToken = GenerateVerificationToken();

            var account = new Account
            {
                Email = registerReq.Email,
                Phone = registerReq.Phone,
                Password = hashedPassword,
                RoleId = 3, // Role User
                VerifyToken = verifyToken,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();

            await _emailService.SendVerificationEmail(registerReq.Email, verifyToken);
            return true;
        }

        public async Task<bool> RegisterShopAsync(RegisterShopReqDto registerShopReq)
        {
            if (await _db.Accounts.AnyAsync(a => a.Email == registerShopReq.Email))
                return false;

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerShopReq.Password);
            string verifyToken = GenerateVerificationToken();

            var account = new Account
            {
                Email = registerShopReq.Email,
                Phone = registerShopReq.Phone,
                Password = hashedPassword,
                RoleId = 2, // Role Shop
                VerifyToken = verifyToken,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();

            await _emailService.SendVerificationEmail(registerShopReq.Email, verifyToken);
            return true;
        }

        public async Task<bool> VerifyEmailAsync(string email, string token)
        {
            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == email && a.VerifyToken == token);
            if (account == null) return false;

            account.IsVerified = true;
            account.VerifiedAt = DateTime.Now;
            account.VerifyToken = null;

            await _db.SaveChangesAsync();
            return true;
        }

        private string GenerateVerificationToken()
        {
            var rng = new Random();
            return rng.Next(100000, 999999).ToString(); // 6 số ngẫu nhiên
        }
    }
}
