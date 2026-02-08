namespace VideoGameApi.Api.Dto.DigitalOrders
{
    public class DigitalOrderItemDto
    {
        public int Id { get; set; }
        public string ProductKey { get; set; } = string.Empty;
        public bool IsAssigned { get; set; }
        public DateTime? AssignedAt { get; set; }
       
    }
}
