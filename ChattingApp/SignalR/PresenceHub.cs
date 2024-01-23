using ChattingApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChattingApp.SignalR
{
    [Authorize]
    public class PresenceHub :Hub
    {
        private readonly PresenceTracker tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            this.tracker = tracker;
        }
        public override async Task OnConnectedAsync()
        {
          var isOnline=  await tracker.USerConnected(Context.User.GetUsername(), Context.ConnectionId);
            if(isOnline)
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

            var currentUsers = await tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUSers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
           var isOffline= await tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
            if(isOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            var currentUsers=await tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUSers", currentUsers);
            await base.OnDisconnectedAsync(exception);


        }
    }
}
