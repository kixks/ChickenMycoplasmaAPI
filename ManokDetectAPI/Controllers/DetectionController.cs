using ManokDetectAPI.Database;
using ManokDetectAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        //public IActionResult UploadSnapshot([FromBody] SnapshotRequest request)
        //{
        //    var base64Image = request.Snapshot;
        //    var farmerId = request.FarmerId;

        //    if (string.IsNullOrEmpty(base64Image) || string.IsNullOrEmpty(farmerId))
        //        return BadRequest("Missing data");

        //    var bytes = Convert.FromBase64String(base64Image);
        //    var filePath = $"Snapshots/{farmerId}_{DateTime.Now.Ticks}.jpg";
        //    System.IO.File.WriteAllBytes(filePath, bytes);

        //    return Ok(new { status = "success", path = filePath });
        //}



    }
}
