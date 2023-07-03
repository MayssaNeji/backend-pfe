using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTMS.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace LTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChauffeursController : ControllerBase
    {
        private readonly LtmsContext _context;

        public ChauffeursController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/Chauffeurs
        [HttpGet("GetAllChauffeurs")]
        public async Task<ActionResult<IEnumerable<Chauffeur>>> GetAllChauffeurs()
        {
          if (_context.Chauffeurs == null)
          {
              return NotFound();
          }
            return await _context.Chauffeurs.ToListAsync();
        }

        // GET: api/Chauffeurs/5
        [HttpGet("search")]
        public async Task<ActionResult<Chauffeur>> GetChauffeur(int id)
        {
            var chauffeur = await _context.Chauffeurs
                .Include(c => c.AgenceNavigation)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chauffeur == null)
            {
                return NotFound();
            }

            return Ok(chauffeur);
        }


        // PUT: api/Chauffeurs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutChauffeur")]
        public async Task<IActionResult> PutChauffeur(int id,[FromBody] Chauffeur chauffeur)
        {
            if (id != chauffeur.Id)
            {
                return BadRequest();
            }

            _context.Entry(chauffeur).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChauffeurExists(id))
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

        // POST: api/Chauffeurs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostChauffeur")]
        public async Task<ActionResult<Chauffeur>> PostChauffeur(Chauffeur chauffeur)
        {
            var existingChauffeur = await _context.Chauffeurs.FirstOrDefaultAsync(c => c.Nom == chauffeur.Nom && c.Prenom == chauffeur.Prenom && c.Agence == chauffeur.Agence);
            if (existingChauffeur != null)
            {
                return BadRequest("Chauffeur existe déjà");
            }
            else
            {
                _context.Chauffeurs.Add(chauffeur);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetChauffeur", new { id = chauffeur.Id }, chauffeur);
        }


        // DELETE: api/Chauffeurs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChauffeur(int id)
        {
            if (_context.Chauffeurs == null)
            {
                return NotFound();
            }
            var chauffeur = await _context.Chauffeurs.FindAsync(id);
            if (chauffeur == null)
            {
                return NotFound();
            }

            _context.Chauffeurs.Remove(chauffeur);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChauffeurExists(int id)
        {
            return (_context.Chauffeurs?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}
