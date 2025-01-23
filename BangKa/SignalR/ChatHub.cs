using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BangKa.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
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
    }
}
