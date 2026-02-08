using System.ComponentModel.DataAnnotations;

namespace VideoGameApi.Api.Dto.DigitalProducts
{
    public class AddDigitalProductKeyDto
    {
        [Required]
        public int DigitalProductId { get; set; }
        [Required]
        [StringLength(200)]
        public string ProductKey { get; set; } = string.Empty;
    }
}
