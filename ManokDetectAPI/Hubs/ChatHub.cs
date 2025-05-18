using Microsoft.AspNetCore.SignalR;
using ManokDetectAPI.Entities;
using ManokDetectAPI.Database;

namespace ManokDetectAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly manokDetectDBContext _context;

        public ChatHub(manokDetectDBContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string text)
        {
            var message = new Message
            {
                User = user,
                Text = text,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
