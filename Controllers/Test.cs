using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTMS.Models;

namespace LTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Test : ControllerBase
    {
        private readonly LtmsContext _context;

        public Test(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/Test
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Véhicule>>> GetVéhicule()
        {
          if (_context.Véhicule == null)
          {
              return NotFound();
          }
            return await _context.Véhicule.ToListAsync();
        }

        // GET: api/Test/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Véhicule>> GetVéhicule(int id)
        {
          if (_context.Véhicule == null)
          {
              return NotFound();
          }
            var véhicule = await _context.Véhicule.FindAsync(id);

            if (véhicule == null)
            {
                return NotFound();
            }

            return véhicule;
        }

        // PUT: api/Test/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVéhicule(int id, Véhicule véhicule)
        {
            if (id != véhicule.Id)
            {
                return BadRequest();
            }

            _context.Entry(véhicule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VéhiculeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Test
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Véhicule>> PostVéhicule(Véhicule véhicule)
        {
          if (_context.Véhicule == null)
          {
              return Problem("Entity set 'LtmsContext.Véhicule'  is null.");
          }
            _context.Véhicule.Add(véhicule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVéhicule", new { id = véhicule.Id }, véhicule);
        }

        // DELETE: api/Test/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVéhicule(int id)
        {
            if (_context.Véhicule == null)
            {
                return NotFound();
            }
            var véhicule = await _context.Véhicule.FindAsync(id);
            if (véhicule == null)
            {
                return NotFound();
            }

            _context.Véhicule.Remove(véhicule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VéhiculeExists(int id)
        {
            return (_context.Véhicule?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
