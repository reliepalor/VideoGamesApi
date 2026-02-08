using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VideoGameApi.Models.Users;

namespace VideoGameApi.Models.Conversations
{
    public enum ConversationStatus
    {
        Open = 0,
        Closed = 1
    }

    public class Conversation
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Subject { get; set; } = string.Empty;

        public ConversationStatus Status { get; set; } = ConversationStatus.Open;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }

        public ICollection<ConversationMessage> Messages { get; set; }
            = new List<ConversationMessage>();
    }
}
