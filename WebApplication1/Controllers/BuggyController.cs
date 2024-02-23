using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUsers> GetNotFound()
        {
            try
            {
                var thing = _context.Users.Find(-1);

                if (thing == null) return NotFound();
                return Ok(thing);
            }
            catch (Exception ex)
            {
                return StatusCode(404, "Not found");
            }
            
            
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            
                var thing = _context.Users.Find(-1);

                var thingToReturn = thing?.ToString();

                return thingToReturn;
            
            

        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This is bad request");
        }

        
    }
}
