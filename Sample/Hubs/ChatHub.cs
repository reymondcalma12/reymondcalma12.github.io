using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Sample.Data;
using Sample.Models;

namespace Sample.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _dbContext;
        private readonly Dictionary<string, string> _connectedClients = new();

        public ChatHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task GetAllUsers()
        {
            try
            {
                var userId = Context.GetHttpContext().Session.GetString("UsersId");
                var allUsers = _dbContext.Users.Where(u => u.Id != userId).ToList();

                await Clients.Caller.SendAsync("ReceiveAllUsers", allUsers);

            }catch (Exception ex)
            {
                Console.WriteLine($"Error occurred in GetAllUsers: {ex.Message}");
                await Clients.Others.SendAsync("Error", "An error occurred while fetching users.");
            }

        }

        public async Task GetUsersWithMessages()
        {
            try
            {
                var userId = Context.GetHttpContext().Session.GetString("UsersId");

                var newMessages = (
                  from m in _dbContext.Message
                  where m.ReceiverId == userId
                  join u in _dbContext.Users on m.SenderId equals u.Id
                  group new { m, u } by new { u.Name, u.Id } into g
                  select new
                  {
                      SenderName = g.Key.Name,
                      SenderId = g.Key.Id,
                      Message = g.OrderByDescending(x => x.m.Date).First().m
                  })
                  .ToList();

                await Clients.Caller.SendAsync("GetUsersWithMessages", newMessages);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred in GetAllUsers: {ex.Message}");
                await Clients.Others.SendAsync("Error", "An error occurred while fetching users.");
            }
        }


        public async Task GetMessages(string receiverId)
        {
            var currentUserId = Context.GetHttpContext().Session.GetString("UsersId");
            var messages =  _dbContext.Message
                .Where(m => (m.ReceiverId == receiverId && m.SenderId == currentUserId) || (m.ReceiverId == currentUserId && m.SenderId == receiverId))
                .OrderBy(m => m.Date)
                .ToList();
            await Clients.Caller.SendAsync("ReceiveMessages", messages);
        }


        public async Task GetNewMessages()
        {
            var currentUserId = Context.GetHttpContext().Session.GetString("UsersId");

            var newMessages = await _dbContext.Message
                .Where(m => m.ReceiverId == currentUserId)
                .ToListAsync();

            await Clients.Caller.SendAsync("NotifyMessage", newMessages);
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


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.GetHttpContext().Session.GetString("UsersId");
            if (_connectedClients.ContainsKey(userId))
            {
                _connectedClients.Remove(userId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}
