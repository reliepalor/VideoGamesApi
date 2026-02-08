using FirebaseAdmin.Messaging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VideoGameApi.Models.Users;

namespace VideoGameApi.Models.Conversations
{
    public class ConversationMessage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Conversation))]
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        [ForeignKey(nameof(SenderUser))]
        public int SenderUserId { get; set; }
        public User SenderUser { get; set; } = null!;


        public string Message { get; set; } = string.Empty;
        public bool IsDelivered { get; set; } = false;
        public bool IsSeen { get; set; } = false;
        public string? Reaction { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
       public bool IsAdmin { get; internal set; }
    }
}
