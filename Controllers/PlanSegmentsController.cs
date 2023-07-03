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
    public class PlanSegmentsController : ControllerBase
    {
        private readonly LtmsContext _context;

        public PlanSegmentsController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/PlanSegments
        [HttpGet("GetAllPlanSegments")]
        public async Task<ActionResult<IEnumerable<PlanSegment>>> GetPlanSegments()
        {
          if (_context.PlanSegments == null)
          {
              return NotFound();
          }
            return await _context.PlanSegments.ToListAsync();
        }

        // GET: api/PlanSegments/5
        [HttpGet("GetPlanSegments")]
        public async Task<ActionResult<PlanSegment>> GetPlanSegment(string id)
        {
          if (_context.PlanSegments == null)
          {
              return NotFound();
          }
            var planSegment = await _context.PlanSegments.FindAsync(id);

            if (planSegment == null)
            {
                return NotFound();
            }

            return planSegment;
        }

        // PUT: api/PlanSegments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutPlanSegment")]
        public async Task<IActionResult> PutPlanSegment(string id, [FromBody] PlanSegment planSegment)
        {
            if (id != planSegment.RefSemaine)
            {
                return BadRequest();
            }

            _context.Entry(planSegment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanSegmentExists(id))
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

        // POST: api/PlanSegments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostPlanSegment")]
        public async Task<ActionResult<PlanSegment>> PostPlanSegment(PlanSegment planSegment )
        {
          if (_context.PlanSegments == null)
          {
              return Problem("Entity set 'LtmsContext.PlanSegments'  is null.");
          }




            var segment = await _context.Segments.FirstOrDefaultAsync(a => a.Nom == planSegment.Segment);
            if (segment == null)
            {
                return BadRequest("segment Introuvable.");
            }

           



            var Employe = await _context.Employes.FirstOrDefaultAsync(a => a.Matricule == planSegment.Matricule);
            if (Employe == null)
            {
                return BadRequest("Employé Introuvable.");
            }

          
            planSegment.Shift = Employe.Shift;


            var Shift = await _context.Shifts.FirstOrDefaultAsync(a => a.ReferenceShift == planSegment.Shift);
            if (Shift == null)
            {
                return BadRequest("Shift Introuvable.");
            }

            planSegment.Lundi = Shift.Lundi;
            planSegment.Mardi= Shift.Mardi;
            planSegment.Mercredi= Shift.Mercredi;
            planSegment.Jeudi= Shift.Jeudi;
            planSegment.Vendredi= Shift.Vendredi;
            planSegment.Samedi= Shift.Samedi;
            planSegment.Dimanche= Shift.Dimanche;




            planSegment.Nom = Employe.Nom;
            planSegment.Prenom = Employe.Prenom;
           





            _context.PlanSegments.Add(planSegment);
            await _context.SaveChangesAsync();
           

            return CreatedAtAction("GetPlanSegment", new { id = planSegment.Id }, planSegment);
        }

        // DELETE: api/PlanSegments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlanSegment(int id)
        {
            if (_context.PlanSegments == null)
            {
                return NotFound();
            }
            var planSegment = await _context.PlanSegments.FindAsync(id);
            if (planSegment == null)
            {
                return NotFound();
            }

            _context.PlanSegments.Remove(planSegment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlanSegmentExists(string id)
        {
            return (_context.PlanSegments?.Any(e => e.RefSemaine == id)).GetValueOrDefault();
        }
    }
}

