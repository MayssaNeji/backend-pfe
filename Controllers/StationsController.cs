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
using Humanizer;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.ComponentModel;

namespace LTMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController : ControllerBase
    {
        private readonly LtmsContext _context;

        public StationsController(LtmsContext context)
        {
            _context = context;
        }

        // GET: api/Stations
        [HttpGet("GetAllStations")]
        public async Task<ActionResult<IEnumerable<Station>>> GetStations()
        {
          if (_context.Stations == null)
          {
              return NotFound();
          }
            return await _context.Stations.ToListAsync();
        }

        // GET: api/Stations/5
        [HttpGet("GetStation")]
        public async Task<ActionResult<Station>> GetStation(int id)
        {
          if (_context.Stations == null)
          {
              return NotFound();
          }
            var station = await _context.Stations.FindAsync(id);

            if (station == null)
            {
                return NotFound();
            }

            return station;
        }

        // PUT: api/Stations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutStation")]
        public async Task<IActionResult> PutStation(int id, Station station)
        {
            if (id != station.Id)
            {
                return BadRequest();
            }

            _context.Entry(station).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationExists(id))
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

        // POST: api/Stations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostStation")]
        public async Task<ActionResult<Station>> PostStation(Station station)
        {
          if (_context.Stations == null)
          {
              return Problem("Entity set 'LtmsContext.Stations'  is null.");
          }

            var existingChauffeur = await _context.Stations.FirstOrDefaultAsync(c => c.RefSapLeoni == station.RefSapLeoni);
            if (existingChauffeur != null)
            {
                return BadRequest("Vehicule existe déjà");
            }
            else {
                _context.Stations.Add(station);
                await _context.SaveChangesAsync();
            }

          

            return CreatedAtAction("GetStation", new { id = station.Id }, station);
        }
       
        // DELETE: api/Stations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            if (_context.Stations == null)
            {
                return NotFound();
            }
            var station = await _context.Stations.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }

            _context.Stations.Remove(station);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StationExists(int id)
        {
            return (_context.Stations?.Any(e => e.Id == id)).GetValueOrDefault();
        }




[HttpPost("UploadExcelFile")]
public async Task<IActionResult> UploadExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Le fichier est vide.");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            // Check if the expected table structure is present
            if (worksheet == null || worksheet.Dimension == null || worksheet.Dimension.End.Row < 2 || worksheet.Dimension.End.Column < 6)
            {
                return BadRequest("Le fichier ne contient pas la liste des stations.");
            }

            // Validate column names in the first row
            var expectedColumnNames = new List<string>
    {
        "RefSapLeoni",
        "ReferenceRegion",
        "Latitude",
        "Longitude",
        "Rayon"
    };

            var actualColumnNames = new List<string>();
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var columnName = worksheet.Cells[1, col].Value?.ToString();
                actualColumnNames.Add(columnName);
            }

            if (!expectedColumnNames.SequenceEqual(actualColumnNames))
            {
                return BadRequest("Le fichier a un format invalide. Vérifiez que les noms de colonnes sont corrects.");
            }

            var data = new List<Station>();

            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var refSapLeoni = worksheet.Cells[row, 2].Value?.ToString();
                var referenceRegion = worksheet.Cells[row, 3].Value?.ToString();
                var latitudeString = worksheet.Cells[row, 4].Value?.ToString();
                var longitudeString = worksheet.Cells[row, 5].Value?.ToString();
                var rayonString = worksheet.Cells[row, 6].Value?.ToString();

                // Perform validation checks
                if (string.IsNullOrEmpty(refSapLeoni) || string.IsNullOrEmpty(referenceRegion) ||
                    !int.TryParse(latitudeString, out int latitude) || !int.TryParse(longitudeString, out int longitude) ||
                    !int.TryParse(rayonString, out int rayon))
                {
                    return BadRequest("Le fichier contient des données invalides. Vérifiez que toutes les valeurs sont correctes.");
                }

                var model = new Station
                {
                    RefSapLeoni = refSapLeoni,
                    ReferenceRegion = referenceRegion,
                    Latitude = latitude,
                    Longitude = longitude,
                    Rayon = rayon
                };

                data.Add(model);
            }

            _context.Stations.AddRange(data);
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
