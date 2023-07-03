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
    public class AgencesController : ControllerBase
    {
        private readonly LtmsContext _context;

        public AgencesController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/Agences
        [HttpGet("GetAllAgences")]
        public async Task<ActionResult<IEnumerable<Agence>>> GetAllAgences()
        {
          if (_context.Agences == null)
          {
              return NotFound();
          }
            return await _context.Agences.ToListAsync();
        }

        // GET: api/Agences/5
        [HttpGet("GetAgence")]
        public async Task<ActionResult<Agence>> GetAgence(string id)
        {
          if (_context.Agences == null)
          {
              return NotFound();
          }
            var agence = await _context.Agences.FindAsync(id);

            if (agence == null)
            {
                return NotFound();
            }

            return agence;
        }

        // PUT: api/Agences/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutAgence")]
        public async Task<ActionResult> PutAgence(int id, Agence updatedAgence)
        {
            if (id != updatedAgence.Id)
            {
                return BadRequest("Agence Introuvable");
            }

            var existingAgence = await _context.Agences.FindAsync(id);
            if (existingAgence == null)
            {
                return NotFound();
            }

            // Update the 'Nom' property of the existing entity
            existingAgence.Nom = updatedAgence.Nom;

            // Save the changes
            await _context.SaveChangesAsync();

            return Ok("updated");
        }


        // POST: api/Agences
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostAgence")]
        public async Task<ActionResult<Agence>> PostAgence(Agence agence)
        {


            var existingAgency = await _context.Agences.FirstOrDefaultAsync(c => c.Nom == agence.Nom);

            if (existingAgency!= null)
            {
                return BadRequest("Agence deja existante");
            }

            _context.Agences.Add(agence);
             await _context.SaveChangesAsync();
           

            return CreatedAtAction("GetAgence", new { id = agence.Id }, agence);
        }

        // DELETE: api/Agences/5
        [HttpDelete("DeleteAgence")]
        public async Task<IActionResult> DeleteAgence(string id)
        {
            if (_context.Agences == null)
            {
                return NotFound();
            }
            var chauffeur = await _context.Agences.FindAsync(id);
            if (chauffeur == null)
            {
                return NotFound();
            }

            _context.Agences.Remove(chauffeur);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    
    




    private bool AgenceExists(int id)
        {
            return (_context.Agences?.Any(e => e.Id == id)).GetValueOrDefault();
        }

     
    }
}
