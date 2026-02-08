using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VideoGameApi.Models.DigitalProducts
{
    public class DigitalProductKey
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(DigitalProduct))]
        public int DigitalProductId { get; set; }
        public DigitalProduct DigitalProduct { get; set; }

        [Required]
        public string ProductKey { get; set; }

        public bool IsUsed { get; set; } = false;

        public int? OrderItemId { get; set; }

        public int? AssignedToUserId { get; set; }
        public DateTime? UsedAt { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
