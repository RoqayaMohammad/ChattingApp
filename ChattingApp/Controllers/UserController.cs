﻿using ChattingApp.Data;
using ChattingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChattingApp.Controllers
{
    [Authorize]
    public class UserController : BaseApiController
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;

        }
        [HttpGet]
        public async Task< ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
           
            return await _context.AppUsers.ToListAsync();
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            
            return await _context.AppUsers.FindAsync(id);
        }
    }
}
