using Microsoft.AspNetCore.SignalR;
using ManokDetectAPI.Entities;
using ManokDetectAPI.Database;
using ManokDetectAPI.DTO;

namespace ManokDetectAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly manokDetectDBContext _context;
        private static readonly Dictionary<string, string> UserConnections = new();


        public ChatHub(manokDetectDBContext context)
        {
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            var userName = Context.GetHttpContext()?.Request.Query["user"];
            if (!string.IsNullOrEmpty(userName))
            {
                UserConnections[userName] = Context.ConnectionId;
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var user = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (!string.IsNullOrEmpty(user.Key))
            {
                UserConnections.Remove(user.Key);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(SendMessageDto messageDto)
        {
            var message = new Message
            {
                User = messageDto.Sender,
                Recipient = messageDto.Recipient,
                Text = messageDto.Text,
                ImageUrl = messageDto.ImageUrl,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            var response = new
            {
                user = message.User,
                recipient = message.Recipient,
                text = message.Text,
                imageUrl = message.ImageUrl,
                timestamp = message.Timestamp
            };

            if (UserConnections.TryGetValue(messageDto.Recipient, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", response);
            }

            // Optional: echo back to sender too
            if (UserConnections.TryGetValue(messageDto.Sender, out var senderConnection))
            {
                await Clients.Client(senderConnection).SendAsync("ReceiveMessage", response);
            }
        }
    }
}
