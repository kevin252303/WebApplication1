using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<MemberDTO>> Register(Registerdto registerdto)
        {
            if (await UserExist(registerdto.UserName))
            {
                return BadRequest("Username is taken");
            }
  
            using var hmac = new HMACSHA512();

            var user = new AppUsers
            {
                Name = registerdto.UserName.ToLower(),
                password = registerdto.Password,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<MemberDTO>> Login(Logindto loginDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Name == loginDTO.username);

            if (user == null)
            {
                return Unauthorized("invalid username");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computehas = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.password));

            for (int i = 0; i < computehas.Length; i++)
            {
                if (computehas[i] != user.PasswordHash[i]) return Unauthorized("Invalid password"); 
            }
            return Ok(user);
        }

        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.Name == username.ToLower());
        }
    }
}
