using Microsoft.AspNetCore.SignalR;
using Sample.Data;
using Sample.Models;

namespace Sample.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _dbContext;

        public ChatHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task GetAllUsers()
        {
            var userId = Context.UserIdentifier;
            var allUsers = _dbContext.Users.Where(u => u.Id != userId).ToList();
            await Clients.All.SendAsync("ReceiveAllUsers", allUsers);
        }

        public async Task GetMessages(string receiverId)
        {
            var currentUserId = Context.UserIdentifier;
            var messages =  _dbContext.Message
                .Where(m => (m.ReceiverId == receiverId && m.SenderId == currentUserId) || (m.ReceiverId == currentUserId && m.SenderId == receiverId))
                .OrderBy(m => m.Date)
                .ToList();
            await Clients.All.SendAsync("ReceiveMessages", messages);
        }

        public async Task SendMessage(string receiverId, string messageText)
        {
            var senderId = Context.UserIdentifier;
            var message = new Message
            {
                Text = messageText,
                SenderId = senderId,
                ReceiverId = receiverId
            };
            _dbContext.Message.Add(message);
            await _dbContext.SaveChangesAsync();
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
