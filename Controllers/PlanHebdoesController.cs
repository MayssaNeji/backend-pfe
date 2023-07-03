using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTMS.Models;
using System.Linq;
using OfficeOpenXml.Packaging;

namespace LTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanHebdoesController : ControllerBase
    {
        private readonly LtmsContext _context;

        public PlanHebdoesController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/PlanHebdoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanHebdo>>> GetPlanHebdos()
        {
            var planHebdos = await _context.PlanHebdos.ToListAsync();
            if (planHebdos == null || planHebdos.Count == 0)
            {
                return NotFound();
            }
            return planHebdos;
        }

        // GET: api/PlanHebdoes/5
        [HttpGet("GetPlanHebdo")]
        public async Task<ActionResult<PlanHebdo>> GetPlanHebdo(Guid id)
        {
            var planHebdo = await _context.PlanHebdos.FindAsync(id);
            if (planHebdo == null)
            {
                return NotFound();
            }
            return planHebdo;
        }

        // PUT: api/PlanHebdoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutPlanHebdo")]
        public async Task<IActionResult> PutPlanHebdo(Guid id, PlanHebdo planHebdo)
        {
            if (id != planHebdo.Id)
            {
                return BadRequest();
            }

            _context.Entry(planHebdo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanHebdoExists(id))
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

        // POST: api/PlanHebdoes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostPlanHebdo")]
        public async Task<ActionResult<List<PlanHebdo>>> PostPlanHebdo(int targetAnnee, int targetMois, string targetRefSemaine)
        {
            var matchingPlanSegments = await _context.PlanSegments
                .Where(ps =>
                    ps.Annee == targetAnnee &&
                    ps.Mois == targetMois &&
                    ps.RefSemaine == targetRefSemaine)
                .ToListAsync();

            var planHebdos = new List<PlanHebdo>();


            foreach (var planSegment in matchingPlanSegments)
            {
                var employe = await _context.Employes.FirstOrDefaultAsync(e => e.Matricule == planSegment.Matricule);

                var circuit = await _context.Circuits.FirstOrDefaultAsync(c => c.RefSapLeoni == employe.Station);

                if (employe != null)
                {
                    if (circuit != null)
                    {
                        var planHebdo = new PlanHebdo
                        {
                            Id = Guid.NewGuid(),
                            Matricule = planSegment.Matricule ?? 0,
                            Nom = planSegment.Nom ?? "",
                            Prenom = planSegment.Prenom ?? "",
                            Segment = planSegment.Segment ?? "",
                            Circuit = circuit.RefChemin ?? "", // Assuming Circuit property cannot be null
                            Station = employe.Station ?? "",
                            Ps = employe.Ps ?? "",
                            Lundi = planSegment.Lundi ?? "",
                            Mardi = planSegment.Mardi ?? "",


                            Annee = planSegment.Annee.HasValue ? planSegment.Annee.Value.ToString() : "",
                            Mois = planSegment.Annee.HasValue ? planSegment.Mois.Value.ToString() : "",

                            Mercredi = planSegment.Mercredi ?? "",
                            Jeudi = planSegment.Jeudi ?? "",
                            Vendredi = planSegment.Vendredi ?? "",
                            Samedi = planSegment.Samedi ?? "",
                            Dimanche = planSegment.Dimanche ?? "",
                            RefSemaine = planSegment.RefSemaine,
                            Shift = planSegment.Shift ?? ""
                        };

                        if (employe.Ps == "LTN1")
                        {
                            planHebdo.Organization = "LEONI sousse";
                        }
                        else if (employe.Ps == "LTN2")
                        {
                            planHebdo.Organization = "LEONI Mateur Nord";
                        }
                        else if (employe.Ps == "LTN3")
                        {
                            planHebdo.Organization = "LEONI Sousse";
                        }
                        else if (employe.Ps == "LTN4")
                        {
                            planHebdo.Organization = "LEONI Sousse";
                        }
                        else if (employe.Ps == "LTN5")
                        {
                            planHebdo.Organization = "LEONI Mateur Sud";
                        }
                        else if (employe.Ps == "LTN6")
                        {
                            planHebdo.Organization = "LEONI Menzel Hayet";
                        }

                        planHebdos.Add(planHebdo);
                    }
                    else
                    {
                        return BadRequest("circuittttt");
                        // Handle case when circuit is not found for the employe station
                        // You can log an error, skip the record, or handle it as needed
                    }
                }
                else
                {
                    return BadRequest("employee");
                    // Handle case when employe is not found for the matricule
                    // You can log an error, skip the record, or handle it as needed
                }
            }

            _context.PlanHebdos.AddRange(planHebdos);
            await _context.SaveChangesAsync();

            return Ok(planHebdos);
        }

        // DELETE: api/PlanHebdoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlanHebdo(Guid id)
        {
            var planHebdo = await _context.PlanHebdos.FindAsync(id);
            if (planHebdo == null)
            {
                return NotFound();
            }

            _context.PlanHebdos.Remove(planHebdo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlanHebdoExists(Guid id)
        {
            return _context.PlanHebdos?.Any(e => e.Id == id) ?? false;
        }
    }
}
