using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ASM.API.Hubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var user = connection.User;

            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
