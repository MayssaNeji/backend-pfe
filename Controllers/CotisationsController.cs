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
    public class CotisationsController : ControllerBase
    {
        private readonly LtmsContext _context;

        public CotisationsController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/Cotisations
        [HttpGet("GetCotisations")]
        public async Task<ActionResult<IEnumerable<Cotisation>>> GetCotisations()
        {
          if (_context.Cotisations == null)
          {
              return NotFound();
          }
            return await _context.Cotisations.ToListAsync();
        }

        // GET: api/Cotisations/5
        [HttpGet("GetCotisation")]
        public async Task<ActionResult<Cotisation>> GetCotisation(int id)
        {
          if (_context.Cotisations == null)
          {
              return NotFound();
          }
            var cotisation = await _context.Cotisations.FindAsync(id);

            if (cotisation == null)
            {
                return NotFound();
            }

            return cotisation;
        }

        // PUT: api/Cotisations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutCotisation")]
        public async Task<IActionResult> PutCotisation(int id, Cotisation cotisation)
        {
            if (id != cotisation.Id)
            {
                return BadRequest();
            }

            _context.Entry(cotisation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CotisationExists(id))
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

        // POST: api/Cotisations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostCotisation")]
        public async Task<ActionResult<Cotisation>> PostCotisation(string Annee, string Mois, string Ps)
        {
            if (_context.Cotisations == null)
            {
                return Problem("Entity set 'LtmsContext.Cotisations' is null.");
            }

            var planSegments = await _context.PlanHebdos
                .Where(p => p.Annee == Annee && p.Mois == Mois && p.Ps == Ps)
                .ToListAsync();

            if (planSegments.Count == 0)
            {
                return NotFound("Il n'ya pas une planification de segment correspondante à ces paramétres pour calculer les cotisations");
            }

            var segmentList = planSegments.Select(p => p.Segment).ToList();

            var employes = await _context.Employes
                .Where(e => segmentList.Contains(e.Segment))
                .ToListAsync();

            if (employes.Count == 0)
            {
                return NotFound("No employees found.");
            }

            var cotisations = new List<Cotisation>();

            foreach (var employe in employes)
            {
                var cotisation = new Cotisation
                {
                    Ps = Ps,
                    Annee = Annee,
                    Mois = Mois,
                    Employe = employe.Matricule.ToString(),
                    Segment = employe.Segment
                };

                // Set organization based on employe.Ps value
                switch (employe.Ps)
                {
                    case "LTN1":
                        cotisation.Organization = "LEONI sousse";
                        break;
                    case "LTN2":
                        cotisation.Organization = "LEONI Mateur Nord";
                        break;
                    case "LTN3":
                    case "LTN4":
                        cotisation.Organization = "LEONI Sousse";
                        break;
                    case "LTN5":
                        cotisation.Organization = "LEONI Mateur Sud";
                        break;
                    case "LTN6":
                        cotisation.Organization = "LEONI Menzel Hayet";
                        break;
                }

                var circuit = await _context.Circuits.FirstOrDefaultAsync(c => c.RefSapLeoni == employe.Station);
                cotisation.Circuit = circuit.RefChemin;

                if (circuit == null)
                {
                    return NotFound("Circuit not found.");
                }

                int? multiplicationResult = circuit.NbKm * circuit.CoutKm;
                cotisation.Cotisation1 = (multiplicationResult * 0.2).ToString();

                cotisations.Add(cotisation);
            }

            _context.Cotisations.AddRange(cotisations);
            await _context.SaveChangesAsync();

            return Ok(cotisations);
        }



        // DELETE: api/Cotisations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCotisation(int id)
        {
            if (_context.Cotisations == null)
            {
                return NotFound();
            }
            var cotisation = await _context.Cotisations.FindAsync(id);
            if (cotisation == null)
            {
                return NotFound();
            }

            _context.Cotisations.Remove(cotisation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CotisationExists(int id)
        {
            return (_context.Cotisations?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
