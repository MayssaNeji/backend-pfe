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
    public class ShiftsController : ControllerBase
    {
        private readonly LtmsContext _context;

        public ShiftsController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/Shifts
        [HttpGet("GetAllShifts")]
        public async Task<ActionResult<IEnumerable<Shift>>> GetShifts()
        {
          if (_context.Shifts == null)
          {
              return NotFound();
          }
            return await _context.Shifts.ToListAsync();
        }

        // GET: api/Shifts/5
        [HttpGet("GetShift")]
        public async Task<ActionResult<Shift>> GetShift(string Nom)
        {
            
            {
                if (_context.Shifts == null)
                {
                    return NotFound();
                }
                var Shift = await _context.Shifts.FirstOrDefaultAsync(s => s.ReferenceShift == Nom);

                if (Shift == null)
                {
                    return NotFound();
                }

                return Shift;
            }

        }

        // PUT: api/Shifts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutShift")]
        public async Task<IActionResult> PutShift(string Nom, [FromBody] Shift shift)
          
        
        {
            if (shift == null || shift.ReferenceShift != Nom)
            {
                return BadRequest();
            }

            var existingShift = await _context.Shifts.FirstOrDefaultAsync(s => s.ReferenceShift == Nom);
            if (existingShift == null)
            {
                return NotFound("shift existe déjà");
            }

            // Update the properties of the existing segment

            existingShift.Lundi= shift.Lundi;
            existingShift.Mardi = shift.Mardi;
            existingShift.Mercredi = shift.Mercredi;
            existingShift.Jeudi = shift.Jeudi;
            existingShift.Vendredi = shift.Vendredi;
            existingShift.Samedi = shift.Samedi;
            existingShift.Dimanche = shift.Dimanche;

           
                _context.Entry(existingShift).State = EntityState.Modified;
                await _context.SaveChangesAsync();
           
            return NoContent();
        }


        // POST: api/Shifts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostShift")]
        public async Task<ActionResult<Shift>> PostShift(Shift shift)
        {
          if (_context.Shifts == null)
          {
              return Problem("Entity set 'LtmsContext.Shifts'  is null.");
          }
            _context.Shifts.Add(shift);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ShiftExists(shift.ReferenceShift))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetShift", new { id = shift.ReferenceShift }, shift);
        }

        // DELETE: api/Shifts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShift(string id)
        {
            if (_context.Shifts == null)
            {
                return NotFound();
            }
            var shift = await _context.Shifts.FindAsync(id);
            if (shift == null)
            {
                return NotFound();
            }

            _context.Shifts.Remove(shift);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShiftExists(string id)
        {
            return (_context.Shifts?.Any(e => e.ReferenceShift == id)).GetValueOrDefault();
        }
    }
}
