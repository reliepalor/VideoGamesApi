using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoGameApi.Data;
using VideoGameApi.Models;

namespace VideoGameApi.Api.Controllers.Conversations
{
    [ApiController]
    [Route("api/admin/conversations")]
    [Authorize(Roles = "Admin")]
    public class AdminConversationsController(VideoGameDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var convos = await context.Conversations
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.Id,
                    c.Subject,
                    c.Status,
                    user = new
                    {
                        c.User.Username,
                        c.User.Email
                    },
                    c.CreatedAt
                })
                .ToListAsync();

            return Ok(convos);
        }

        //[HttpPost("{id}/close")]
        //public async Task<IActionResult> Close(int id)
        //{
        //    var convo = await context.Conversations.FindAsync(id);
        //    if (convo == null)
        //        return NotFound();

        //    convo.Status = ConversationStatus.Closed;
        //    convo.ClosedAt = DateTime.UtcNow;

        //    await context.SaveChangesAsync();
        //    return Ok(new { message = "Conversation closed" });
        //}
    }
}
