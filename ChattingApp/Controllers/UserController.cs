using AutoMapper;
using ChattingApp.Data;
using ChattingApp.DTOs;
using ChattingApp.Extensions;
using ChattingApp.Interfaces;
using ChattingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChattingApp.Controllers
{
    [Authorize]
    public class UserController : BaseApiController
    {
        
        private readonly IUserRepository _userRepository; 
        private readonly IMapper _mapper;
        private readonly IPhotoService photoService;

        public UserController( IUserRepository userRepository, IMapper mapper,IPhotoService photoService)
        {
            
            _userRepository = userRepository;
            _mapper = mapper;
            this.photoService = photoService;
        }
        [HttpGet]
        public async Task< ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {

            var users = await _userRepository.GetMembersAsync();
            
            return Ok(users);
        }
       
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
            
            
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            
            var user=await _userRepository.GetUSerByUsernameAsync(User.GetUsername());
            if (user == null)
            {
                return NotFound();
            }
            _mapper.Map(memberUpdateDto, user);
            if(await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUSerByUsernameAsync(User.GetUsername());
            if (user == null) { return NotFound(); }
            var result=await photoService.AddPhotoASync(file);
            if(result.Error !=null) { return BadRequest(result.Error.Message); }
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0) photo.IsMain = true;
            user.Photos.Add(photo);
            if(await _userRepository.SaveAllAsync()) return _mapper.Map<PhotoDto>(photo);
            return BadRequest("Problem adding photo");
        }
    }
}
