using ChattingApp.Data;
using ChattingApp.DTOs;
using ChattingApp.Interfaces;
using ChattingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChattingApp.Controllers
{

    public class AccountController:BaseApiController
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;


        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        { 
            if(await UserExists(registerDto.UserName)) return BadRequest();

          using var hmac=new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key

            };
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.AppUsers.SingleOrDefaultAsync(x=> x.UserName==loginDto.UserName);
            if(user==null) return Unauthorized("invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < ComputedHash.Length; i++)

            {
                if (ComputedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid Password");
}
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

        }

            private async Task<bool> UserExists(string username)
            {
            return await _context.AppUsers.AnyAsync(x=> x.UserName == username);
            }
    }
}
