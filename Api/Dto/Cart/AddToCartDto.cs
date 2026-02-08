using System.ComponentModel.DataAnnotations;

namespace VideoGameApi.Api.Dto.Cart
{
    public class AddToCartDto
    {
        [Required]
        public int VideoGameId { get; set; }
        [Range(1, 100)]
        public int Quantity { get; set; } = 1;
    }
}
