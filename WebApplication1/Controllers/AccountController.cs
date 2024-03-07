using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        //private readonly ITokenServise _tokenServise;
        private readonly IMapper _mapper;
        private readonly ITokenServise _tokenServise;

        public AccountController(DataContext context, IMapper mapper, ITokenServise tokenServise)
        {
            _context = context;
            //_tokenServise = tokenServise;
            _mapper = mapper;
            _tokenServise = tokenServise;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(Registerdto registerdto)
        {
            if (await UserExist(registerdto.UserName))
            {
                return BadRequest("Username is taken");
            }

            var user = _mapper.Map<AppUsers>(registerdto);

            using var hmac = new HMACSHA512();


            user.UserName = registerdto.UserName.ToLower();
            user.password = registerdto.Password;              
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.Password));
            user.PasswordSalt = hmac.Key;
           

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Token = _tokenServise.CreateToken(user),
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(Logindto loginDTO)
        {
            var user = await _context.Users
                .Include(p=>p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDTO.userName);

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
            return new UserDTO
            {
                Username = user.UserName,
                PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain).Url,
                KnownAs = user.KnownAs,
                Token = _tokenServise.CreateToken(user),
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
