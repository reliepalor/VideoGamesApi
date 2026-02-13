namespace VideoGameApi.Api.Dto.DigitalProducts
{
    public class DigitalProductKeyResponseDto
    {
        public int Id { get; set; }
        public string ProductKey { get; set; } = string.Empty;
        public bool IsUsed { get; set; }
        public int? AssignedToUserId { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
