namespace VideoGameApi.Api.Dto.Reviews
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int VideoGameId { get; set; }
        public string VideoGameTitle { get; set; } = null!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

    }
}
