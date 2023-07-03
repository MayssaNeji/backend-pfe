using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTMS.Models;
using OfficeOpenXml.Packaging;
using Microsoft.AspNetCore.Components.Server.Circuits;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;

namespace LTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanAgencesController : ControllerBase
    {
        private readonly LtmsContext _context;

        public PlanAgencesController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/PlanAgences
        [HttpGet("GetPlanAgences")]
        public async Task<ActionResult<IEnumerable<PlanAgence>>> GetPlanAgences()
        {
          if (_context.PlanAgences == null)
          {
              return NotFound();
          }
            return await _context.PlanAgences.ToListAsync();
        }

        // GET: api/PlanAgences/5
        [HttpGet("GetPlanAgence")]
        public async Task<ActionResult<PlanAgence>> GetPlanAgence(int id)
        {
          if (_context.PlanAgences == null)
          {
              return NotFound();
          }
            var planAgence = await _context.PlanAgences.FindAsync(id);

            if (planAgence == null)
            {
                return NotFound();
            }

            return planAgence;
        }

        // PUT: api/PlanAgences/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutPlanAgence")]
        public async Task<IActionResult> PutPlanAgence(int id, PlanAgence planAgence)
        {
            if (id != planAgence.Id)
            {
                return BadRequest();
            }

            _context.Entry(planAgence).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanAgenceExists(id))
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

        // POST: api/PlanAgences
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostPlanAgence")]
        public async Task<ActionResult<PlanAgence>> PostPlanAgence(string refSemaine,string Agence,string destination, int navette)
        {
          if (_context.PlanAgences == null)
          {
              return Problem("Entity set 'LtmsContext.PlanAgences'  is null.");
          }
      
            

            var matchingPlanHebdo = await _context.PlanHebdos
                .Where(ps =>
                    ps.RefSemaine == refSemaine)
                .ToListAsync();

            var effectif = matchingPlanHebdo
                   .Select(p => p.Matricule)
                   .Distinct()
                   .Count();

            var besoinEnBus = (int)Math.Ceiling((double)effectif / 50);

            var planAgence = new PlanAgence
            {
                Mois = matchingPlanHebdo.FirstOrDefault()?.Mois,
                Annee = matchingPlanHebdo.FirstOrDefault()?.Annee,
                RefSemaine = matchingPlanHebdo.FirstOrDefault()?.RefSemaine ?? "",
                Circuit = matchingPlanHebdo.FirstOrDefault()?.Circuit ?? "",
                Effectif = effectif,
                BesoinEnBus=besoinEnBus,
                Agence=Agence,
                Destination=destination,
                Navette=navette

                
            };

            _context.PlanAgences.Add(planAgence);
        
                await _context.SaveChangesAsync();
    

            return CreatedAtAction("GetPlanAgence", new { id = planAgence.Id }, planAgence);
        }

        // DELETE: api/PlanAgences/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlanAgence(int id)
        {
            if (_context.PlanAgences == null)
            {
                return NotFound();
            }
            var planAgence = await _context.PlanAgences.FindAsync(id);
            if (planAgence == null)
            {
                return NotFound();
            }

            _context.PlanAgences.Remove(planAgence);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlanAgenceExists(int id)
        {
            return (_context.PlanAgences?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost("SendEmail")]
        public void SendEmail(string mail, IFormFile facture)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("mayssaneji6@gmail.com"));
            email.To.Add(MailboxAddress.Parse(mail));
            email.Subject = "Planification";

            // Create an attachment from the facture file
            var attachment = new MimePart(facture.ContentType)
            {
                Content = new MimeContent(facture.OpenReadStream(), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = facture.FileName
            };

            // Add the attachment to the email
            var multipart = new Multipart("mixed");
            multipart.Add(attachment);
            email.Body = multipart;

            using var smtp = new SmtpClient();
            smtp.Connect("smtp-relay.sendinblue.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("mayssaneji6@gmail.com", "n4aLdtYXPjvgWTD3");
            smtp.Send(email);
            smtp.Disconnect(true);
        }



    }
}
