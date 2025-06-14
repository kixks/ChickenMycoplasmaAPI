using ManokDetectAPI.Entities;
using ManokDetectAPI.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManokDetectAPI.DTO;




namespace ManokDetectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly manokDetectDBContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MessagesController(manokDetectDBContext context , IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            return await _context.Messages.OrderBy(m => m.Timestamp).ToListAsync();
        }

        [HttpPost]

        public async Task<IActionResult> SendSMS([FromBody] SendSmsDto request)
        {
            //looking up for the farmers phone number
            var farmer = await _context.Users.FirstOrDefaultAsync(f => f.Id == request.farmerId);

            if (farmer == null)
            {
                return NotFound(new { message = "Farmer Not Found" });
            }

            var mobileNumber = farmer.MobileNumber;

            if (mobileNumber == null)
            {
                return NotFound(new { message = "Mobile Number Not Found Please Go to Settings to Update" });
            }

            var messageToSent = request.MessageContent;
            var apiToken = _configuration["SmsToken:api_token"];

            var smsPayload = new
            {
                api_token = apiToken,
                phone_number = mobileNumber,
                message = messageToSent,
            };

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://sms.iprogtech.com/api/v1/sms_messages", smsPayload);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"SMS API Error: {error}");
            }

            return Ok(new { status = "User has been notified!", to = mobileNumber });


        }
    }



}
