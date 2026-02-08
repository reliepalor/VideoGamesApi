using System.ComponentModel.DataAnnotations;

namespace VideoGameApi.Api.Dto.DigitalProducts
{
    public class AdminAssignKeyDto
    {
        [Required]
        public int OrderItemId { get; set; }

        [Required]
        public int DigitalProductKeyId { get; set; }
    }
}
