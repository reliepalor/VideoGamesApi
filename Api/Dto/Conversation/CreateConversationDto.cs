using System.ComponentModel.DataAnnotations;

namespace VideoGameApi.Api.Dto.Conversation
{
    public class CreateConversationDto
    {
        [Required]
        public string Subject { get; set; } = string.Empty;
        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
