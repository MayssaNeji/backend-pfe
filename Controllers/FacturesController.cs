using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTMS.Models;
using OfficeOpenXml.Packaging;
using OfficeOpenXml;
using OfficeOpenXml.Style;


using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;



namespace LTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturesController : ControllerBase
    {
        private readonly LtmsContext _context;

        public FacturesController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/Factures
        [HttpGet("GetFactures")]
        public async Task<ActionResult<IEnumerable<Facture>>> GetFactures()
        {
          if (_context.Factures == null)
          {
              return NotFound();
          }
            return await _context.Factures.ToListAsync();
        }

        // GET: api/Factures/5
        [HttpGet("GetFacture")]
        public async Task<ActionResult<List<Facture>>> GetFacture(int Annee, int Mois, string agence)
        {
            var matchingFactures = await _context.Factures
                .Where(f =>
                    f.Annee == Annee.ToString() &&
                    f.Mois == Mois.ToString() &&
                    f.Agence == agence)
                .ToListAsync();

            if (matchingFactures.Count == 0)
            {
                return NotFound();
            }

            return matchingFactures;
        }


        // PUT: api/Factures/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutFacture")]
        public async Task<IActionResult> PutFacture(int id, Facture facture)
        {
            if (id != facture.Id)
            {
                return BadRequest();
            }

            _context.Entry(facture).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FactureExists(id))
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

        // POST: api/Factures
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostFacture")]
        public async Task<ActionResult<List<Facture>>> PostFacture(int Annee, int Mois, string agence)
        {
            var matchingPlanAgence = await _context.PlanAgences
                .Where(ps =>
                    ps.Annee == Annee.ToString() &&
                    ps.Mois == Mois.ToString() &&
                    ps.Agence == agence)
                .ToListAsync();

            if (matchingPlanAgence.Count == 0)
            {
                return NotFound("No matching PlanAgences found.");
            }

            var facture = new Facture();
            facture.Annee = Annee.ToString();
            facture.Mois = Mois.ToString();
            facture.Agence = agence;

            var circuit = await _context.Circuits.FirstOrDefaultAsync(c => c.RefChemin == matchingPlanAgence[0].Circuit.ToString());

            facture.Circuit = circuit.RefChemin.ToString();
            facture.NbrKm = circuit.NbKm;
            facture.CoutKm = circuit.CoutKm;

            List<Facture> factureList = new List<Facture>();

            foreach (var planSegment in matchingPlanAgence)
            {
                var newFacture = new Facture
                {
                    Annee = facture.Annee,
                    Mois = facture.Mois,
                    Agence = facture.Agence,
                    Circuit = facture.Circuit,
                    NbrKm = facture.NbrKm,
                    CoutKm = facture.CoutKm,
                    NbrNav = planSegment.Navette,
                    Totale = facture.NbrKm * facture.CoutKm * planSegment.Navette
                };

                factureList.Add(newFacture);
               
            }
            await _context.AddRangeAsync(factureList);
            await _context.SaveChangesAsync();
            return Ok(factureList);
        }





        // DELETE: api/Factures/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFacture(int id)
        {
            if (_context.Factures == null)
            {
                return NotFound();
            }
            var facture = await _context.Factures.FindAsync(id);
            if (facture == null)
            {
                return NotFound();
            }

            _context.Factures.Remove(facture);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FactureExists(int id)
        {
            return (_context.Factures?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail(string mail, IFormFile facture)
        {
        
            if (facture == null || facture.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            using var stream = new MemoryStream();
            await facture.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

           
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("mayssaneji6@gmail.com"));
            email.To.Add(MailboxAddress.Parse(mail));
            email.Subject = "Facture";

            var attachment = new MimePart(facture.ContentType)
            {
                Content = new MimeContent(facture.OpenReadStream(), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                FileName = facture.FileName
            };

            var multipart = new Multipart("mixed");
            multipart.Add(attachment);
            email.Body = multipart;

            using var smtp = new SmtpClient();
            smtp.Connect("smtp-relay.sendinblue.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("mayssaneji6@gmail.com", "n4aLdtYXPjvgWTD3");
            smtp.Send(email);
            smtp.Disconnect(true);

            return Ok();
        }




    }
}





