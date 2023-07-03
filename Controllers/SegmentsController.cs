using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LTMS.Models;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;

namespace LTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SegmentsController : ControllerBase
    {
        private readonly LtmsContext _context;

        public SegmentsController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/Segments
        [HttpGet("GetAllSegments")]
        public async Task<ActionResult<IEnumerable<Segment>>> GetAllSegments()
        {
            if (_context.Segments == null)
            {
                return NotFound();
            }
            return await _context.Segments.ToListAsync();
        }

        // GET: api/Segments/5
        [HttpGet("GetSegment")]
        public async Task<ActionResult<Segment>> GetSegment(string nom)
        {
            if (_context.Segments == null)
            {
                return NotFound();
            }
            var segment = await _context.Segments.FirstOrDefaultAsync(s => s.Nom == nom);

            if (segment == null)
            {
                return NotFound();
            }

            return segment;
        }

        // PUT: api/Segments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Update")]
        public async Task<IActionResult> PutSegment(int id, [FromBody] Segment segment)
        {
            if ( segment==null)
            {
                return BadRequest();
            }

            _context.Entry(segment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SegmentExists(id))
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

        // POST: api/Segments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostSegment")]
        public async Task<ActionResult<Segment>> PostSegment(Segment segment)
        {
          if (_context.Segments == null)
          {
              return Problem("Entity set 'LtmsContext.Segments'  is null.");
          }


            var existingChauffeur = await _context.Segments.FirstOrDefaultAsync(c => c.NomSegSapRef == segment.NomSegSapRef || c.Nom == segment.Nom);
            if (existingChauffeur != null)
            {
                return BadRequest("Segment existe déjà");
            }
            else
            {
                _context.Segments.Add(segment);
                await _context.SaveChangesAsync();

            }


            return CreatedAtAction("GetSegment", new { id = segment.Id }, segment);
        }

        // DELETE: api/Segments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> DeleteSegment(int id)
        {
           
           
            var segment = await _context.Segments.FindAsync(id);
            if (segment == null)
            {
                return NotFound();
            }

            _context.Segments.Remove(segment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SegmentExists(int id)
        {
            return (_context.Segments?.Any(e => e.Id == id)).GetValueOrDefault();
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

            if (worksheet == null || worksheet.Dimension == null || worksheet.Dimension.Rows < 2)
            {
                return BadRequest("le fichier est vide.");
            }

            // Validate column names in the first row
            var expectedColumnNames = new List<string>
    {
        "Nom",
        "NomSegSapRef",
        "CentreDeCout",
        "ChefDeSegment",
        "RhSegment"
    };

            var actualColumnNames = new List<string>();
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var columnName = worksheet.Cells[1, col].Value?.ToString();
                actualColumnNames.Add(columnName);
            }

            if (!expectedColumnNames.SequenceEqual(actualColumnNames))
            {
                return BadRequest("le fichier introduit est incorrect");
            }

            var data = new List<Segment>();
            var errorMessages = new List<string>();

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var model = new Segment
                {
                    Nom = worksheet.Cells[row, 2].Value?.ToString(),
                    NomSegSapRef = worksheet.Cells[row, 3].Value?.ToString(),
                    CentreDeCout = worksheet.Cells[row, 4].Value?.ToString(),
                    ChefDeSegment = worksheet.Cells[row, 5].Value?.ToString(),
                    RhSegment = worksheet.Cells[row, 6].Value?.ToString()
                };

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

            _context.Segments.AddRange(data);
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
