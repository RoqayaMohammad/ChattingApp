using ChattingApp.Data;
using ChattingApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ChattingApp.Controllers
{
    public class AccountController:BaseApiController
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
            _context = context;

        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> register(string username,string password)
        { 
          using var hmac=new HMACSHA512();
            var user = new AppUser
            {
                UserName = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key

            };
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
    }
}
