using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace VideoGameApi.Hubs
{
    [Authorize]
    public class ConversationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("UserOnline", Context.UserIdentifier);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("UserOffline", Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");
        }

        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");
        }

        public async Task Typing(int conversationId)
        {
            await Clients
                .Group($"conversation-{conversationId}")
                .SendAsync("UserTyping", Context.UserIdentifier);
        }

        public async Task Seen(int conversationId)
        {
            await Clients
                .Group($"conversation-{conversationId}")
                .SendAsync("MessagesSeen", Context.UserIdentifier);
        }
    }
}
