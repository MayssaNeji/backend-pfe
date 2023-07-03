using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTMS.Models;
using OfficeOpenXml;

namespace LTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployesController : ControllerBase
    {
        private readonly LtmsContext _context;

        public EmployesController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/Employes
        [HttpGet("GetAllEmployes")]
        public async Task<ActionResult<IEnumerable<Employe>>> GetEmployes()
        {
          if (_context.Employes == null)
          {
              return NotFound();
          }
            return await _context.Employes.ToListAsync();
        }

        // GET: api/Employes/5
        [HttpGet("GetEmploye")]
        public async Task<ActionResult<Employe>> GetEmploye(int matricule)
        {
          if (_context.Employes == null)
          {
              return NotFound();
          }
            var employe = await _context.Employes.FindAsync(matricule);

            if (employe == null)
            {
                return NotFound();
            }

            return employe;
        }

        // PUT: api/Employes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutEmploye")]
        public async Task<IActionResult> PutEmploye(int id, Employe employe)
        {
            if (id != employe.Matricule)
            {
                return BadRequest();
            }

            _context.Entry(employe).State = EntityState.Modified;

                await _context.SaveChangesAsync();
         
           

            return NoContent();
        }

        // POST: api/Employes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostEmploye")]
        public async Task<ActionResult<Employe>> PostEmploye(Employe employe)
        {


            var existingChauffeur = await _context.Employes.FirstOrDefaultAsync(c => c.Nom == employe.Nom &&  c.Prenom == employe.Prenom && c.Telephone == employe.Telephone);
            if (existingChauffeur != null)
            {
                return BadRequest("Employé deja existant");
            }
            else
            {
                _context.Employes.Add(employe);
                await _context.SaveChangesAsync();

            }



            

            return CreatedAtAction("GetEmploye", new { id = employe.Matricule }, employe);
        }

        // DELETE: api/Employes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmploye(int id)
        {
            if (_context.Employes == null)
            {
                return NotFound();
            }
            var employe = await _context.Employes.FindAsync(id);
            if (employe == null)
            {
                return NotFound();
            }

            _context.Employes.Remove(employe);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeExists(int id)
        {
            return (_context.Employes?.Any(e => e.Matricule == id)).GetValueOrDefault();
        }




        [HttpPost("UploadExcelFile")]
        public async Task<IActionResult> UploadExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            // Check if the expected table structure is present
            if (worksheet == null || worksheet.Dimension == null || worksheet.Dimension.End.Row < 2 || worksheet.Dimension.End.Column < 12)
            {
                return BadRequest("The uploaded file is incompatible.");
            }

            // Validate column names in the first row
            var expectedColumnNames = new List<string>
    {
        "Nom",
        "Prenom",
        "Matricule",
        "Telephone",
        "CentreDeCout",
        "ContreMaitre",
        "NomDuGroupe",
        "Ps",
        "Segment",
        "Shift",
        "Station"
    };

            var actualColumnNames = new List<string>();
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var columnName = worksheet.Cells[1, col].Value?.ToString();
                actualColumnNames.Add(columnName);
            }

            if (!expectedColumnNames.SequenceEqual(actualColumnNames))
            {
                return BadRequest("The uploaded file has an invalid format. Please make sure the column names are correct.");
            }

            var data = new List<Employe>();
            var errorMessages = new List<string>();

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var model = new Employe
                {
                    Nom = worksheet.Cells[row, 2].Value?.ToString(),
                    Prenom = worksheet.Cells[row, 3].Value?.ToString(),
                    Matricule = 0, // Initialize with a default value

                    // Convert.ToInt32 replaced with int.TryParse
                    Telephone = int.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out var telephoneValue)
                        ? telephoneValue
                        : 0,

                    CentreDeCout = worksheet.Cells[row, 6].Value?.ToString(),
                    ContreMaitre = worksheet.Cells[row, 7].Value?.ToString(),
                    NomDuGroupe = worksheet.Cells[row, 8].Value?.ToString(),
                    Ps = worksheet.Cells[row, 9].Value?.ToString(),
                    Segment = worksheet.Cells[row, 10].Value?.ToString(),
                    Shift = worksheet.Cells[row, 11].Value?.ToString(),
                    Station = worksheet.Cells[row, 12].Value?.ToString()
                };

                // int.TryParse used for Matricule as well
                int.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out var matriculeValue);
                model.Matricule = matriculeValue;

                // Perform validation checks
                if (string.IsNullOrEmpty(model.Nom))
                {
                    errorMessages.Add($"Invalid value in row {row}: Nom is required.");
                }

                // Add more validation checks as per your requirements

                if (errorMessages.Count == 0)
                {
                    data.Add(model);
                }
            }

            if (errorMessages.Count > 0)
            {
                // Return error messages indicating the invalid data
                return BadRequest(string.Join(" ", errorMessages));
            }

            _context.Employes.AddRange(data);
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
