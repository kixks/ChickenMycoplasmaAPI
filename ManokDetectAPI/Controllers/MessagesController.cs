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

        public MessagesController(manokDetectDBContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        //[HttpGet("{userName}")]
        //public async Task<ActionResult<IEnumerable<Message>>> GetMessages(string userName)
        //{
        //    var filteredMessages = await _context.Messages
        //        .Where(m => m.User == userName || m.Recipient == userName)
        //        .OrderBy(m => m.Timestamp)
        //        .ToListAsync();

        //    return filteredMessages;
        //}

        [HttpGet("conversation/{user1}/{user2}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetConversation(string user1, string user2)
        {
            return await _context.Messages
                .Where(m => (m.User == user1 && m.Recipient == user2) || (m.User == user2 && m.Recipient == user1))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        
    }



}
