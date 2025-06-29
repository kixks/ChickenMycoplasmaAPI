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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DetectionController(manokDetectDBContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("check-farmer-id")]
        public IActionResult checkFarmerId([FromBody] VerifyFarmerDTO request)
        {
            bool exists = _context.Users.Any(f => f.Id == request.Id);
            return Ok(new { exists });
        }


        [HttpPost("upload-snapshot")]

        public async Task<IActionResult> UploadSnapshot([FromBody] SnapshotRequestDto request)
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

            var snapshotUrl = $"https://kingfish-wealthy-calf.ngrok-free.app/Snapshots/{fileName}";


            try
            {
                var message = $"ALERT: A symptom was detected on a chicken. View: {snapshotUrl}";
                await SendSmsNotification(farmerId, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SMS ERROR] Failed to send SMS to farmerId {farmerId}: {ex.Message}");
            }

            return Ok(new { status = "success", path = snapshot.snapshot });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<SnapshotResponseDTO>>> SendDetectedSymptomatic(int id)
        {
            var timezone = TimeZoneInfo.FindSystemTimeZoneById("Singapore"); // UTC+8

            var snapshots = await _context.Snapshots.Where(s => s.farmerId == id).OrderByDescending(s => s.CreatedAt).Select(s => new SnapshotResponseDTO
            {
                Id = s.Id,
                SnapshotUrl = $"https://kingfish-wealthy-calf.ngrok-free.app/Snapshots/{Path.GetFileName(s.snapshot)}",
                ConfidenceScore = s.confidenceScore,
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(s.CreatedAt, timezone)
            }).ToListAsync();

            return Ok(snapshots);

        }

        private async Task SendSmsNotification(int farmerId, string message)
        {
            var farmer = await _context.Users.FirstOrDefaultAsync(f => f.Id == farmerId);
            if (farmer == null || string.IsNullOrEmpty(farmer.MobileNumber))
            {
                Console.WriteLine("Farmer not found or no mobile number.");
                return;
            }

            if (farmer.LastSmsSentAt.HasValue && (DateTime.UtcNow - farmer.LastSmsSentAt.Value).TotalMinutes < 1)
            {
                Console.WriteLine($"SMS not sent: Cooldown active for farmerId {farmerId}.");
                return;
            }

            var smsPayload = new
            {
                api_token = _configuration["SmsToken:api_token"],
                phone_number = farmer.MobileNumber,
                message = message,
            };

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://sms.iprogtech.com/api/v1/sms_messages", smsPayload);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to send SMS: {error}");
            }
            else
            {
                Console.WriteLine("SMS sent successfully.");
            }
        }

    }
}
