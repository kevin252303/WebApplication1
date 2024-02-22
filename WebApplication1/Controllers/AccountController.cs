using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly Context _context;

        public AccountController(Context context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Class>> Register(Registerdto registerdto)
        {
            if (await UserExist(registerdto.UserName))
            {
                return BadRequest("Username is taken");
            }
  
            using var hmac = new HMACSHA512();

            var user = new Class
            {
                Name = registerdto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Class.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Class>> Login(Logindto loginDTO)
        {
            var user = await _context.Class.SingleOrDefaultAsync(x => x.Name == loginDTO.username);

            if (user == null)
            {
                return Unauthorized("invalid username");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computehas = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.password));

            for(int i = 0; i < computehas.Length; i++)
            {
                if (computehas[i] != user.PasswordHash[i]) return Unauthorized("Invalid password"); 
            }
            return user;
        }

        private async Task<bool> UserExist(string username)
        {
            return await _context.Class.AnyAsync(x => x.Name == username.ToLower());
        }
    }
}
