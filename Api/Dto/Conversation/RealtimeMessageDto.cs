namespace VideoGameApi.Api.Dto.Conversation
{
    public class RealtimeMessageDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }

        public int SenderUserId { get; set; }
        public string SenderUsername { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
