using System.ComponentModel.DataAnnotations;
namespace VideoGameApi.Api.Dto.Reviews
{
    public class CreateReviewMessageDto
    {
        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
