using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTMS.Models;
using OfficeOpenXml;
using System.Text;

namespace LTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LtmsContext _context;

        public UserController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet("GetAllComptes")]
        public async Task<ActionResult<IEnumerable<Compte>>> GetAllComptes()
        {
          if (_context.Comptes == null)
          {
              return NotFound();
          }
            return await _context.Comptes.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("GetCompte")]
        public async Task<ActionResult<Compte>> GetCompte(int id)
        {
          if (_context.Comptes == null)
          {
              return NotFound();
          }
            var compte = await _context.Comptes.FindAsync(id);

            if (compte == null)
            {
                return NotFound();
            }

            return compte;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutCompte")]
        public async Task<IActionResult> PutCompte(int id, Compte compte)
        {
            if (id != compte.Id)
            {
                return BadRequest();
            }

            _context.Entry(compte).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompteExists(id))
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

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPost("AddCompte")]
        public async Task<ActionResult<Compte>> AddCompte(Compte compte)
        {
          if (_context.Comptes == null)
          {
              return Problem("Entity set 'LtmsContext.Comptes'  is null.");
          }
            _context.Comptes.Add(compte);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompte", new { id = compte.Id }, compte);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompte(int id)
        {
            if (_context.Comptes == null)
            {
                return NotFound();
            }
            var compte = await _context.Comptes.FindAsync(id);
            if (compte == null)
            {
                return NotFound();
            }

            _context.Comptes.Remove(compte);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompteExists(int id)
        {
            return (_context.Comptes?.Any(e => e.Id == id)).GetValueOrDefault();
        }



        [HttpPost("UploadExcelFile")]
        public async Task<IActionResult>UploadExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            var data = new List<Compte>();
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                // Read the image file data from the worksheet and convert it to a byte array
                var avatar = worksheet.Cells[row, 9].Value as byte[];

                var model = new Compte
                {
                    Login = worksheet.Cells[row, 2].Value?.ToString(),
                    PasswordHash = Encoding.UTF8.GetBytes(worksheet.Cells[row, 3].Value?.ToString()),
                    PasswordSalt = Encoding.UTF8.GetBytes(worksheet.Cells[row, 4].Value?.ToString()),
                    Nom = worksheet.Cells[row, 5].Value?.ToString(),
                    Prenom = worksheet.Cells[row, 6].Value?.ToString(),
                    Matricule = Convert.ToInt32(worksheet.Cells[row, 7].Value),
                    Tel = Convert.ToInt32(worksheet.Cells[row, 8].Value),
                    DateDeNaissance = Convert.ToDateTime(worksheet.Cells[row, 9].Value),
                    Role = worksheet.Cells[row, 10].Value?.ToString()

                };

                data.Add(model);
            }

            _context.Comptes.AddRange(data);
            await _context.SaveChangesAsync();

            var currentUser = HttpContext.User.Identity.Name;

            var historique = new HistoriqueImport
            {
                NomFichier = file.FileName,
                DateImport = DateTime.Now,
                Creater = currentUser
            };
            _context.HistoriqueImports.Add(historique);
            _context.SaveChanges();

            return Ok(data);
        }

    





        [HttpGet("GetHistoriqueImports")]
        public async Task<ActionResult<IEnumerable<HistoriqueImport>>> GetHistoriqueImports()
        {
            return await _context.HistoriqueImports.ToListAsync();
        }
    }

}
