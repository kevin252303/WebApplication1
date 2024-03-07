using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.Extentions;

namespace WebApplication1.SignalR
{
    [Authorize]
    public class PresenceHub :Hub
    {
        private readonly PresenceTracker _tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }
        public override async Task OnConnectedAsync()
        {
            await _tracker.UserConnected(Context.User.GetUserName(),Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline",Context.User.GetUserName());

            var currentUser = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUser);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _tracker.UserDisconnected(Context.User.GetUserName(),Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

            var currentUser = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUser);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
