using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace BangKaAPI.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
        // Store list UserName and ConnectionId
        private static ConcurrentDictionary<string, string> _Users = new ConcurrentDictionary<string, string>();
        public async Task Register(string username)
        {
            _Users[username] = Context.ConnectionId;
            await Clients.All.SendAsync("UserConnected", username);
        }
        public override Task OnConnectedAsync()
        {
            var userName = Context.User.Identity.Name; // Lấy thông tin người dùng từ cookie
            Console.WriteLine($"User {userName} connected with Connection ID: {Context.ConnectionId}");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userName = Context.User.Identity.Name;
            Console.WriteLine($"User {userName} disconnected.");
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            var userName = Context.User.Identity.Name;
            await Clients.All.SendAsync("ReceiveMessage", userName, message);
        }

        public async Task SendMessageToUser(string recipientUsername, string message)
        {
            if(_Users.TryGetValue(recipientUsername, out var connectionId))
            {
                var senderUsername = _Users.FirstOrDefault(u => u.Value == Context.ConnectionId);
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderUsername, message);
            }
        }
    }
}
