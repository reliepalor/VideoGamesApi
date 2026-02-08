using VideoGameApi.Api.Dto.Reviews;

namespace VideoGameApi.Api.Dto.Games
{
    public class GameReviewsResponseDto
    {
        public int VideoGameId { get; set; }
        public string VideoGameTitle { get; set; } = null;
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ReviewItemDto> Reviews { get; set; } = [];
    }
}
