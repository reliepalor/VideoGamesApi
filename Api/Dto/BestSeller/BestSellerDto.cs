namespace VideoGameApi.Api.Dto.BestSeller
{
    public class BestSellerDto
    {
        public int VideoGameId { get; set; }
        public string Title { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public string ImagePath { get; set; }

        public decimal Price { get; set; }
    }
}
