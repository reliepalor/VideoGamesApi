using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VideoGameApi.Api.Dto.Conversation;
using VideoGameApi.Data;
using VideoGameApi.Api.Dto.Conversation;
using VideoGameApi.Hubs;
using VideoGameApi.Models;
using VideoGameApi.Models.Conversations;

namespace VideoGameApi.Api.Controllers.Conversations
{
    [ApiController]
    [Route("api/conversations")]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly VideoGameDbContext _context;
        private readonly IHubContext<ConversationHub> _hub;

        public ConversationsController(
            VideoGameDbContext context,
            IHubContext<ConversationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        private int UserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private bool IsAdmin =>
            User.IsInRole("Admin");

        /* =========================================================
           🔑 GET OR CREATE SINGLE CONVERSATION (MESSENGER RULE)
           ========================================================= */
        [HttpPost]
        public async Task<IActionResult> GetOrCreateConversation(
            CreateConversationDto dto)
        {
            // 🔍 Find existing conversation for this user
            var convo = await _context.Conversations
                .FirstOrDefaultAsync(c => c.UserId == UserId);

            // 🆕 Create ONLY if none exists
            if (convo == null)
            {
                convo = new Conversation
                {
                    UserId = UserId,
                    Subject = "Chat with Support",
                    Status = ConversationStatus.Open,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Conversations.Add(convo);
                await _context.SaveChangesAsync();
            }

            // 💬 Add message
            var message = new ConversationMessage
            {
                ConversationId = convo.Id,
                SenderUserId = UserId,
                Message = dto.Message,
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.ConversationMessages.Add(message);
            await _context.SaveChangesAsync();

            // 📡 Broadcast via SignalR
            await _hub.Clients
                .Group($"conversation-{convo.Id}")
                .SendAsync("ReceiveMessage", new RealtimeMessageDto
                {
                    Id = message.Id,
                    ConversationId = convo.Id,
                    SenderUserId = UserId,
                    SenderUsername = User.Identity?.Name ?? "User",
                    IsAdmin = false,
                    Message = message.Message,
                    CreatedAt = message.CreatedAt
                });

            return Ok(new
            {
                conversationId = convo.Id
            });
        }

        /* =========================================================
           📂 USER GETS HIS SINGLE CONVERSATION
           ========================================================= */
        [HttpGet("my")]
        public async Task<IActionResult> MyConversation()
        {
            var convo = await _context.Conversations
                .Where(c => c.UserId == UserId)
                .Select(c => new
                {
                    c.Id,
                    c.Subject,
                    Status = c.Status.ToString(),
                    c.CreatedAt
                })
                .FirstOrDefaultAsync();

            // No conversation yet → return empty
            if (convo == null)
                return Ok(null);

            return Ok(convo);
        }

        /* =========================================================
           💬 GET CONVERSATION WITH MESSAGES
           ========================================================= */
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversation(int id)
        {
            var convo = await _context.Conversations
                .Include(c => c.Messages)
                    .ThenInclude(m => m.SenderUser)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (convo == null)
                return NotFound();

            if (!IsAdmin && convo.UserId != UserId)
                return Forbid();

            return Ok(new
            {
                convo.Id,
                convo.Subject,
                Status = convo.Status.ToString(),
                messages = convo.Messages
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => new
                    {
                        m.Id,
                        m.Message,
                        m.CreatedAt,
                        m.IsAdmin,
                        m.Reaction,
                        SenderUsername = m.SenderUser.Username
                    })
            });
        }

        /* =========================================================
           ✉️ SEND MESSAGE (USER OR ADMIN)
           ========================================================= */
        [HttpPost("{id}/messages")]
        public async Task<IActionResult> SendMessage(
            int id,
            CreateConversationMessageDto dto)
        {
            var convo = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == id);

            if (convo == null)
                return NotFound();

            if (!IsAdmin && convo.UserId != UserId)
                return Forbid();

            if (convo.Status == ConversationStatus.Closed)
                return BadRequest("Conversation is closed");

            var message = new ConversationMessage
            {
                ConversationId = id,
                SenderUserId = UserId,
                Message = dto.Message,
                IsAdmin = IsAdmin,
                CreatedAt = DateTime.UtcNow
            };

            _context.ConversationMessages.Add(message);
            await _context.SaveChangesAsync();

            // 📡 Broadcast realtime
            await _hub.Clients
                .Group($"conversation-{id}")
                .SendAsync("ReceiveMessage", new RealtimeMessageDto
                {
                    Id = message.Id,
                    ConversationId = id,
                    SenderUserId = UserId,
                    SenderUsername = User.Identity?.Name ?? "User",
                    IsAdmin = IsAdmin,
                    Message = message.Message,
                    CreatedAt = message.CreatedAt
                });

            return Ok();
        }

        [HttpPost("{id}/seen")]
        public async Task<IActionResult> MarkSeen(int id)
        {
            var messages = await _context.ConversationMessages
                .Where(m => m.ConversationId == id && !m.IsSeen)
                .ToListAsync();

            foreach (var msg in messages)
                msg.IsSeen = true;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{conversationId}/messages/{messageId}/reactions")]
        public async Task<IActionResult> AddReaction(int conversationId, int messageId, [FromBody] AddReactionDto dto)
        { 
        var message = await _context.ConversationMessages 
                .FirstOrDefaultAsync(m => m.Id == messageId && m.ConversationId == conversationId);
            if (message == null)
                return NotFound();
           
            message.Reaction = dto.Reaction;
            await _context.SaveChangesAsync();
           
            return Ok();        
        }

    }
}
