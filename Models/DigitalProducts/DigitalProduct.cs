using System.ComponentModel.DataAnnotations;
using VideoGameApi.Models.DigitalProducts.Enums;

namespace VideoGameApi.Models.DigitalProducts
{
    public class DigitalProduct
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Brand { get; set; }

        [MaxLength(100)]
        public string? Platform { get; set; }
        public DigitalProductType ProductType { get; set; }
        public LicenseDuration LicenseDuration { get; set; }

        [Range(0, 999999)]
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? ImagePath { get; set; }

        public int Stock { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<DigitalProductKey> DigitalProductKeys { get; set; }
            = new List<DigitalProductKey>();
    }
}
