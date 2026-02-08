using Google.Api.Gax;
using System.ComponentModel.DataAnnotations;
using VideoGameApi.Models.DigitalProducts.Enums;

namespace VideoGameApi.Api.Dto.DigitalProducts
{
    public class CreateDigitalProductDto
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        [Required]
        public DigitalProductType ProductType { get; set; }

        [Required]
        public LicenseDuration LicenseDuration { get; set; } 

        [StringLength(500)]
        public string? Description { get; set; }

        [Range(0.01, 99999)]
        public decimal Price { get; set; }
    }
}
