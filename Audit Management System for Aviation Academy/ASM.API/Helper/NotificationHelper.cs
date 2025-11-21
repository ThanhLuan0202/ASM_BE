using ASM.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ASM.API.Helper
{
    public class NotificationHelper
    {
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationHelper(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public async Task SendToUserAsync(string userId, object data)
        {
            await _hub.Clients.User(userId).SendAsync("ReceiveNotification", data);
        }
    }
}
