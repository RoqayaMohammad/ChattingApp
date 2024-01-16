using ChattingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChattingApp.Controllers
{
    
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> userMaanger;

        public AdminController(UserManager<AppUser> userMaanger) 
        {
            this.userMaanger = userMaanger;
        }


        [Authorize(Policy ="RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
           var users=await userMaanger.Users
                .OrderBy(u=>u.UserName)
                .Select(u=> new
                {
                    u.Id,
                    Username=u.UserName,
                    Roles=u.UserRoles.Select(r=>r.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);

        }

        [Authorize(Policy="RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
            if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");
            var selectedRoles=roles.Split(',').ToArray();
            var user= await userMaanger.FindByNameAsync(username);
            if (user == null) return NotFound();

            var userRoles = await userMaanger.GetRolesAsync(user);
            var result= await userMaanger.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("Failed to remove from roles");
            return Ok(await userMaanger.GetRolesAsync(user));
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration() {
            return Ok("");
             
        }
    }
}
