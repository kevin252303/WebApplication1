using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Extentions;
using WebApplication1.Helpers;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class UserController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public IPhotoService PhotoService { get; }

        public UserController(IUserRepository userRepository, IMapper mapper,
            IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery]UserParams userParams)
        {
            var user = await _userRepository.GetMembersAsync(userParams);
            Response.AppPaginationHeader(new PaginationHeader(user.CurrentPage, user.PageSize, user.TotalCount,user.TotalPages));
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        {
            var user = await _userRepository.GetUserByNameAsync(username);
            return _mapper.Map<MemberDTO>(user);
        }


        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDto)
        {
            var user = await _userRepository.GetUserByNameAsync(User.GetUserName());
            
            if (user == null) return NotFound();

            _mapper.Map(memberUpdateDto, user);

            if(await _userRepository.saveAllAsyns()) return NoContent();

            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByNameAsync(User.GetUserName());

            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if(user.Photos.Count == 0) photo.IsMain = true;

            user.Photos.Add(photo);

            if(await _userRepository.saveAllAsyns())
            {
                return CreatedAtAction(nameof(GetUser), new {username = user.UserName},_mapper.Map<PhotoDTO>(photo));
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult>SetMainPhoto(int photoid)
        {
            var user = await _userRepository.GetUserByNameAsync(User.GetUserName());

            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x=>x.Id == photoid);

            if (photo == null) return NotFound();

            if (photo.IsMain) return BadRequest("this is already a main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if(currentMain == null)
            {
                currentMain.IsMain = false;
                photo.IsMain = true;
            }

            if (await _userRepository.saveAllAsyns()) return NoContent();

            return BadRequest("Problem setting main photo");
        }
    }
}
