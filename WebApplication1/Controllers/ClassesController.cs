using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ClassesController : BaseApiController
    {
        private readonly Context _context;

        public ClassesController(Context context)
        {
            _context = context;
        }

        [Route("getall")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Class>>> GetClass()
        {
            return await _context.Class.ToListAsync();
        }

        [Route("getbyid/{id}")]
        [HttpGet]
        public async Task<ActionResult<Class>> GetClass(long id)
        {
            var @class = await _context.Class.FindAsync(id);

            if (@class == null)
            {
                return NotFound();
            }

            return @class;
        }


        [Route("Update/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutClass(long id, Class @class)
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
        public async Task<ActionResult<Class>> PostClass(Class @class)
        {
            _context.Class.Add(@class);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClass", new { id = @class.Id }, @class);
        }

        [Route("BulkInsert")]
        [HttpPost]
        public async Task<ActionResult<List<Class>>> BulkInsert(List<Class> classes)
        {
            _context.Class.AddRange(classes);
            await _context.SaveChangesAsync();

            return Ok(classes);
        }

        [Route("Delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteClass(long id)
        {
            var @class = await _context.Class.FindAsync(id);
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
                _context.Class.RemoveRange(_context.Class);
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
            return _context.Class.Any(e => e.Id == id);
        }
    }
}
