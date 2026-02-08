namespace VideoGameApi.Api.Dto.Games
{
    public class VideoGameSalesDto
    {
        public int VideoGameId { get; set; }
        public string Title { get; set; }
        public int TotalNumbers { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
