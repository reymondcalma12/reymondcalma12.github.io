using Microsoft.AspNetCore.SignalR;
using Sample.Data;

namespace Sample.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext db;

        public ChatHub(AppDbContext db)
        {
            this.db = db;
        }


        public async Task UserOffline(string userId)
        {
            var user =  db.Users.FirstOrDefault(a => a.Id == userId);

            if (user != null)
            {
                user.Online = false;
                await db.SaveChangesAsync();
                await Clients.All.SendAsync("UserStatusUpdated", userId, false);
            }
        }


    }
}
