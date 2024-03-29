﻿using AutoMapper;
using ChattingApp.Data;
using ChattingApp.DTOs;
using ChattingApp.Extensions;
using ChattingApp.Interfaces;
using ChattingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChattingApp.Controllers
{

    public class AccountController:BaseApiController
    {
        //private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper mapper;
        private readonly IMessageRepository _messageRepository;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper,IMessageRepository messageRepository)
        {
            _userManager = userManager;
            //_context = context;
            _tokenService = tokenService;
            this.mapper = mapper;
            _messageRepository = messageRepository;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody]RegisterDto registerDto)
        { 
            if(await UserExists(registerDto.UserName)) return BadRequest("username is taken");
            var user = mapper.Map<AppUser>(registerDto);

          //using var hmac=new HMACSHA512();

            user.UserName = registerDto.UserName.ToLower();
            //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
           // user.PasswordSalt = hmac.Key;

            
            //_context.AppUsers.Add(user);
            //await _context.SaveChangesAsync();


            var result= await _userManager.CreateAsync(user,registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAS = user.KnownAs
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                .Include(p=>p.Photos)
                .SingleOrDefaultAsync(x=> x.UserName==loginDto.UserName);  

            if(user==null) return Unauthorized("invalid username");

            var result= await _userManager.CheckPasswordAsync(user,loginDto.Password);
            if(!result) return Unauthorized("Invalid PAssword");

            //using var hmac = new HMACSHA512(user.PasswordSalt);
            //var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            //for (int i = 0; i < ComputedHash.Length; i++)

            
            //    if (ComputedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid Password");

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAS = user.KnownAs,
                

            };

        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();
            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }

            private async Task<bool> UserExists(string username)
            {
            return await _userManager.Users.AnyAsync(x=> x.UserName == username);
            }
    }
}
