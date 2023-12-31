using ChattingApp.Data;
using ChattingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChattingApp.Controllers
{
    public class BuggyController:BaseApiController
    {
        private readonly AppDbContext _context;

        public BuggyController(AppDbContext context)
        {
            _context = context;
        }

        



        //[HttpGet("badrequest/{id}")]
        //public IActionResult GetNotFoundRequest(int id)
        //{
        //    return Ok();
        //}


        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret(int id)
        {
            return "Secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound(int id)
        {
            var thing = _context.AppUsers.Find(-1);
            if (thing == null) return NotFound();
            return thing;
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var thing = _context.AppUsers.Find(50);
            var thingToReturn = thing.ToString();
            return thingToReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("Bad request");
        }
    }
}

