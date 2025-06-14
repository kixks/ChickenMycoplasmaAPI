using ManokDetectAPI.Database;
using ManokDetectAPI.Entities;
using ManokDetectAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManokDetectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetectionController : ControllerBase
    {
        private readonly manokDetectDBContext _context;

        public DetectionController(manokDetectDBContext context)
        {
            _context = context;
        }

        [HttpPost("check-farmer-id")]
        public IActionResult checkFarmerId([FromBody] VerifyFarmerDTO request)
        {
            bool exists = _context.Users.Any(f => f.Id == request.Id);
            return Ok(new { exists });
        }


        [HttpPost("upload-snapshot")]

        public IActionResult UploadSnapshot([FromBody] SnapshotRequestDto request)
        {
            var base64Image = request.snapshot;
            var farmerId = request.farmerId;
            var confidenceScore = request.confidenceScore;

            if (string.IsNullOrEmpty(base64Image) || farmerId == 0)
                return BadRequest("Missing data");

            var bytes = Convert.FromBase64String(base64Image);
            var fileName = $"{farmerId}_{DateTime.UtcNow.Ticks}.jpg";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" ,"Snapshots");
            Directory.CreateDirectory(folderPath); // ensure the folder exists

            var filePath = Path.Combine(folderPath, fileName);
            System.IO.File.WriteAllBytes(filePath, bytes);

            var snapshot = new Snapshot
            {
                farmerId = farmerId,
                snapshot = Path.Combine("Snapshots", fileName),
                confidenceScore = confidenceScore
            };

            _context.Snapshots.Add(snapshot);
            _context.SaveChanges();

            return Ok(new { status = "success", path = snapshot.snapshot });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<SnapshotResponseDTO>>> SendDetectedSymptomatic(int id)
        {
            var snapshots = await _context.Snapshots.Where(s => s.farmerId == id).OrderByDescending(s => s.CreatedAt).Select(s => new SnapshotResponseDTO
            {
                Id = s.Id,
                SnapshotUrl = $"https://localhost:7165/Snapshots/{Path.GetFileName(s.snapshot)}",
                ConfidenceScore = s.confidenceScore,
                CreatedAt = s.CreatedAt
            }).ToListAsync();

            return Ok(snapshots);

        }


    }
}
