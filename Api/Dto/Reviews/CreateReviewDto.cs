using System.ComponentModel.DataAnnotations;
namespace VideoGameApi.Api.Dto.Reviews
{
    public class CreateReviewDto
    {
        public int VideoGameId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int? OrderId { get; set; }
    }
}
