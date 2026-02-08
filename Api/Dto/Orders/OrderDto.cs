namespace VideoGameApi.Api.Dto.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int? SourceCartItemId { get; set; }  
        public int VideoGameId { get; set; }               // <- added
        public string GameTitle { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string? ProductKey { get; set; }
    }
}
