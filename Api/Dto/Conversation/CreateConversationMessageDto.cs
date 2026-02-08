using System.ComponentModel.DataAnnotations;
namespace VideoGameApi.Api.Dto.Conversation
{
    public class CreateConversationMessageDto
    {
        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
