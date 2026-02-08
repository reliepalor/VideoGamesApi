namespace VideoGameApi.Api.Dto.Orders
{
    public class OrderItemWithReviewDto
    {
        public int OrderId { get; set; }
        public int VideoGameId { get; set; }
        public string GameTitle { get; set; } = null!;
        public bool HasReviewed { get; set; }
    }
}
