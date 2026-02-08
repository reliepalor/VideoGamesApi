namespace VideoGameApi.Api.Dto.Games
{
    public class VideoGameUpdateDto
    {
        public string Title { get; set; }
        public string Platform { get; set; }
        public string Developer { get; set; }
        public string Publisher { get; set; }
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }
    }
}
