using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Extentions;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{

    public class UserController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public IPhotoService PhotoService { get; }

        public UserController(IUserRepository userRepository, DataContext context, IMapper mapper,
            IPhotoService photoService)
        {
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            var user = await _userRepository.getUsersAsyns();
            var usertoreturn = _mapper.Map<IEnumerable<MemberDTO>>(user);
            return Ok(usertoreturn);
        }

        
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
                return CreatedAtAction(nameof(GetUser), new {username = user.Name},_mapper.Map<PhotoDTO>(photo));
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
