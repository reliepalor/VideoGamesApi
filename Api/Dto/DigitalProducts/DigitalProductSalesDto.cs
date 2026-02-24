namespace VideoGameApi.Api.Dto.DigitalProducts
{
    public class DigitalProductSalesDto
    {
        public int DigitalProductId { get; set; }
        public string Name { get; set; } = string.Empty;

        public int TotalOrders { get; set; }

        public int TotalQuantity { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}