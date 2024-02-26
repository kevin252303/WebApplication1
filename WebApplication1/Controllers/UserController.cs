using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    
    public class UserController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, DataContext context, IMapper mapper)
        {
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUsers>>> GetUsers()
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


        [Route("Update/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutClass(long id, AppUsers @class)
        {
            if (id != @class.Id)
            {
                return BadRequest();
            }

            _context.Entry(@class).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClassExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Record updated succesfully..");
        }

        [Route("Insert")]
        [HttpPost]
        public async Task<ActionResult<AppUsers>> PostClass(AppUsers @class)
        {
            _context.Users.Add(@class);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClass", new { id = @class.Id }, @class);
        }

        [Route("BulkInsert")]
        [HttpPost]
        public async Task<ActionResult<List<AppUsers>>> BulkInsert(List<AppUsers> classes)
        {
            _context.Users.AddRange(classes);
            await _context.SaveChangesAsync();

            return Ok(classes);
        }

        [Route("Delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteClass(long id)
        {
            var @class = await _context.Users.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }

            _context.Remove(@class);
            await _context.SaveChangesAsync();

            return Ok(@class);
        }

        [Route("Deleteall")]
        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            try
            {
                _context.Users.RemoveRange(_context.Users);
                await _context.SaveChangesAsync();

                return Ok("All records deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            
        }

        private bool ClassExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
