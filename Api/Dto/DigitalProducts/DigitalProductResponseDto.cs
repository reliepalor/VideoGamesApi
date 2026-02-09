using VideoGameApi.Models.DigitalProducts.Enums;

namespace VideoGameApi.Api.Dto.DigitalProducts
{
    public class DigitalProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public DigitalProductType ProductType { get; set; }
        public LicenseDuration LicenseDuration { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? ImagePath { get; set; }
        public int Stock { get; set; }
        public int AvailableKeys {get; set;}
    }
}
