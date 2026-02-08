namespace VideoGameApi.Api.Dto.Orders
{
    public class AdminApproveOrderDto
    {
        public List<ProductKeyDto> Items { get; set; } = new();
    }

    public class ProductKeyDto
    {
        public int OrderItemId { get; set; }
        public string ProductKey { get; set; } = null!;
    }
}
