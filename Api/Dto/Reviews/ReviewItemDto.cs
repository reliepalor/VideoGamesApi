namespace VideoGameApi.Api.Dto.Reviews
{
    public class ReviewItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
