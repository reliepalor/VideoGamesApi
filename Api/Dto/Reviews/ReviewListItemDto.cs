namespace VideoGameApi.Api.Dto.Reviews
{
    public class ReviewListItemDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
    }
}
