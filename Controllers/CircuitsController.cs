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
    public class CircuitsController : ControllerBase
    {
        private readonly LtmsContext _context;

        public CircuitsController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/Circuits
        [HttpGet("GetAllCircuits")]
        public async Task<ActionResult<IEnumerable<Circuit>>> GetAllCircuits()
        {
          if (_context.Circuits == null)
          {
              return NotFound();
          }
            return await _context.Circuits.ToListAsync();
        }

        // GET: api/Circuits/5
        [HttpGet("GetCircuit")]
        public async Task<ActionResult<Circuit>> GetCircuit(int id)
        {
          if (_context.Circuits == null)
          {
              return NotFound();
          }
            var circuit = await _context.Circuits.FindAsync(id);

            if (circuit == null)
            {
                return NotFound();
            }

            return circuit;
        }

        // PUT: api/Circuits/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutCircuit")]
        public async Task<IActionResult> PutCircuit(int id, Circuit circuit)
        {
            if (id != circuit.Id)
            {
                return BadRequest();
            }

            var existingCircuit = await _context.Circuits.FirstOrDefaultAsync(c => c.RefSapLeoni == circuit.RefSapLeoni && c.Id != circuit.Id);
            if (existingCircuit != null)
            {
                return BadRequest("La station appartient déjà à un autre circuit.");
            }

            _context.Entry(circuit).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Circuits
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostCircuit")]
        public async Task<ActionResult<Circuit>> PostCircuit(Circuit circuit)
        {
            if (string.IsNullOrEmpty(circuit.Agence))
            {
                return BadRequest("circuit property is required.");
            }





            var existingChauffeur = await _context.Circuits.FirstOrDefaultAsync(c => c.RefChemin == circuit.RefChemin);
            if (existingChauffeur != null)
            {
                return BadRequest("circuit existe déjà");
            }
            else
            {
                var existingSTATION = await _context.Circuits.FirstOrDefaultAsync(c => c.RefSapLeoni == circuit.RefSapLeoni);
                if (existingSTATION != null)
                {
                    return BadRequest("la station appartient dejà a un circuit");
                }

                _context.Circuits.Add(circuit);
                await _context.SaveChangesAsync();

            }


         

            return CreatedAtAction("GetCircuit", new { id = circuit.Id }, circuit);
        }

        // DELETE: api/Circuits/5
        [HttpDelete("DeleteCircuit")]
        public async Task<IActionResult> DeleteCircuit(int id)
        {
            if (_context.Circuits == null)
            {
                return NotFound();
            }
            var circuit = await _context.Circuits.FindAsync(id);
            if (circuit == null)
            {
                return NotFound();
            }

            _context.Circuits.Remove(circuit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CircuitExists(int id)
        {
            return (_context.Circuits?.Any(e => e.Id == id)).GetValueOrDefault();
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

            var data = new List<Circuit>();
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {


                var model = new Circuit
                {
                    RefSapLeoni = worksheet.Cells[row, 2].Value?.ToString(),
                    PointArrivee = worksheet.Cells[row, 3].Value?.ToString(),
                    RefChemin = worksheet.Cells[row, 4].Value?.ToString(),
                    Agence = worksheet.Cells[row, 5].Value?.ToString(),
                    CoutKm = Convert.ToInt32(worksheet.Cells[row, 6].Value),
                    NbKm = Convert.ToInt32(worksheet.Cells[row, 7].Value),
                    ContributionEmploye = Convert.ToInt32(worksheet.Cells[row, 8].Value),


                };

                data.Add(model);
            }

            _context.Circuits.AddRange(data);
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
