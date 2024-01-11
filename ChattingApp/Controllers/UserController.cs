using AutoMapper;
using ChattingApp.Data;
using ChattingApp.DTOs;
using ChattingApp.Extensions;
using ChattingApp.Helpers;
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
        //[AllowAnonymous]
        [HttpGet]
        public async Task< ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {

            var users = await _userRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.ToltalPages));
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

            var user = await _userRepository.GetUSerByUsernameAsync(User.GetUsername());
            if (user == null)
            {
                return NotFound();
            }
            _mapper.Map(memberUpdateDto, user);
            if (await _userRepository.SaveAllAsync())
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
                PublicId = result.PublicId,
                
            };

            if (user.Photos.Count == 0) photo.IsMain = true;
            user.Photos.Add(photo);
            if(await _userRepository.SaveAllAsync()) return CreatedAtAction(nameof(GetUser), new { username = user.UserName },_mapper.Map<PhotoDto>(photo));
            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user =await _userRepository.GetUSerByUsernameAsync(User.GetUsername());
            if (user == null) { return NotFound();}
            var photo=  user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if (photo == null)  return NotFound();
            if (photo.IsMain) return BadRequest("this is already yoyr main photo");
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if(currentMain!=null) currentMain.IsMain = false;
            photo.IsMain=true;
            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("problem setting the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUSerByUsernameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if (photo == null) return NotFound();
            if(photo.IsMain) return BadRequest("you cannot delete your main photo");
            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoASync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await _userRepository.SaveAllAsync()) return Ok  ();

            return BadRequest("Problem Deleting Photo");
            
        }

    }
}
