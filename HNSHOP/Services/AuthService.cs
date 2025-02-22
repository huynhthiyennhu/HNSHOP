using HNSHOP.Data;
using HNSHOP.Dtos.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HNSHOP.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> LoginAsync(LoginReqDto loginReq)
        {
            var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == loginReq.Email);
            if (account == null || !BCrypt.Net.BCrypt.Verify(loginReq.Password, account.Password))
                return false;

            if (!account.IsVerified)
                return false;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.RoleId == 2 ? "Shop" : "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return true;
        }
    }
}
