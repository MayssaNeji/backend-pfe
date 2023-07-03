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
    public class VehiculesController : ControllerBase
    {
        private readonly LtmsContext _context;

        public VehiculesController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/Vehicules
        [HttpGet("GetAllVehicules")]
        public async Task<ActionResult<IEnumerable<Vehicule>>> GetVehicules()
        {
          if (_context.Vehicules == null)
          {
              return NotFound();
          }
            return await _context.Vehicules.ToListAsync();
        }

        // GET: api/Vehicules/5
        [HttpGet("GetVehicule")]
        public async Task<ActionResult<Vehicule>> GetVehicule(int id)
        {
          if (_context.Vehicules == null)
          {
              return NotFound();
          }
            var vehicule = await _context.Vehicules.FindAsync(id);

            if (vehicule == null)
            {
                return NotFound();
            }

            return vehicule;
        }

        // PUT: api/Vehicules/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutVehicule")]
        public async Task<IActionResult> PutVehicule(int id, Vehicule vehicule)
        {
            if (id != vehicule.Id)
            {
                return BadRequest();
            }

            _context.Entry(vehicule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehiculeExists(id))
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

        // POST: api/Vehicules
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostVehicule")]
        public async Task<ActionResult<Vehicule>> PostVehicule(Vehicule vehicule)
        {
          if (_context.Vehicules == null)
          {
              return Problem("Entity set 'LtmsContext.Vehicules'  is null.");
          }

            var existingVehicule = await _context.Vehicules.FirstOrDefaultAsync(c => c.NomDeReference== vehicule.NomDeReference || c.NumSerie == vehicule.NumSerie);
            if (existingVehicule != null)
            {
                return BadRequest("Vehicule existe déjà");
            }
            else
            {
                _context.Vehicules.Add(vehicule);
            await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetVehicule", new { id = vehicule.Id }, vehicule);
        }

        // DELETE: api/Vehicules/5
        [HttpDelete("DeleteVehicule")]
        public async Task<IActionResult> DeleteVehicule(int id)
        {
            if (_context.Vehicules == null)
            {
                return NotFound();
            }
            var vehicule = await _context.Vehicules.FindAsync(id);
            if (vehicule == null)
            {
                return NotFound();
            }

            _context.Vehicules.Remove(vehicule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VehiculeExists(int id)
        {
            return (_context.Vehicules?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
