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
    public class AuditsController : ControllerBase
    {
        private readonly LtmsContext _context;

        public AuditsController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/Audits
        [HttpGet("GetAllAudits")]
        public async Task<ActionResult<IEnumerable<Audit>>> GetAudits()
        {
          if (_context.Audits == null)
          {
              return NotFound();
          }
            return await _context.Audits.ToListAsync();
        }

        // GET: api/Audits/5
        [HttpGet("GetAudit")]
        public async Task<ActionResult<Audit>> GetAudit(int id)
        {
          if (_context.Audits == null)
          {
              return NotFound();
          }
            var audit = await _context.Audits.FindAsync(id);

            if (audit == null)
            {
                return NotFound();
            }

            return audit;
        }

        // PUT: api/Audits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutAudit")]
        public async Task<IActionResult> PutAudit(int id, Audit audit)
        {
            if (id != audit.Id)
            {
                return BadRequest();
            }

            _context.Entry(audit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuditExists(id))
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

        // POST: api/Audits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostAudit")]
        public async Task<ActionResult<Audit>> PostAudit(Audit audit)
        {
          if (_context.Audits == null)
          {
              return Problem("Entity set 'LtmsContext.Audits'  is null.");
          }

            var chauffeur = await _context.Chauffeurs.FirstOrDefaultAsync(a => a.Nom == audit.PersonneAuditee);
            if (chauffeur == null)
            {
                return BadRequest("Chauffeur not found.");
            }

            audit.PersonneAuditeeNavigation = chauffeur;

            _context.Audits.Add(audit);
           
             await _context.SaveChangesAsync();
           

            return CreatedAtAction("GetAudit", new { id = audit.Id }, audit);
        }

        // DELETE: api/Audits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAudit(int id)
        {
            if (_context.Audits == null)
            {
                return NotFound();
            }
            var audit = await _context.Audits.FindAsync(id);
            if (audit == null)
            {
                return NotFound();
            }

            _context.Audits.Remove(audit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuditExists(int id)
        {
            return (_context.Audits?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
