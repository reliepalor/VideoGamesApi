using VideoGameApi.Models.Games;

namespace VideoGameApi.Models.Orders
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int? SourceCartItemId { get; set; }

        public int VideoGameId { get; set; }
        public VideoGame VideoGame { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public string GameTitle { get; set; } = null!;

        // ✅ NEW
        public string? ProductKey { get; set; }
    }
}
